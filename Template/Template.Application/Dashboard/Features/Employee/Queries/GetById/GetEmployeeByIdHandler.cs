using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Employee.Queries.GetById;

public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery.Request, OperationResponse<GetEmployeeByIdQuery.Response>>
{
    private readonly IRepository _repository;

    public GetEmployeeByIdHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse<GetEmployeeByIdQuery.Response>> Handle(GetEmployeeByIdQuery.Request request, CancellationToken cancellationToken)
    {
        var employee = await _repository.Query<Domain.Entities.Security.Employee>()
            .Include(e => e.UserRoles)
            .Where(e => e.Id == request.Id)
            .Select(GetEmployeeByIdQuery.Response.Selector())
            .FirstOrDefaultAsync(cancellationToken);
            
        if (employee is null)
            return new HttpMessage("Employee Not Found", HttpStatusCode.NotFound);
            
        return employee;
    }
}

