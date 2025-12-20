using Template.Domain.Entities.Base;

namespace Template.Domain.Entities.Settings;

public class Neighborhood:Entity
{
    public string Name { get; set; }
    public Guid CityId { get; set; }
    public City City { get; set; }
}