using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Gestao_de_Estacionamentos.Core.Aplicacao;
public static class DependencyInjection
{
    public static IServiceCollection AddCamadaAplicacao(
        this IServiceCollection services,
        ILoggingBuilder logging,
        IConfiguration configuration
    )
    {
        var assembly = typeof(DependencyInjection).Assembly;
        var licensekey = configuration["AUTOMAPPER_LICENSE_KEY"];
        var redisConnectionString = configuration["REDIS_CONNECTION_STRING"];

        if (string.IsNullOrWhiteSpace(licensekey))
            throw new Exception("A variável AUTOMAPPER_LICENSE_KEY não foi fornecida.");

        if (string.IsNullOrWhiteSpace(redisConnectionString))
            throw new Exception("A variável REDIS_CONNECTION_STRING não foi fornecida.");


        //injeção de dependências

        services.AddSerilogConfig(logging, configuration);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.LicenseKey = licensekey;
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.LicenseKey = licensekey;

        }, assembly);

        services.AddValidatorsFromAssembly(assembly);

        services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = redisConnectionString;
            opt.InstanceName = "gestao-de-estacionamento";
        });

        return services;
    }

    private static void AddSerilogConfig(this IServiceCollection services, ILoggingBuilder logging, IConfiguration configuration)
    {
        var licenseKey = configuration["NEWRELIC_LICENSE_KEY"];

        if (string.IsNullOrWhiteSpace(licenseKey))
            throw new Exception("A variável NEWRELIC_LICENSE_KEY não foi fornecida.");

        var caminhoAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var caminhoArquivoLogs = Path.Combine(caminhoAppData, "gestao-de-estacionamento", "erro.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(caminhoArquivoLogs, LogEventLevel.Error)
            .WriteTo.NewRelicLogs(
                endpointUrl: "https://log-api.newrelic.com/log/v1",
                applicationName: "gestao-de-estacionamento-app",
                licenseKey: licenseKey
            )
            .CreateLogger();

        logging.ClearProviders();
        services.AddSerilog();
    }
}
