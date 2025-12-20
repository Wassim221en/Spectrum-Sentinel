using Template.API.Dashboard.Events;
using Template.Domain.Events.Duties;

namespace Template.Dashboard.Dashboard.Events.Handlers;

public class AddDutyEventHandler:IIntegrationEventHandler<AddDutyEvent>
{
    private readonly IRepository _repository;

    public AddDutyEventHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(AddDutyEvent @event, CancellationToken cancellationToken = default)
    {
        
    }
}