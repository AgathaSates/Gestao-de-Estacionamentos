using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
public class Vaga : EntidadeBase<Vaga>
{
    public int NumeroVaga { get; set; }
    public char Zona { get; private set; } 
    public bool EstaOcupada { get; set; }
    public Veiculo? VeiculoEstacionado { get; set; }

    public Vaga() { }

    public Vaga(char zona) : this()
    {
        Id = Guid.NewGuid();
        EstaOcupada = false;
        Zona = char.ToUpperInvariant(zona);
    }

    public void AdicionarVeiculo(Veiculo veiculo)
    {
        VeiculoEstacionado = veiculo;
        EstaOcupada = true;
    }

    public void RemoverVeiculo()
    {
        VeiculoEstacionado = null;
        EstaOcupada = false;
    }

    public override void AtualizarRegistro(Vaga registroEditado)
    {
        Zona = char.ToUpperInvariant(registroEditado.Zona);
    }
}
