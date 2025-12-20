using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Neighborhood.Commands.Delete;

public class DeleteNeighborhoodsCommand
{
    public class Request : IRequest<OperationResponse>
    {
        public List<Guid> NeighborhoodIds { get; set; } = new();
    }
}

