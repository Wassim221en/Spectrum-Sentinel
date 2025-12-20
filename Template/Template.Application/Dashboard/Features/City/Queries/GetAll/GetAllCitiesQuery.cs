using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.City.Queries.GetAll;

public class GetAllCitiesQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        public string? Search { get; set; }
        public Guid? CountryId { get; set; }
    }

    public class Response
    {
        public int Count { get; set; }
        public List<CityRes> Cities { get; set; }

        public class CityRes
        {
            public Guid CityId { get; set; }
            public string Name { get; set; }
            public Guid CountryId { get; set; }
            public string CountryName { get; set; }

            public static Expression<Func<Domain.Entities.Settings.City, CityRes>> Selector() => c => new()
            {
                CityId = c.Id,
                Name = c.Name,
                CountryId = c.CountryId,
                CountryName = c.Country.Name
            };
        }
    }
}

