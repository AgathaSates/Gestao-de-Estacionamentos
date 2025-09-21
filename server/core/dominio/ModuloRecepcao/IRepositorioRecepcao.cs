using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
public interface IRepositorioRecepcao : IRepositorio<CheckIn> 
{
    Task<Veiculo?> SelecionarVeiculoPorPlaca(string placa);
    Task<Veiculo?> SelecionarVeiculoPorTicket(int? numeroTicket);
}