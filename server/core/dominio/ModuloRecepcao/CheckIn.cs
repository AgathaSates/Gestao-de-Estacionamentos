using System.Diagnostics.CodeAnalysis;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
public class CheckIn : EntidadeBase<CheckIn>
{
    public Veiculo Veiculo { get; set; }
    public string CPF { get; set; }
    public string Nome { get; set; }
    public Ticket Ticket { get; set; }

    [ExcludeFromCodeCoverage]
    public CheckIn() { }

    public CheckIn(Veiculo veiculo, string cpf, string nome) : this()
    {
        Id = Guid.NewGuid();
        Veiculo = veiculo;
        CPF = cpf;
        Nome = nome;
    }

    public void AdicionarTicket(Ticket ticket)
    {
        Ticket = ticket;
    }

    public override void AtualizarRegistro(CheckIn registroEditado)
    {
        Veiculo.Placa = registroEditado.Veiculo.Placa;
        Veiculo.Modelo = registroEditado.Veiculo.Modelo;
        Veiculo.Cor = registroEditado.Veiculo.Cor;
        Veiculo.Observacoes = registroEditado.Veiculo.Observacoes;
        CPF = registroEditado.CPF;
        Nome = registroEditado.Nome;
        Ticket = Ticket;
    }
}