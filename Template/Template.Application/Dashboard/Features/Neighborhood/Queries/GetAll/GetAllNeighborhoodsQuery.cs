using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Neighborhood.Queries.GetAll;

public class GetAllNeighborhoodsQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        public string? Search { get; set; }
        public Guid? CityId { get; set; }
    }

    public class Response
    {
        public int Count { get; set; }
        public List<NeighborhoodRes> Neighborhoods { get; set; }

        public class NeighborhoodRes
        {
            public Guid NeighborhoodId { get; set; }
            public string Name { get; set; }
            public Guid CityId { get; set; }
            public string CityName { get; set; }

            public static Expression<Func<Domain.Entities.Settings.Neighborhood, NeighborhoodRes>> Selector() => n => new()
            {
                NeighborhoodId = n.Id,
                Name = n.Name,
                CityId = n.CityId,
                CityName = n.City.Name
            };
        }
    }
}

