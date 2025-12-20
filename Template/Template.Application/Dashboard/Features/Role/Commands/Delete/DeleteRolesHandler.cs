using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Template.Dashboard.Core.Response;
using Template.Domain.Exceptions.Http;

namespace Template.Dashboard.Role.Commands.Delete;

public class DeleteRolesHandler : IRequestHandler<DeleteRolesCommand.Request, OperationResponse>
{
    private readonly IRepository _repository;

    public DeleteRolesHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<OperationResponse> Handle(DeleteRolesCommand.Request request, CancellationToken cancellationToken)
    {
        if (!request.RoleIds.Any())
            return new HttpMessage("No role IDs provided", HttpStatusCode.BadRequest);

        // Get roles to delete
        var roles = await _repository.TrackingQuery<Domain.Primitives.Entity.Identity.Role>()
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        if (!roles.Any())
            return new HttpMessage("No roles found", HttpStatusCode.NotFound);

        // Soft delete roles
        _repository.SoftDeleteRange(roles);
        await _repository.SaveChangesAsync(cancellationToken);

        return OperationResponse.Ok();
    }
}

