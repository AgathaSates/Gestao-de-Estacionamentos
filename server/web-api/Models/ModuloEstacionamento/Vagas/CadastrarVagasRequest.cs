namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Vagas;

public record CadastrarVagasRequest(int quantidadeParaGerar, char zona);

public record CadastrarVagasResponse(int quantidadeParaGerar, char zona);
