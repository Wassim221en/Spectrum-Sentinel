using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Role.Commands.Delete;

public class DeleteRolesCommand
{
    public class Request : IRequest<OperationResponse>
    {
        public List<Guid> RoleIds { get; set; } = new();
    }
}

