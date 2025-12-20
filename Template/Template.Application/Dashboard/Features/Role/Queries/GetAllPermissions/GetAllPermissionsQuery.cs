using MediatR;
using Template.Dashboard.Core.Response;
using Template.Domain;

namespace Template.Dashboard.Dashboard.Features.Role.Queries.GetAllPermissions;

public class GetAllPermissionsQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        
    }

    public class Response
    {
        public List<PermissionsPage>PermissionsPages { get; set; }
    }
}