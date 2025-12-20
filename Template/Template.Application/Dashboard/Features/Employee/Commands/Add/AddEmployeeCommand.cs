using MediatR;
using Microsoft.AspNetCore.Http;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Employee.Queries.GetAll;

namespace Template.Dashboard.Employee.Commands.Add;

public class AddEmployeeCommand
{
    public class Request : IRequest<OperationResponse<GetAllEmployeesQuery.Response.EmployeeRes>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public List<Guid> RoleIds { get; set; } = new();
        public Guid NeighborhoodId { get; set; }
        public IFormFile ? Image { get; set; }
    }
}