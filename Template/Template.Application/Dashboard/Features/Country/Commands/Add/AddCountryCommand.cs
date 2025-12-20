using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Country.Queries.GetAll;

namespace Template.Dashboard.Country.Commands.Add;

public class AddCountryCommand
{
    public class Request : IRequest<OperationResponse<GetAllCountriesQuery.Response.CountryRes>>
    {
        public string Name { get; set; }
    }
}

