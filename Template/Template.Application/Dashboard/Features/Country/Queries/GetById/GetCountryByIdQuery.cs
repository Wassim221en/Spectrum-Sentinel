using System.Linq.Expressions;
using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Country.Queries.GetById;

public class GetCountryByIdQuery
{
    public class Request : IRequest<OperationResponse<Response>>
    {
        public Guid Id { get; set; }
    }

    public class Response
    {
        public Guid CountryId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DateCreated { get; set; }

        public static Expression<Func<Domain.Entities.Settings.Country, Response>> Selector() => c => new()
        {
            CountryId = c.Id,
            Name = c.Name,
            DateCreated = c.DateCreated
        };
    }
}

