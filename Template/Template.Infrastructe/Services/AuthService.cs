using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Template.Application.Interfaces;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Interfaces;
using Template.Domain.Entities;
using Template.Domain.Entities.Security;
using Template.Domain.Exceptions.Http;
using Template.Domain.Primitives.Entity.Identity;

namespace Template.Infrastructe.Services;

public class AuthService<TUser> : IAuthService <TUser>
    where  TUser:User
{
    private readonly UserManager<TUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IRepository _repository;

    public AuthService(
        UserManager<TUser> userManager,
        IConfiguration configuration,
        IEmailService emailService, IRepository repository)
    {
        _userManager = userManager;
        _configuration = configuration;
        _emailService = emailService;
        _repository = repository;
    }
    public async Task<OperationResponse<string>>ForgetPasswordAsync(TUser user)
    {
        
        var restToken = GenerateResetToken();
        user.SetResetPasswordToken(HashToken(restToken),DateTime.UtcNow.AddHours(1));
        await _userManager.UpdateAsync(user);
        try
        {
            await _emailService.SendPasswordResetEmailAsync(
                user.Email!,
                restToken,
                user.UserName??""
            );
        }
        catch (Exception emailEx)
        {
            // Log email error but don't fail the request
            Console.WriteLine($"Failed to send email: {emailEx.Message}");
        }
        return restToken;
    }
    /*
    public async Task<AuthResponseDto> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
            
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "If the email exists, a password reset link has been sent"
                };
            }

            var resetToken = GenerateResetToken();
            user.SetResetPasswordToken(resetToken, DateTime.UtcNow.AddHours(1));

            await _userManager.UpdateAsync(user);

            // Send email with reset token
            try
            {
                await _emailService.SendPasswordResetEmailAsync(
                    user.Email!,
                    resetToken,
                    user.FullName
                );
            }
            catch (Exception emailEx)
            {
                // Log email error but don't fail the request
                Console.WriteLine($"Failed to send email: {emailEx.Message}");
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "If the email exists, a password reset link has been sent",
                Data = new { ResetToken = resetToken } // For development/testing only
            };
        }
        catch (Exception ex)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }
    */

    public async Task<OperationResponse<string>> GenerateRefreshToken(Guid  userId,string? oldRefreshToken=null, CancellationToken cancellationToken = default)
    {
        var user = await _repository.TrackingQuery<Employee>()
            .Include(e => e.RefreshTokens)
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken: cancellationToken);
        if (user is null)
            return new HttpMessage("User not found.", HttpStatusCode.NotFound);
        var token = GenerateRefreshToken();
        if (oldRefreshToken is not null)
        {
            var oldRefreshTokenHash = HashToken(oldRefreshToken ?? "");
            var oldRfToken = user.RefreshTokens.FirstOrDefault(rt =>
                !rt.DateDeleted.HasValue && rt.RefreshTokenHash == oldRefreshTokenHash && rt.IsActive());
            if (oldRfToken is null)
                return new HttpMessage("Invalied RefreshToken", HttpStatusCode.BadRequest);
            oldRfToken.Renew(token,DateTime.UtcNow+TimeSpan.FromDays(7));
        }
        else
        {
            var hashToken=HashToken(token);
            var refreshToken=new RefreshToken(hashToken,DateTime.UtcNow+TimeSpan.FromDays(7),user.Id);
            await _repository.AddAsync(refreshToken);
        }
        await _repository.SaveChangesAsync(cancellationToken);
        return token;
    }
    

    public async Task<string> GenerateAccessToken(TUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string GenerateResetToken()
    {
        byte[] bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        int value = BitConverter.ToInt32(bytes, 0) & 0x7FFFFFFF;
        int number = value % 1_000_000; 
        return number.ToString("D6");
    }


    public string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

