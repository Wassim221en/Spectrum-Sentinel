using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Common;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Role.Queries.GetAll;

public class GetAllRolesHandler:IRequestHandler<GetAllRolesQuery.Request,OperationResponse<GetAllRolesQuery.Response>>
{
    private readonly IRepository _repository;

    public GetAllRolesHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetAllRolesQuery.Response>> Handle(GetAllRolesQuery.Request request, CancellationToken cancellationToken)
    {
        var specification = new GetAllRolesSpecification(request);
        var roles = await _repository.Query<Domain.Primitives.Entity.Identity.Role>()
            .Include(r=>r.UserRoles)
            .Where(specification.Criteria)
            .Select(GetAllRolesQuery.Response.RoleRes.Selector())
            .ToListAsync(cancellationToken);
        return new GetAllRolesQuery.Response
        {
            Count = roles.Count,
            Roles = roles.ApplyPagination(request.PageSize,request.PageIndex)
        };
    }
}