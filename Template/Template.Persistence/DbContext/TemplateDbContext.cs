using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Template.Dashboard.DbContext;
using Template.Domain.Entities;
using Template.Domain.Entities.Notifications;
using Template.Domain.Entities.Security;
using Template.Domain.Primitives.Entity.Identity;

namespace Template.Persistence.DbContext;

public class TemplateDbContext:IdentityDbContext<User,Role,Guid>, ITemplateDbContext
{
    public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<EmployeeNotification> UserNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        builder.Entity<Role>()
            .HasMany(r => r.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.Entity<User>()
            .HasMany(u => u.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();
        builder.Entity<User>()
            .HasMany(u => u.UserClaims)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();
        builder.Entity<Role>()
            .HasMany(r=>r.RoleClaims)
            .WithOne()
            .HasForeignKey(ur=>ur.RoleId)
            .IsRequired();
        builder.Entity<Role>()
            .Property(r => r.Number)
            .UseIdentityAlwaysColumn()
            .HasIdentityOptions(startValue: 1, incrementBy: 1)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);;

    }
}