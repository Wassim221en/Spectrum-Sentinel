using MediatR;
using Template.Dashboard.Core.Response;

namespace Template.Dashboard.City.Commands.Delete;

public class DeleteCitiesCommand
{
    public class Request : IRequest<OperationResponse>
    {
        public List<Guid> CityIds { get; set; } = new();
    }
}

