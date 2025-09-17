namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento
{
    public interface IRepositorioFatura
    {
       Task<Fatura> ObterValorDeFatura();
       Task<Relatorio> GerarRelatorioFinanceiro();
       Task<int> CalcularFatura();
    }
}