using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Microsoft.Extensions.Configuration;

namespace Gestao_de_Estacionamento.Infraestrutura.Conf
{
    public class RepositorioConfiguracao(IConfiguration configuracao) : IRepositorioConfiguracao
    {
        public decimal ObterValorDiaria()
        {
            return Convert.ToDecimal(configuracao["ValorDiaria"]);
        }
    }
}
