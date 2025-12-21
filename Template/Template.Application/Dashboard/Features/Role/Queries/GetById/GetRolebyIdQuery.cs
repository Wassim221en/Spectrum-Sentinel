using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;
using Template.Domain.Enums;

namespace Template.Dashboard.Role.Queries.GetById;

public class GetRolebyIdQuery
{
    public class Request:IRequest<OperationResponse<Response>>
    {
        public Guid Id { get; set; }
    }

    public class Response
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public RoleStatus Status { get; set; }
        public List<string?> Permissions { get; set; }
        public static Expression<Func<Domain.Primitives.Entity.Identity.Role, Response>> Selector() => r => new()
        {
            RoleId = r.Id,
            RoleName = r.Name ?? "",
            Status = r.Status,
            Permissions = r.RoleClaims.Select(c =>c.ClaimValue).ToList()
        };
    }
}