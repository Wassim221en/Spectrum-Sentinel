using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.City.Queries.GetById;

public class GetCityByIdQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        public Guid Id { get; set; }
    }

    public class Response
    {
        public Guid CityId { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
        public string CountryName { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public static Expression<Func<Domain.Entities.Settings.City, Response>> Selector() => c => new()
        {
            CityId = c.Id,
            Name = c.Name,
            CountryId = c.CountryId,
            CountryName = c.Country.Name,
            DateCreated = c.DateCreated
        };
    }
}

