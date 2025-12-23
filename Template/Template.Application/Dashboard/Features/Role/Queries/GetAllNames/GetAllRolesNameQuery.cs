using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Dashboard.Features.Role.Queries.GetAllNames;

public class GetAllRolesNameQuery
{
    public class Request:IRequest<OperationResponse<List<Response>>>
    {
        
    }

    public class Response
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }

        public static Expression<Func<Domain.Primitives.Entity.Identity.Role, Response>> Selector() => r => new()
        {
            RoleId = r.Id,
            RoleName = r.Name ?? ""
        };
    }
    
}