using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Role.Queries.GetAll;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Role.Commands.Add;

public class AddRoleHandler:IRequestHandler<AddRoleCommand.Request,OperationResponse<GetAllRolesQuery.Response.RoleRes>>
{
    private readonly RoleManager<Domain.Primitives.Entity.Identity.Role> _roleManager;

    public AddRoleHandler(RoleManager<Domain.Primitives.Entity.Identity.Role> roleManager)
    {
        _roleManager = roleManager;
    }


    public async Task<OperationResponse<GetAllRolesQuery.Response.RoleRes>> Handle(AddRoleCommand.Request request, CancellationToken cancellationToken)
    {
        var role = new Domain.Primitives.Entity.Identity.Role(request.Name,request.IsActive);
        var identityResult = await _roleManager.CreateAsync(role);
        if (!identityResult.Succeeded)
            return new HttpMessage("Adding role failed", HttpStatusCode.InternalServerError);
        foreach (var permission in request.Permissions)
        {
            identityResult = await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            if(identityResult.Succeeded is false)
                return new HttpMessage("Adding permission failed", HttpStatusCode.InternalServerError);
        }

        return new GetAllRolesQuery.Response.RoleRes()
        {
            RoleId = role.Id,
            RoleName = role.Name??"",
            IsActive = role.IsActive,
            UserCount = 0,
        };
    }
}