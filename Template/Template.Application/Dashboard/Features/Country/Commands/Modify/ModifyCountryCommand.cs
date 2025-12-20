using MediatR;
using Template.Dashboard.Core.Response;
using Template.Dashboard.Country.Queries.GetById;

namespace Template.Dashboard.Country.Commands.Modify;

public class ModifyCountryCommand
{
    public class Request : IRequest<OperationResponse<GetCountryByIdQuery.Response>>
    {
        public Guid CountryId { get; set; }
        public string Name { get; set; }
    }
}

