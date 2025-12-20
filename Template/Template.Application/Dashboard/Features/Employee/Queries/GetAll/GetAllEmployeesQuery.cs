using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Employee.Queries.GetAll;

public class GetAllEmployeesQuery
{
    public class Request:PaginationRequest,IRequest<OperationResponse<Response>>
    {
        public string ?Search { get; set; }
        public bool? IsActive { get; set; }
    }

    public class Response
    {
        public int Count { get; set; }
        public List<EmployeeRes> Employees { get; set; }

        public class EmployeeRes
        {
            public Guid EmployeeId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string FullName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public bool IsActive { get; set; }

            public static Expression<Func<Domain.Entities.Security.Employee, EmployeeRes>> Selector() => e => new()
            {
                EmployeeId = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                FullName = e.FullName,
                Email = e.Email ?? "",
                PhoneNumber = e.PhoneNumber ?? "",
                IsActive = !e.DateDeleted.HasValue
            };
        }
    }
}