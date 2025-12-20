using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.City.Queries.GetById;

namespace Template.Dashboard.City.Commands.Modify;

public class ModifyCityCommand
{
    public class Request : IRequest<OperationResponse<GetCityByIdQuery.Response>>
    {
        public Guid CityId { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
    }
}

