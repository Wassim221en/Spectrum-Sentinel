using System.ComponentModel.DataAnnotations.Schema;
using Doraemon.Domain.Primitives.Entity.Interface;
using Microsoft.AspNetCore.Identity;
using Template.Domain.Primitives.Entity.Interface;

namespace Template.Domain.Primitives.Entity.Identity;

public abstract class User : IdentityUser<Guid>, IBaseEntity<Guid>
{
    public long Number { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public List<string?> DeviceTokens { get; set; } = new();
    private string? ResetPasswordToken { get; set; }
    private DateTime? ResetPasswordTokenExpiry { get; set; }

    public ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }
    public ICollection<IdentityUserClaim<Guid>> UserClaims { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTimeOffset? DateDeleted { get; set; }
    public DateTimeOffset DateCreated { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTimeOffset? LastSeen { get; set; }

    private List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public void AddRefreshToken(RefreshToken refreshToken)
        => _refreshTokens.Add(refreshToken);

    public void RemoveRefreshToken(RefreshToken refreshToken)
        => _refreshTokens.Remove(refreshToken);

    public void ClearRefreshTokens()
        => _refreshTokens.Clear();

    public void SetResetPasswordToken(string token, DateTime expiry)
    {
        ResetPasswordToken = token;
        ResetPasswordTokenExpiry = expiry;
    }

    public void ClearResetPasswordToken()
    {
        ResetPasswordToken = null;
        ResetPasswordTokenExpiry = null;
    }

    public bool IsResetPasswordTokenValid()
    {
        return !string.IsNullOrEmpty(ResetPasswordToken)
               && ResetPasswordTokenExpiry.HasValue
               && ResetPasswordTokenExpiry.Value > DateTime.UtcNow;
    }

    public bool ValidateResetPasswordToken(string hashedToken)
    {
        return ResetPasswordToken == hashedToken && IsResetPasswordTokenValid();
    }
}