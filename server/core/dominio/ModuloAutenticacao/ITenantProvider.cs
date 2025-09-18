namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;

public interface ITenantProvider
{
    Guid? UsuarioId { get; }
    bool IsInRole(string role);
}