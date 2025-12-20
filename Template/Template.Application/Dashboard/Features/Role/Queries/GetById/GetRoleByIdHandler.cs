using System.Net;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Role.Queries.GetById;

public class GetRoleByIdHandler:IRequestHandler<GetRolebyIdQuery.Request ,OperationResponse<GetRolebyIdQuery.Response>>
{
    private readonly IRepository _repository;
    private readonly RoleManager<Domain.Primitives.Entity.Identity.Role> _roleManager;

    public GetRoleByIdHandler(IRepository repository, RoleManager<Domain.Primitives.Entity.Identity.Role> roleManager)
    {
        _repository = repository;
        _roleManager = roleManager;
    }

    public async Task<OperationResponse<GetRolebyIdQuery.Response>> Handle(GetRolebyIdQuery.Request request, CancellationToken cancellationToken)
    {
        var role = await _repository.Query<Domain.Primitives.Entity.Identity.Role>()
            .Include(r=>r.RoleClaims)
            .Where(r => r.Id == request.Id)
            .Select(GetRolebyIdQuery.Response.Selector()).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if(role is null)
            return new HttpMessage("Role Not Found", HttpStatusCode.NotFound);
        return role;
    }
}