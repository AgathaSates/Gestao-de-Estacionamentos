using System.Text.Json.Serialization;
using Gestao_de_Estacionamentos.Core.Aplicacao;
using Gestao_de_Estacionamentos.WebApi.AutoMapper;
using Gestao_de_Estacionamentos.WebApi.Swagger;
using Gestao_de_Estacionamentos.Infraestutura.Orm;
using Gestao_de_Estacionamentos.WebApi.Identity;
using Gestao_de_Estacionamentos.WebApi.Orm;

namespace Gestao_de_Estacionamentos.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
                .AddCamadaAplicacao(builder.Logging, builder.Configuration)
                .AddCamadaInfraestruturaOrm(builder.Configuration);

        builder.Services.AddAutoMapperProfiles(builder.Configuration);
        builder.Services.AddIdentityProviderConfig(builder.Configuration);

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddSwaggerConfig();

        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.ApplyMigrations();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}