using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Role.Queries.GetAll;
using Template.Domain.Enums;

namespace Template.Dashboard.Role.Commands.Add;

public class AddRoleCommand
{
    public class Request:IRequest<OperationResponse<GetAllRolesQuery.Response.RoleRes>>
    {
        public string Name { get; set; }
        public RoleStatus Status { get; set; }
        public List<string>Permissions { get; set; }
    }
    
}