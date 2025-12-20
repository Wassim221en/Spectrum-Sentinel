using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Interfaces;
using Template.Domain.Exceptions.Http;
using Template.Domain.Primitives.Entity.Identity;

namespace Template.API.Employee.Commands.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand.Request,OperationResponse<LoginCommand.Response>>
{
    private readonly IAuthService<Domain.Entities.Security.Employee> _authService;
    private readonly UserManager<Domain.Entities.Security.Employee> _userManager;
    private readonly IRepository _repository;
    public LoginCommandHandler(IAuthService<Domain.Entities.Security.Employee> authService, UserManager<Domain.Entities.Security.Employee> userManager, IRepository repository)
    {
        _authService = authService;
        _userManager = userManager;
        _repository = repository;
    }

    public async Task<OperationResponse<LoginCommand.Response>> Handle(LoginCommand.Request request, CancellationToken cancellationToken)
    {
        var employee = await _userManager.Users
            .Include(e=>e.RefreshTokens)
            .Include(e=>e.UserRoles)
            .FirstOrDefaultAsync(e=>e.Email == request.Email || e.PhoneNumber==request.PhoneNumber||e.UserName==request.UserName, cancellationToken: cancellationToken);
        if (employee is null)
            return new HttpMessage("User Not Found!", HttpStatusCode.BadRequest);
        if (!await _userManager.CheckPasswordAsync(employee, request.Password))
            return new HttpMessage("Password Or Email Is incorect", HttpStatusCode.BadRequest);
        var token = await _authService.GenerateAccessToken(employee);
        var refreshToken = await _authService.GenerateRefreshToken(employee.Id,null, cancellationToken);
        if (refreshToken.Success is false)
            return refreshToken.ErrorMessage!;
        var roleIds = employee.UserRoles.Select(ur => ur.RoleId);
        var roles = await _repository.Query<Role>()
            .Include(r=>r.RoleClaims)
            .Where(r => roleIds.Contains(r.Id)).ToListAsync(cancellationToken);
        return new LoginCommand.Response()
        {
            UserId = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email ?? "",
            RefreshToken = refreshToken.Data!,
            Token = token,
            Roles = roles.Select(r => new LoginCommand.Response.RoleRes
            {
                RoleId = r.Id,
                RoleName = r.Name ?? "",
                Permissions = r.RoleClaims.Select(c => c.ClaimValue).ToList()
            }).ToList()
        };
    }
}

