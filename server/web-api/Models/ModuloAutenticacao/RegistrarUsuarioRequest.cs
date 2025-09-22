namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloAutenticacao;

public record RegistrarUsuarioRequest(
    string NomeCompleto,
    string Email,
    string Senha,
    string ConfirmarSenha
);
