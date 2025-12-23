using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Dashboard.Features.Role.Queries.GetAllNames;

public class GetAllRolesNamesHandler:IRequestHandler<GetAllRolesNameQuery.Request,OperationResponse<List<GetAllRolesNameQuery.Response>>>
{
    private readonly IRepository _repository;

    public GetAllRolesNamesHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<List<GetAllRolesNameQuery.Response>>> Handle(GetAllRolesNameQuery.Request request, CancellationToken cancellationToken)
    {
        var roles = await _repository.Query<Domain.Primitives.Entity.Identity.Role>()
            .Select(GetAllRolesNameQuery.Response.Selector())
            .ToListAsync(cancellationToken);
        return roles;

    }
}