using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloFaturamento
{
    [TestClass]
    [TestCategory("Teste de Unidade da Entidade: Relatório")]
    public class RelatorioTests
    {
        [TestMethod]
        public void Deve_Criar_Relatorio_Corretamente()
        {
            //Arrange
            var faturas = new List<Fatura>
    {
        new Fatura(Guid.NewGuid(), "ABC1234", DateTime.Now.AddDays(-5), DateTime.Now.AddDays(-2)),
        new Fatura(Guid.NewGuid(), "XYZ5678", DateTime.Now.AddDays(-4), DateTime.Now.AddDays(-1))
    };

            var dataInicial = DateTime.Now.AddDays(-5);
            var dataFinal = DateTime.Now;

            //Act
            var relatorio = new Relatorio(dataInicial, dataFinal, faturas);

            //Assert
            Assert.IsNotNull(relatorio.Id);
            Assert.AreEqual(dataInicial, relatorio.DataInicial);
            Assert.AreEqual(dataFinal, relatorio.DataFinal);
            Assert.AreEqual(faturas, relatorio.Faturas);
            Assert.AreEqual(0, relatorio.ValorTotal);
        }

        [TestMethod]
        public void Deve_Gerar_Valor_Total_Corretamente()
        {
            //Arrange
            var faturas = new List<Fatura>
    {
        new Fatura(Guid.NewGuid(), "ABC1234", DateTime.Now.AddDays(-5), DateTime.Now.AddDays(-2)),
        new Fatura(Guid.NewGuid(), "XYZ5678", DateTime.Now.AddDays(-4), DateTime.Now.AddDays(-1))
    };

            faturas[0].CalcularValorTotal(4, 2000);
            faturas[1].CalcularValorTotal(4, 1500);
            var relatorio = new Relatorio(DateTime.Now.AddDays(-5), DateTime.Now, faturas);

            //Act
            relatorio.GerarValorTotal();

            //Assert
            Assert.AreEqual(140, relatorio.ValorTotal);
        }
    }
}
