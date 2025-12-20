using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core;
using Template.Dashboard.Core.Response;
using Template.Domain.Primitives.Entity.Identity;

namespace Template.Dashboard.Role.Queries.GetAll;

public class GetAllRolesQuery
{
    public class Request:PaginationRequest,IRequest<OperationResponse<Response>>
    {
        public string? Search { get; set; }
        public bool ?IsActive { get; set; }
    }

    public class Response
    {
        public int Count { get; set; }
        public List<RoleRes> Roles { get; set; }
        public class RoleRes
        {
            public Guid RoleId { get; set; }
            public string RoleName { get; set; }
            public int UserCount { get; set; }
            public bool IsActive { get; set; }

            public static Expression<Func<Domain.Primitives.Entity.Identity.Role, RoleRes>> Selector() => r => new()
            {
                RoleId=r.Id,
                RoleName=r.Name??"",
                UserCount = r.UserRoles.Count(),
                IsActive=r.IsActive
            };
        }
    }
}