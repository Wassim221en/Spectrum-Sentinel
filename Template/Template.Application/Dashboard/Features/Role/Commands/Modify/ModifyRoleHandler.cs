using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Employee.Queries.GetAll;
using Template.Dashboard.Role.Queries.GetById;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Role.Commands.Modify;

public class ModifyRoleHandler:IRequestHandler<ModifyRoleCommand.Request,OperationResponse<GetRolebyIdQuery.Response>>
{
    private readonly RoleManager<Domain.Primitives.Entity.Identity.Role> _roleManager;
    private readonly IRepository _repository;

    public ModifyRoleHandler(RoleManager<Domain.Primitives.Entity.Identity.Role> roleManager, IRepository repository)
    {
        _roleManager = roleManager;
        _repository = repository;
    }

    public async Task<OperationResponse<GetRolebyIdQuery.Response>> Handle(ModifyRoleCommand.Request request, CancellationToken cancellationToken)
    {
        var role = await _repository.TrackingQuery<Domain.Primitives.Entity.Identity.Role>()
            .Include(r=>r.RoleClaims)
            .Include(r=>r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == request.RoleId,cancellationToken);
        if (role is null)
            return new HttpMessage("Role not found", HttpStatusCode.NotFound);
        role.ClearAllUsers();
        var users = await _repository.Query<Domain.Entities.Security.Employee>()
            .Where(u => request.UserIds.Contains(u.Id)).ToListAsync(cancellationToken);
        role.AddUsers(users.Select(u=>u.Id).ToList());
        role.ClearAllPermissions();
        role.AddPermissions(request.Permissions);
        _repository.Update(role);
        await _repository.SaveChangesAsync(cancellationToken);
        return await _repository.Query<Domain.Primitives.Entity.Identity.Role>()
            .Include(r=>r.RoleClaims)
            .Where(r => r.Id == request.RoleId)
            .Select(GetRolebyIdQuery.Response.Selector()).FirstAsync(cancellationToken: cancellationToken);

    }
}