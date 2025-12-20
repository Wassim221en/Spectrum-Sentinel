using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Template.Domain.Primitives.Entity.Interface;

namespace Template.Domain.Primitives.Entity.Identity;

public class UserRole:IdentityUserRole<Guid>
{
}