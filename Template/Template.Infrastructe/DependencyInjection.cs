using Microsoft.Extensions.DependencyInjection;
using Template.Application.Interfaces;
using Template.Dashboard.Common.Interfaces;
using Template.Dashboard.Interfaces;
using Template.Infrastructe.Services;

namespace Template.Infrastructe;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructe(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAuthService<>), typeof(AuthService<>));
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFcmService, FcmService>();
        services.AddScoped<IFileService, FileService>();
        services.AddHttpClient<IWhatsAppService, WhatsAppService>();

        return services;
    }
}