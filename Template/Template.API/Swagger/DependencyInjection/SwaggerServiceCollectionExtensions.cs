using Swashbuckle.AspNetCore.SwaggerGen;

namespace Template.API.DependencyInjection;

public static class SwaggerServiceCollectionExtensions
{
    public static IServiceCollection AddNeptuneeSwagger(this IServiceCollection services, Action<SwaggerGenOptions>? options = null)
    {
        if (options is not null)
        {
            services.Configure(options);
        }

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(o =>
        {
            o.EnableAnnotations(true, true);

            // Custom schema ID generator to avoid conflicts
            o.CustomSchemaIds(type =>
            {
                if (type.IsGenericType)
                {
                    var genericTypeName = type.GetGenericTypeDefinition().Name.Replace("`1", "").Replace("`2", "");
                    var genericArgs = string.Join("", type.GetGenericArguments().Select(t =>
                    {
                        if (t.IsNested)
                            return t.DeclaringType?.Name + t.Name;
                        return t.Name;
                    }));
                    return genericTypeName + "Of" + genericArgs;
                }

                if (type.IsNested)
                    return type.DeclaringType?.Name + type.Name;

                return type.Name;
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{AppDomain.CurrentDomain.FriendlyName}.xml");
            if (File.Exists(xmlPath))
            {
                o.IncludeXmlComments(xmlPath,true);
            }
        });

        return services;
    }

   
}