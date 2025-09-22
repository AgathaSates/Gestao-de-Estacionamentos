using Microsoft.OpenApi.Models;

namespace Gestao_de_Estacionamentos.WebApi.Swagger;

public static class SwaggerConfig
{
    public static void AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Gestao de Estacionamento API", Version = "v1" });

            options.MapType<TimeSpan>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "time-span",
                Example = new Microsoft.OpenApi.Any.OpenApiString("00:00:00")
            });

            options.MapType<DateTime>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date-time",
                Example = new Microsoft.OpenApi.Any.OpenApiString("2025-09-18T00:00:00")
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Informe o token JWT no padrão \"Bearer {token}\"",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
    }
}
