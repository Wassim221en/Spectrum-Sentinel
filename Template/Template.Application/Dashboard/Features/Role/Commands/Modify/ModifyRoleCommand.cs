using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Role.Queries.GetById;

namespace Template.Dashboard.Role.Commands.Modify;

public class ModifyRoleCommand
{
    public class Request:IRequest<OperationResponse<GetRolebyIdQuery.Response>>
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public List<Guid>UserIds { get; set; }
        public List<string>Permissions { get; set; }
    }
}