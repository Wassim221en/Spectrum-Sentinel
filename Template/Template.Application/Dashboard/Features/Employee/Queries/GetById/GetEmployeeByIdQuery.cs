using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public List<string> Roles { get; set; }

        public static Expression<Func<Domain.Entities.Security.Employee, Response>> Selector() => e => new()
        {
            EmployeeId = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            FullName = e.FullName,
            Email = e.Email ?? "",
            PhoneNumber = e.PhoneNumber ?? "",
            EmailConfirmed = e.EmailConfirmed,
            PhoneNumberConfirmed = e.PhoneNumberConfirmed,
            IsActive = !e.DateDeleted.HasValue,
            DateCreated = e.DateCreated,
            Roles = e.UserRoles.Select(ur => ur.RoleId.ToString()).ToList()
        };
    }
}

