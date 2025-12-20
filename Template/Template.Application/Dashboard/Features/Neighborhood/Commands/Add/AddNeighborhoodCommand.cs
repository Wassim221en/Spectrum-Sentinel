using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Neighborhood.Queries.GetAll;

namespace Template.Dashboard.Neighborhood.Commands.Add;

public class AddNeighborhoodCommand
{
    public class Request : IRequest<OperationResponse<GetAllNeighborhoodsQuery.Response.NeighborhoodRes>>
    {
        public string Name { get; set; }
        public Guid CityId { get; set; }
    }
}

