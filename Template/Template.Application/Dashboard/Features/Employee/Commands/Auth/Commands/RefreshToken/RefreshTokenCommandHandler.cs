using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Interfaces;
using Template.Domain.Exceptions.Http;
using Template.Domain.Primitives.Entity.Identity;

namespace Template.API.Employee.Commands.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand.Request, OperationResponse<RefreshTokenCommand.Response>>
{
    private readonly IAuthService<Domain.Entities.Security.Employee> _authService;
    private readonly UserManager<Domain.Entities.Security.Employee> _userManager;
    private readonly IRepository _repository;

    public RefreshTokenCommandHandler(IAuthService<Domain.Entities.Security.Employee> authService, UserManager<Domain.Entities.Security.Employee> userManager, IRepository repository)
    {
        _authService = authService;
        _userManager = userManager;
        _repository = repository;
    }

    public async Task<OperationResponse<RefreshTokenCommand.Response>> Handle(RefreshTokenCommand.Request request, CancellationToken cancellationToken)
    {
        var employee = await _repository.TrackingQuery<Domain.Entities.Security.Employee>()
            .Include(e => e.RefreshTokens)
            .Include(e=>e.UserRoles)
            .FirstOrDefaultAsync(e => e.Id == request.UserId, cancellationToken: cancellationToken);
        if (employee is null)
            return new HttpMessage("User Not Found!", HttpStatusCode.NotFound);
        var refreshToken = await _authService.GenerateRefreshToken(employee.Id,request.RefreshToken,cancellationToken);
        if (!refreshToken.Success)
            return refreshToken.ErrorMessage!;
        var token = await _authService.GenerateAccessToken(employee);
        var roleIds = employee.UserRoles.Select(ur => ur.RoleId);
        var roles = await _repository.Query<Role>()
            .Include(r=>r.RoleClaims)
            .Where(r => roleIds.Contains(r.Id)).ToListAsync(cancellationToken);
        return new RefreshTokenCommand.Response()
        {
            UserId = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email??"",
            RefreshToken =refreshToken.Data??"",
            Token = token,
            Roles = roles.Select(r => new RefreshTokenCommand.Response.RoleRes
            {
                RoleId = r.Id,
                RoleName = r.Name ?? "",
                Permissions = r.RoleClaims.Select(c => c.ClaimValue).ToList()
            }).ToList()
        };
    }
}

