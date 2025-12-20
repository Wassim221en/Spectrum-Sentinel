namespace Template.Domain.Events.Duties;

public class AddDutyEvent:IntegrationEvent
{
    public Guid DutyId { get; set; }
    public string Title { get; set; }
    public List<Guid>EmployeeIds { get; set; }

    public AddDutyEvent(Guid dutyId, string title, List<Guid> employeeIds)
    {
        DutyId = dutyId;
        Title = title;
        EmployeeIds = employeeIds;
    }
}