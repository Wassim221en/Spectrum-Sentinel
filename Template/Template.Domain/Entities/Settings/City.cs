using Template.Domain.Entities.Base;

namespace Template.Domain.Entities.Settings;

public class City:Entity
{
    public string Name { get; set; }
    public Guid CountryId { get; set; }
    public Country Country { get; set; }
}