using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Neighborhood.Queries.GetById;

public class GetNeighborhoodByIdQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        public Guid Id { get; set; }
    }

    public class Response
    {
        public Guid NeighborhoodId { get; set; }
        public string Name { get; set; }
        public Guid CityId { get; set; }
        public string CityName { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public static Expression<Func<Domain.Entities.Settings.Neighborhood, Response>> Selector() => n => new()
        {
            NeighborhoodId = n.Id,
            Name = n.Name,
            CityId = n.CityId,
            CityName = n.City.Name,
            DateCreated = n.DateCreated
        };
    }
}

