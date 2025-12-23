using MediatR;
using Microsoft.AspNetCore.Http;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Employee.Queries.GetAll;
using Template.Domain.Enums;

namespace Template.Dashboard.Employee.Commands.Add;

public class AddEmployeeCommand
{
    public class Request : IRequest<OperationResponse<GetAllEmployeesQuery.Response.EmployeeRes>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public Guid RoleId { get; set; } = new();
        public EmployeeStatus Status { get; set; }
    }
}