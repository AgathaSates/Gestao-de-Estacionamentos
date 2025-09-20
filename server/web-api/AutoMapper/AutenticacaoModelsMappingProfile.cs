using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Commands;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloAutenticacao;

namespace Gestao_de_Estacionamentos.WebApi.AutoMapper;

public class AutenticacaoModelsMappingProfile : Profile
{
    public AutenticacaoModelsMappingProfile()
    {
        CreateMap<RegistrarUsuarioRequest, RegistrarUsuarioCommand>();
        CreateMap<AutenticarUsuarioRequest, AutenticarUsuarioCommand>();
    }
}