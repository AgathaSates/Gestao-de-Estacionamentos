using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento
{
    public interface IRepositorioFatura : IRepositorio<Fatura>
    {
       Task<Fatura> ObterFaturaAsync(Guid ticketId);      
       decimal CalcularValorFatura(int NumeroDiarias, decimal valorDiaria);        
    }
}