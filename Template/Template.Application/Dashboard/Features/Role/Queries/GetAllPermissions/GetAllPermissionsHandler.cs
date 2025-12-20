using MediatR;
using Template.Dashboard.Core.Response;
using Template.Domain;

namespace Template.Dashboard.Dashboard.Features.Role.Queries.GetAllPermissions;

public class GetAllPermissionsHandler:IRequestHandler<GetAllPermissionsQuery.Request,OperationResponse<GetAllPermissionsQuery.Response>>
{
    
    public async Task<OperationResponse<GetAllPermissionsQuery.Response>> Handle(GetAllPermissionsQuery.Request request, CancellationToken cancellationToken)
    {
        return new GetAllPermissionsQuery.Response()
        {
            PermissionsPages = Permissions.GetAllPermissions()
        };
    }
}