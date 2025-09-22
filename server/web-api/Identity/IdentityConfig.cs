using System.Text;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Gestao_de_Estacionamentos.WebApi.Identity;

public static class IdentityConfig
{
    public static void AddIdentityProviderConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITenantProvider, IdentityTenantProvider>();
        services.AddScoped<ITokenProvider, AccessTokenProvider>();

        services.AddIdentity<Usuario, Cargo>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddJwtAuthentication(configuration);
    }
    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var chaveAssinaturaJwt = configuration["JWT_GENERATION_KEY"]
            ?? throw new ArgumentException("Não foi possível obter a chave de assinatura de tokens.");

        var audienciaValida = configuration["JWT_AUDIENCE_DOMAIN"]
            ?? throw new ArgumentException("Não foi possível obter o domínio da audiência dos tokens.");

        var chaveEmBytes = Encoding.ASCII.GetBytes(chaveAssinaturaJwt);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(chaveEmBytes),
                ValidAudience = audienciaValida,
                ValidIssuer = "gestao-de-estacionamento",
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
            };
        });
    }
}
