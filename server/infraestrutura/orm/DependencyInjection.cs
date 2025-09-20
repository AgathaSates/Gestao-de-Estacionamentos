using Gestao_de_Estacionamento.Infraestrutura.Conf;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFatura;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloFatura;
using Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloRecepcao;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm;
public static class DependencyInjection
{
    public static IServiceCollection AddCamadaInfraestruturaOrm(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRepositorioRecepcao, RepositorioRecepcaoEmOrm>();
        services.AddScoped<IRepositorioFatura, RepositorioFaturaEmOrm>();
        services.AddScoped<IRepositorioRelatorio, RepositorioRelatorioEmOrm>();
        services.AddScoped<IRepositorioConfiguracao, RepositorioConfiguracao>();

        services.AddEntityFrameworkConfig(configuration);

        return services;
    }

    private static void AddEntityFrameworkConfig(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration["SQL_CONNECTION_STRING"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("A variável SQL_CONNECTION_STRING não foi fornecida.");

        services.AddDbContext<IUnitOfWork, AppDbContext>(options =>
            options.UseNpgsql(connectionString, (opt) => opt.EnableRetryOnFailure(3)));
    }
}