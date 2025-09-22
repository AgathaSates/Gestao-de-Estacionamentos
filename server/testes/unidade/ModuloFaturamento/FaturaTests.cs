using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloFaturamento
{
    [TestClass]
    [TestCategory("Teste de Unidade da Entidade: Fatura")]
    public class FaturaTests
    {
        [TestMethod]
        public void Deve_Criar_Fatura_Corretamente()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var placaVeiculo = "ABC1234";
            var dataEntrada = new DateTime(2023, 10, 1);
            var dataSaida = new DateTime(2023, 10, 5);

            // Act
            var fatura = new Fatura(ticketId, placaVeiculo, dataEntrada, dataSaida);
            // Assert

            Assert.IsNotNull(fatura.Id);
            Assert.AreEqual(ticketId, fatura.TicketId);
            Assert.AreEqual(placaVeiculo, fatura.PlacaVeiculo);
            Assert.AreEqual(dataEntrada, fatura.DataEntrada);
            Assert.AreEqual(dataSaida, fatura.DataSaida);
            Assert.AreEqual(0, fatura.Valortotal);
        }

        [TestMethod]
        public void Deve_Calcular_Valor_Total_Corretamente()
        {
            // Arrange
            var fatura = new Fatura(Guid.NewGuid(), "ABC1234", DateTime.Now, DateTime.Now.AddDays(3));
            int numeroDiarias = 4;
            decimal valorDiaria = 2000;

            // Act
            fatura.CalcularValorTotal(numeroDiarias, valorDiaria);

            // Assert
            Assert.AreEqual(80, fatura.Valortotal);
        }

        [TestMethod]
        public void Deve_Calcular_Numero_Diarias_Corretamente()
        {
            // Arrange
            var fatura = new Fatura(Guid.NewGuid(), "ABC1234", DateTime.Now, DateTime.Now.AddDays(3));
            var dataEntrada = new DateTime(2023, 10, 1);
            var dataSaida = new DateTime(2023, 10, 5);

            // Act
            int numeroDiarias = fatura.CalcularNumeroDiarias(dataEntrada, dataSaida);

            // Assert
            Assert.AreEqual(5, numeroDiarias);
        }
    }
}