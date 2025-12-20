using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.City.Queries.GetAll;

namespace Template.Dashboard.City.Commands.Add;

public class AddCityCommand
{
    public class Request : IRequest<OperationResponse<GetAllCitiesQuery.Response.CityRes>>
    {
        public string Name { get; set; }
        public Guid CountryId { get; set; }
    }
}

