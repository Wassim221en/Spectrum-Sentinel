using Template.Domain.Entities.Settings;
using Template.Domain.Enums;
using Template.Domain.Primitives.Entity.Identity;

namespace Template.Domain.Entities.Security;

public class Employee : User
{
    public EmployeeStatus Status { get; set; }
}