using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Employee.Commands.Delete;

public class DeleteEmployeesCommand
{
    public class Request : IRequest<OperationResponse>
    {
        public List<Guid> EmployeeIds { get; set; } = new();
    }
}

