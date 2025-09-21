using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
public interface IRepositorioEstacionamento : IRepositorio<Vaga>
{
    Task<Vaga?> SelecionarPorNumeroDaVaga(int numeroVaga);
    Task<Vaga?> SelecionarPorPlacaDoVeiculo(string placa);
    Task<Veiculo?> SelecionarVeiculoPorPlaca(string placa);
    Task<Veiculo?> SelecionarVeiculoPorTicket(int? numeroTicket);
    Task<List<Veiculo>> SelecionaTodosOsVeiculosEstacionados();
    void AdicionarVeiculoAVaga(Vaga vaga, Veiculo veiculo);
    void RemoverVeiculoDaVaga(Vaga vaga);
}