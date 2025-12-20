using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Employee.Commands.Delete;

public class DeleteEmployeesHandler : IRequestHandler<DeleteEmployeesCommand.Request, OperationResponse>
{
    private readonly IRepository _repository;

    public DeleteEmployeesHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse> Handle(DeleteEmployeesCommand.Request request, CancellationToken cancellationToken)
    {
        if (!request.EmployeeIds.Any())
            return new HttpMessage("No employee IDs provided", HttpStatusCode.BadRequest);

        // Get employees to delete
        var employees = await _repository.TrackingQuery<Domain.Entities.Security.Employee>()
            .Where(e => request.EmployeeIds.Contains(e.Id))
            .ToListAsync(cancellationToken);

        if (!employees.Any())
            return new HttpMessage("No employees found", HttpStatusCode.NotFound);

        // Soft delete employees
        _repository.SoftDeleteRange(employees);
        await _repository.SaveChangesAsync(cancellationToken);

        return OperationResponse.Ok();
    }
}

