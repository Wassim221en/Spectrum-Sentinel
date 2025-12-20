using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Common;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Employee.Queries.GetAll;

public class GetAllEmployeesHandler : IRequestHandler<GetAllEmployeesQuery.Request, OperationResponse<GetAllEmployeesQuery.Response>>
{
    private readonly IRepository _repository;

    public GetAllEmployeesHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetAllEmployeesQuery.Response>> Handle(GetAllEmployeesQuery.Request request, CancellationToken cancellationToken)
    {
        var specification = new GetAllEmployeesSpecification(request);
        var employees = await _repository.Query<Domain.Entities.Security.Employee>()
            .Where(specification.Criteria)
            .Select(GetAllEmployeesQuery.Response.EmployeeRes.Selector())
            .ToListAsync(cancellationToken);
            
        return new GetAllEmployeesQuery.Response
        {
            Count = employees.Count,
            Employees = employees.ApplyPagination(request.PageSize,request.PageIndex)
        };
    }
}

