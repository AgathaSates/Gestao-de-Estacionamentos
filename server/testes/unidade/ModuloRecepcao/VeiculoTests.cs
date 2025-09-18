using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloRecepcao;

[TestClass]
[TestCategory("Teste de Unidade da Entidade: Veiculo")]
public class VeiculoTests
{
    [TestMethod]
    public void Deve_Criar_Veiculo_Corretamente()
    {
        // Arrange
        var placa = "ABC1234";
        var modelo = "Carro";
        var cor = "Vermelho";
        var observacoes = "Nenhuma";

        // Act
        var veiculo = new Veiculo(placa, modelo, cor, observacoes);

        // Assert
        Assert.IsNotNull(veiculo);
        Assert.AreEqual(placa, veiculo.Placa);
        Assert.AreEqual(modelo, veiculo.Modelo);
        Assert.AreEqual(cor, veiculo.Cor);
        Assert.AreEqual(observacoes, veiculo.Observacoes);
        Assert.IsNotNull(veiculo.Id);
    }

    [TestMethod]
    public void Deve_Atualizar_Veiculo_Corretamente()
    {
        // Arrange
        var veiculo = new Veiculo("ABC1234", "Carro", "Vermelho", "Nenhuma");

        var placaNova = "XYZ5678";
        var modeloNovo = "Moto";
        var corNova = "Azul";
        var observacoesNovas = "Com arranhões";

        var veiculoEditado = new Veiculo(placaNova, modeloNovo, corNova, observacoesNovas);

        // Act
        veiculo.AtualizarRegistro(veiculoEditado);

        // Assert
        Assert.AreEqual(placaNova, veiculo.Placa);
        Assert.AreEqual(modeloNovo, veiculo.Modelo);
        Assert.AreEqual(corNova, veiculo.Cor);
        Assert.AreEqual(observacoesNovas, veiculo.Observacoes);
    }

    [TestMethod]
    public void Deve_Adicionar_Observacoes_Ao_Veiculo_Corretamente()
    {
        // Arrange
        var veiculo = new Veiculo("ABC1234", "Carro", "Vermelho");
        var observacoes = "Com arranhões";

        // Act
        veiculo.AdicionarObservacoes(observacoes);

        // Assert
        Assert.AreEqual(observacoes, veiculo.Observacoes);
    }

    [TestMethod]
    public void Deve_Adicionar_CheckIn_Ao_Veiculo_Corretamente()
    {
        // Arrange
        var veiculo = new Veiculo("ABC1234", "Carro", "Vermelho");
        var checkIn = new CheckIn(veiculo, "123.456.789-00", "João Silva");

        // Act
        veiculo.AdicionarCheckIn(checkIn);

        // Assert
        Assert.AreEqual(checkIn, veiculo.CheckIn);
    }

    [TestMethod]
    public void Deve_Criar_Veiculo_Sem_Observacoes_Corretamente()
    {
        // Arrange
        var placa = "ABC1234";
        var modelo = "Carro";
        var cor = "Vermelho";

        // Act
        var veiculo = new Veiculo(placa, modelo, cor);

        // Assert
        Assert.IsNotNull(veiculo);
        Assert.AreEqual(placa, veiculo.Placa);
        Assert.AreEqual(modelo, veiculo.Modelo);
        Assert.AreEqual(cor, veiculo.Cor);
        Assert.IsNull(veiculo.Observacoes);
        Assert.IsNotNull(veiculo.Id);
    }

    [TestMethod]
    public void Deve_Atualizar_Veiculo_Sem_Observacoes_Corretamente()
    {
        // Arrange
        var veiculo = new Veiculo("ABC1234", "Carro", "Vermelho", "Nenhuma");

        var placaNova = "XYZ5678";
        var modeloNovo = "Moto";
        var corNova = "Azul";

        var veiculoEditado = new Veiculo(placaNova, modeloNovo, corNova);

        // Act
        veiculo.AtualizarRegistro(veiculoEditado);

        // Assert
        Assert.AreEqual(placaNova, veiculo.Placa);
        Assert.AreEqual(modeloNovo, veiculo.Modelo);
        Assert.AreEqual(corNova, veiculo.Cor);
        Assert.IsNull(veiculo.Observacoes);
    }
}
