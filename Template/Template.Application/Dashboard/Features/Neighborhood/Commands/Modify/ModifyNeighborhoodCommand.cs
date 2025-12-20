using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Neighborhood.Queries.GetById;

namespace Template.Dashboard.Neighborhood.Commands.Modify;

public class ModifyNeighborhoodCommand
{
    public class Request : IRequest<OperationResponse<GetNeighborhoodByIdQuery.Response>>
    {
        public Guid NeighborhoodId { get; set; }
        public string Name { get; set; }
        public Guid CityId { get; set; }
    }
}

