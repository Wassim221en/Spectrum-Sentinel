using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.Country.Commands.Delete;

public class DeleteCountriesCommand
{
    public class Request : IRequest<OperationResponse>
    {
        public List<Guid> CountryIds { get; set; } = new();
    }
}

