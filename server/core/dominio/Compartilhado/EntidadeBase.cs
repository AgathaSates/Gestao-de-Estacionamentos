namespace Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;

public abstract class EntidadeBase<T>
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public abstract void AtualizarRegistro(T registroEditado);
}