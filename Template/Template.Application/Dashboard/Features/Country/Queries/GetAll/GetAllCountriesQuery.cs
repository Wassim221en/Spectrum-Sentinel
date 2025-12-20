using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Country.Queries.GetAll;

public class GetAllCountriesQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        public string? Search { get; set; }
    }

    public class Response
    {
        public int Count { get; set; }
        public List<CountryRes> Countries { get; set; }

        public class CountryRes
        {
            public Guid CountryId { get; set; }
            public string Name { get; set; }

            public static Expression<Func<Domain.Entities.Settings.Country, CountryRes>> Selector() => c => new()
            {
                CountryId = c.Id,
                Name = c.Name
            };
        }
    }
}

