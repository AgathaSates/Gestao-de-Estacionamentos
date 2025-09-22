using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Gestao_de_Estacionamentos.Testes.Integracao.Compartilhado;

namespace Gestao_de_Estacionamentos.Testes.Integracao.ModuloFaturamento
{
    public class RepositorioRelatorioEmOrmTests : TestFixture
    {
        [TestMethod]
        public async Task Deve_Cadastrar_Relatorio_Corretamente()
        {
            // Arrange
            var dataInicio = DateTime.UtcNow;
            var dataFim = dataInicio.AddDays(2);

            Veiculo veiculo = new Veiculo("TEST567", "modelo", "cor", "vidro quebrado");
            CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");

            await _repositorioRecepcao!.CadastrarAsync(checkIn);
            await _dbContext!.SaveChangesAsync();

            Fatura fatura1 = new Fatura(checkIn.Ticket.Id, checkIn.Veiculo.Placa, dataInicio, dataFim);
            Fatura fatura2 = new Fatura(checkIn.Ticket.Id, checkIn.Veiculo.Placa, dataInicio, dataFim);
            var faturasCadastradas = new List<Fatura> { fatura1, fatura2 };

            await _repositorioFatura!.CadastrarEntidadesAsync(faturasCadastradas);
            await _dbContext.SaveChangesAsync();

            Relatorio relatorio = new Relatorio(dataInicio, dataFim, faturasCadastradas);
            // Act
            await _repositorioRelatorio!.CadastrarAsync(relatorio);
            await _dbContext.SaveChangesAsync();

            Relatorio? relatorioEncontrado = await _repositorioRelatorio!
                .SelecionarRegistroPorIdAsync(relatorio.Id);

            // Assert
            Assert.AreEqual(relatorio, relatorioEncontrado);
        }

        [TestMethod]
        public async Task Deve_Gerar_Relatorio_Corretamente()
        {
            // Arrange
            var dataInicio = DateTime.UtcNow;
            var dataFim = dataInicio.AddDays(2);

            Veiculo veiculo = new Veiculo("TEST567", "modelo", "cor", "vidro quebrado");
            CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");

            await _repositorioRecepcao!.CadastrarAsync(checkIn);
            await _dbContext!.SaveChangesAsync();

            Fatura fatura1 = new Fatura(checkIn.Ticket.Id, checkIn.Veiculo.Placa, dataInicio, dataFim);
            Fatura fatura2 = new Fatura(checkIn.Ticket.Id, checkIn.Veiculo.Placa, dataInicio, dataFim);

            var faturasCadastradas = new List<Fatura> { fatura1, fatura2 };

            await _repositorioFatura!.CadastrarEntidadesAsync(faturasCadastradas);
            await _dbContext.SaveChangesAsync();

            Relatorio relatorioEsperado = new Relatorio(dataInicio, dataFim, faturasCadastradas);

            // Act
            Relatorio relatorioGerado = await _repositorioRelatorio!
                .GerarRelatorioFinanceiroAsync(dataInicio, dataFim, faturasCadastradas);

            // Assert
            Assert.AreEqual(relatorioEsperado.ValorTotal, relatorioGerado.ValorTotal);
        }
    }
}
