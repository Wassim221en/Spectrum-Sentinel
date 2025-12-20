using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Role.Queries.GetAll;

namespace Template.Dashboard.Role.Commands.Add;

public class AddRoleCommand
{
    public class Request:IRequest<OperationResponse<GetAllRolesQuery.Response.RoleRes>>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<string>Permissions { get; set; }
    }
    
}