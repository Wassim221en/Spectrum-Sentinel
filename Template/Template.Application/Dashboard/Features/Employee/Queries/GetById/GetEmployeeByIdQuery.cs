using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;
using Template.Domain.Enums;

namespace Template.Dashboard.Employee.Queries.GetById;

public class GetEmployeeByIdQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        public Guid Id { get; set; }
    }

    public class Response
    {
        public Guid EmployeeId { get; set; }
        public long Number { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string Role { get; set; }

        public static Expression<Func<Domain.Entities.Security.Employee, Response>> Selector() => e => new()
        {
            EmployeeId = e.Id,
            Number = e.Number,
            FirstName = e.FirstName,
            LastName = e.LastName,
            FullName = e.FullName,
            Email = e.Email ?? "",
            PhoneNumber = e.PhoneNumber ?? "",
            Role = e.UserRoles.FirstOrDefault().RoleId.ToString(),
            DateCreated = e.DateCreated,
            Status = e.Status,
            
        };
    }
}

