using MediatR;
using Microsoft.AspNetCore.Http;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Employee.Queries.GetById;

namespace Template.Dashboard.Employee.Commands.Modify;

public class ModifyEmployeeCommand
{
    public class Request : IRequest<OperationResponse<GetEmployeeByIdQuery.Response>>
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<Guid> RoleIds { get; set; } = new();
    }
}

