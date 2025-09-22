using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloEstacionamento;
[TestClass]
[TestCategory("Teste de Unidade de Entidade: Vaga")]
public class VagaTests
{
    [TestMethod]
    public void Deve_Criar_Vaga_Corretamente()
    {
        // Arrange
        char zona = 'A';

        // Act
        var vaga = new Vaga(zona);

        // Assert
        Assert.IsNotNull(vaga.Id);
        Assert.AreEqual(char.ToUpperInvariant(zona), vaga.Zona);
        Assert.IsFalse(vaga.EstaOcupada);
        Assert.IsNull(vaga.VeiculoEstacionado);
    }

    [TestMethod]
    public void Deve_Adicionar_Veiculo_Corretamente()
    {
        // Arrange
        var vaga = new Vaga('B');
        var veiculo = new Veiculo("XYZ1234", "Sedan", "Azul");

        // Act
        vaga.AdicionarVeiculo(veiculo);

        // Assert
        Assert.IsTrue(vaga.EstaOcupada);
        Assert.AreEqual(veiculo, vaga.VeiculoEstacionado);
    }

    [TestMethod]
    public void Deve_Remover_Veiculo_Corretamente()
    {
        // Arrange
        var vaga = new Vaga('C');
        var veiculo = new Veiculo("XYZ1234", "Sedan", "Azul");

        vaga.AdicionarVeiculo(veiculo);

        // Act
        vaga.RemoverVeiculo();

        // Assert
        Assert.IsFalse(vaga.EstaOcupada);
        Assert.IsNull(vaga.VeiculoEstacionado);
    }

    [TestMethod]
    public void Deve_Atualizar_Registro_Corretamente()
    {
        // Arrange
        var vaga = new Vaga('D');
        var vagaEditada = new Vaga('E');

        // Act
        vaga.AtualizarRegistro(vagaEditada);

        // Assert
        Assert.AreEqual(char.ToUpperInvariant(vagaEditada.Zona), vaga.Zona);
    }
}
