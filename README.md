# Gestao-de-Estacionamentos

# 📌 Demonstração

## 👤 Registro de Usuário
![Demonstração do Projeto](https://i.imgur.com/35lZRxw.png)

## 🛎️ Check-in de Hospede
![Demonstração do Projeto](https://i.imgur.com/CrzKLVs.png)

# 💡 Índice

- [Demonstração](#-demonstração)
- [Introdução](#-introdução)
- [Funcionalidades](#-funcionalidades)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Tecnologias Usadas](#-tecnologias-usadas)
- [Commits e Convenções](#-commits-e-convenções)
- [Contribuidores](#-contribuidores)
- [Mentores](#-mentores)
- [Sobre o Projeto](#-sobre-o-projeto)
- [Feedback](#-feedback)
- [Como Contribuir](#-como-contribuir)

# 📚 Introdução

O Gestão de Estacionamentos é uma API REST com documentação via Swagger/OpenAPI.
Ela automatiza o ciclo de estadia de veículos: check-in (com cadastro de veículo e hóspede), alocação em vaga, checkout com cálculo de fatura pelo tempo permanecido e relatórios consolidados por período.

# ✨ Funcionalidades

- 🚦 **Check-in completo** – Registra Veículo e Hóspede no ato do check-in.

- 🅿️ **Alocação de vaga** – Após o check-in, permite reservar/ocupar uma vaga disponível.

- 🧾 **Checkout com faturamento** – Libera a vaga, calcula o valor da estadia e gera a fatura.

- 📄 **Consulta de faturas** – Listagem e detalhamento de faturas emitidas.

- 📊 **Relatório por período** – Soma o total faturado entre datas informadas.

- ✅ **Validações de regras** – Garante consistência.

- 🔎 **Exploração via Swagger** – Endpoints documentados e testáveis diretamente no /swagger.

## 🧱 Estrutura do Projeto

```text

Gestao_de_Estacionamentos
│
├── 🧠 Core.Dominio                 # Entidades, agregados e regras de negócio
│   └── (Veiculo, Hospede, Vaga, CheckIn, Fatura, etc.)
│
├── 🧭 Core.Aplicacao               # Casos de uso (handlers), DTOs e validações
│
├── 💾 Core.Infraestrutura          # Persistência (ORM/EF Core), repositórios
│
├── 🌐 Api                          # ASP.NET Core Web API + Swagger/OpenAPI
│   ├── Controllers                 # Endpoints (check-in, vagas, faturas, relatórios)
│   └── Configurações               # Swagger, DI, middlewares
│
└── 🧪 Tests                        
    ├── Api                         # Testes automatizados de contrato
    └── Aplicacao/Dominio           # Testes de casos de uso e regras

```
- 🧠 **Domínio:** Modela a realidade do estacionamento (veículos, hóspedes, vagas, estadias e faturamento).

- 🧭 **Aplicação:** Orquestra casos de uso (check-in, relatório), valida entradas e retorna resultados.

- 💾 **Infraestrutura:** Isola detalhes de persistência (repositórios, mapeamentos, migrações).

- 🌐 **API:** Exposição HTTP dos recursos com documentação no Swagger.

- 🧪 **Tests:** Cobrem contratos dos fluxos principais (check-in/out, fatura, relatório).

# 🔧 Tecnologias Usadas

Este projeto utiliza (ou é compatível com) as seguintes tecnologias:

- 💻 C# – Linguagem principal

- ⚙️ ASP.NET Core Web API – Construção de serviços REST

- 📘 Swagger/OpenAPI – Documentação e teste dos endpoints

- 🧱 Entity Framework Core – Acesso a dados (ORM)

- 🗄️ Banco relacional – Ex.: SQL Server (configurável)

- 🧪 Testes automatizados – Foco em contrato dos casos de uso/endpoints

- 🔄 Git & GitHub – Versionamento e colaboração

# 🧠 Commits e Convenções

Utilizamos [Conventional Commits](https://www.conventionalcommits.org/pt-br/v1.0.0/) para padronizar as mensagens de commit.

# 👥 Contribuidores

<p align="left">
  <a href="https://github.com/AgathaSates">
    <img src="https://github.com/AgathaSates.png" width="100" style="border-radius: 50%;" alt="Tiago Santini"/>
    &nbsp;&nbsp;&nbsp;
  <a href="https://github.com/AlexAraldi">
    <img src="https://github.com/AlexAraldi.png" width="100" style="border-radius: 50%;" alt="Alexandre Rech"/>
  </a>
</p>

| Nome | GitHub |
|------|--------|
| Agatha Sates | [@AgathaSates](https://github.com/AgathaSates) |
| Alexander Araldi | [@Alexander Araldi](https://github.com/AlexAraldi) |

# 👨‍🏫 Mentores

<p align="left" style="margin-left: 27px;">
  <a href="https://github.com/tiagosantini">
    <img src="https://github.com/tiagosantini.png" width="100" style="border-radius: 50%;" alt="Tiago Santini"/>
  </a>
  &nbsp;&nbsp;&nbsp;
  <a href="https://github.com/alexandre-rech-lages">
    <img src="https://github.com/alexandre-rech-lages.png" width="100" style="border-radius: 50%;" alt="Alexandre Rech"/>
  </a>
</p>


| Nome | GitHub |
|------|--------|
| Tiago Santini | [@Tiago Santini](https://github.com/tiagosantini) |
| Alexandre Rech | [@Alexandre Rech](https://github.com/alexandre-rech-lages) |

# 🏫 Sobre o Projeto

Desenvolvido durante o curso Fullstack da [Academia do Programador](https://academiadoprogramador.net) 2025

# 💬 Feedback

Se você tiver sugestões de melhoria, novas ideias ou quiser nos avisar sobre um bug, abra uma [Issue](https://github.com/Code-Oblivion/E-Agenda/issues) ou entre em contato!

# 🤝 Como Contribuir

1. 🍴 Faça um fork
2. 🛠️ Crie uma branch `feature/sua-feature`
3. 🔃 Commit com mensagens semânticas (`feat: nova tela`)
4. 📥 Abra um Pull Request e aguarde o review
