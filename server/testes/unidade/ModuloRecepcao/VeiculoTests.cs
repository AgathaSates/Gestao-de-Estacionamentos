using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloRecepcao;
[TestClass]
[TestCategory("Teste de Unidade de Entidade: Veiculo")]
public sealed class VeiculoTests
{
    [TestMethod]
    public void Deve_Criar_Veiculo_Com_Propriedades_Corretas()
    {
        // Arrange
        string placa = "ABC-1234";
        string modelo = "Toyota Corolla";
        string cor = "Prata";
        string observacoes = "Nenhuma";

        // Act
        Veiculo veiculo = new Veiculo(placa, modelo, cor, observacoes);

        // Assert
        Assert.AreNotEqual(Guid.Empty, veiculo.Id);
        Assert.AreEqual(placa, veiculo.Placa);
        Assert.AreEqual(modelo, veiculo.Modelo);
        Assert.AreEqual(cor, veiculo.Cor);
        Assert.AreEqual(observacoes, veiculo.Observacoes);
        Assert.IsNull(veiculo.CheckIn);
        Assert.IsNull(veiculo.Vaga);
    }

    [TestMethod]
    public void Deve_Adicionar_Observacoes_Corretamente()
    {
        // Arrange
        Veiculo veiculo = new Veiculo("ABC-1234", "Toyota Corolla", "Prata");
        string novasObservacoes = "Estacionado na vaga 5";

        // Act
        veiculo.AdicionarObservacoes(novasObservacoes);

        // Assert
        Assert.AreEqual(novasObservacoes, veiculo.Observacoes);
    }

    [TestMethod]
    public void Deve_Adicionar_CheckIn_Corretamente()
    {
        // Arrange
        Veiculo veiculo = new Veiculo("ABC-1234", "Toyota Corolla", "Prata");
        var checkIn = new CheckIn();

        // Act
        veiculo.AdicionarCheckIn(checkIn);

        // Assert
        Assert.AreEqual(checkIn, veiculo.CheckIn);
    }

    [TestMethod]
    public void Deve_Adicionar_Vaga_Corretamente()
    {
        // Arrange
        Veiculo veiculo = new Veiculo("ABC-1234", "Toyota Corolla", "Prata");
        var vaga = new Vaga('A');

        // Act
        veiculo.AdicionarVaga(vaga);

        // Assert
        Assert.AreEqual(vaga, veiculo.Vaga);
    }

    [TestMethod]
    public void Deve_Atualizar_Registro_Corretamente()
    {
        // Arrange
        Veiculo veiculo = new Veiculo("ABC-1234", "Toyota Corolla", "Prata", "Nenhuma");
        Veiculo veiculoEditado = new Veiculo("XYZ-5678", "Honda Civic", "Preto", "Com arranhões");

        // Act
        veiculo.AtualizarRegistro(veiculoEditado);

        // Assert
        Assert.AreEqual("XYZ-5678", veiculo.Placa);
        Assert.AreEqual("Honda Civic", veiculo.Modelo);
        Assert.AreEqual("Preto", veiculo.Cor);
        Assert.AreEqual("Com arranhões", veiculo.Observacoes);
    }
}
