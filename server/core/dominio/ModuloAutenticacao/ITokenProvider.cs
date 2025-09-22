namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;

public interface ITokenProvider
{
    AccessToken GerarAccessToken(Usuario usuario);
}

public record AccessToken(
    string Chave,
    DateTime Expiracao,
    UsuarioAutenticado UsuarioAutenticado
);
