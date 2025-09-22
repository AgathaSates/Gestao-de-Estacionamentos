namespace Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;

public interface IUnitOfWork
{
    Task CommitAsync();
    Task RollbackAsync();
}
