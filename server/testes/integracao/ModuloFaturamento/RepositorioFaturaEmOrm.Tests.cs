using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Testes.Integracao.Compartilhado;

namespace Gestao_de_Estacionamentos.Testes.Integracao.ModuloFaturamento
{

    [TestClass]
    [TestCategory("Testes de Integração de Faturamento")]
    public class RepositorioFaturamentoEmOrm : TestFixture
    {

        [TestMethod]
        public void Deve_Calcular_Valor_Fatura_Corretamente()
        {
            // Arrange
            int numeroDiarias = 5;
            decimal valorDiaria = 20000; // Valor em centavos (R$ 200,00)
            decimal valorEsperado = (numeroDiarias * valorDiaria) / 100; // Convertendo para reais

            // Act
            decimal valorCalculado = _repositorioFatura!.CalcularValorFatura(numeroDiarias, valorDiaria);

            // Assert
            Assert.AreEqual(valorEsperado, valorCalculado);
        }

        [TestMethod]
        public async Task Deve_Selecionar_Faturas_Por_Periodo_Corretamente()
        {
            // Arrange
            var dataInicio = DateTime.UtcNow;
            var dataFim = dataInicio.AddDays(2);

            Veiculo veiculo = new Veiculo("TEST567", "modelo", "cor", "vidro quebrado");
            Veiculo veiculo2 = new Veiculo("TEST000", "modelo2", "cor2", "vidro quebrado2");
            CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");
            CheckIn checkIn2 = new CheckIn(veiculo2, "11122233344", "Maria");

            await _repositorioRecepcao!.CadastrarAsync(checkIn);
            await _repositorioRecepcao!.CadastrarAsync(checkIn2);
            await _dbContext!.SaveChangesAsync();

            Fatura fatura1 = new Fatura(checkIn.Ticket.Id, checkIn.Veiculo.Placa, dataInicio, dataFim);
            Fatura fatura2 = new Fatura(checkIn2.Ticket.Id, checkIn2.Veiculo.Placa, dataInicio.AddDays(1), dataFim.AddDays(1));

            var faturasCadastradas = new List<Fatura> { fatura1, fatura2 };
            var faturasEsperadasOrdenadas = new List<Fatura> { fatura1 }.OrderBy(f => f.DataEntrada);

            await _repositorioFatura!.CadastrarEntidadesAsync(faturasCadastradas);
            await _dbContext.SaveChangesAsync();

            // Act
            List<Fatura> faturasEncontradas = await _repositorioFatura!
                .SelecionarFaturasPorPeriodoAsync(dataInicio, dataFim);

            // Assert
            CollectionAssert.AreEqual(faturasEsperadasOrdenadas.ToList(), faturasEncontradas);
        }

        [TestMethod]
        public async Task Deve_Selecionar_Fatura_Por_Id_Corretamente()
        {
            // Arrange
            var dataInicio = DateTime.UtcNow;
            var dataFim = dataInicio.AddDays(2);

            Veiculo veiculo = new Veiculo("TEST567", "modelo", "cor", "vidro quebrado");
            CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");

            await _repositorioRecepcao!.CadastrarAsync(checkIn);
            await _dbContext!.SaveChangesAsync();

            Fatura fatura1 = new Fatura(checkIn.Ticket.Id, checkIn.Veiculo.Placa, dataInicio, dataFim);

            await _repositorioFatura!.CadastrarAsync(fatura1);
            await _dbContext.SaveChangesAsync();

            // Act
            Fatura? faturaEncontrada = await _repositorioFatura!
                .SelecionarRegistroPorIdAsync(fatura1.Id);

            // Assert
            Assert.AreEqual(fatura1, faturaEncontrada);
        }

        [TestMethod]
        public async Task Deve_Selecionar_Por_Id_Retornar_Nulo_Quando_Nao_Encontrar()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();
            // Act
            Fatura? faturaEncontrada = await _repositorioFatura!
                .SelecionarRegistroPorIdAsync(idInexistente);
            // Assert
            Assert.IsNull(faturaEncontrada);
        }
    }
}
