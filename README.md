# PicpaySimplificado

Resumo
------
Aplicação API REST monolítica em camadas (DDD) para gerenciamento de carteiras e transferências simplificadas, com autorizações e notificações. Contém projeto principal (API) e projeto de testes automatizados.

O que a aplicação faz
---------------------
- Gerencia carteiras (clientes) com CPF/CNPJ, email, tipo de usuário e saldo.
- Registra transferências entre carteiras com validações de saldo e regras de negócio.
- Consulta histórico de transferências.
- Integra com um serviço externo de autorização (IAutorizadorService) e um serviço de notificações (INotificacaoService).
- Documentação automática via Swagger (apenas em ambiente de desenvolvimento).

- Documentação (dev)
  - GET /swagger
  - GET /swagger/index.html

Tecnologias e bibliotecas
-------------------------
- .NET 9 (target)
- C#
- ASP.NET Core Web API
- Entity Framework Core (Pomelo MySQL provider)
- MySQL (MySqlConnector)
- Swagger / Swashbuckle
- xUnit, Moq, FluentAssertions (projeto de testes)
- Grafana (visualização das métricas)
- Prometheus (para métricas de observabilidade)
- Docker

Configuração e execução
-----------------------
Pré-requisitos:
- .NET 9 SDK
- MySQL (compatível com MySQL 8.x)
- Atualizar connection string em appsettings.json com a chave `defaultConnection`

Executar API:
1. Ajuste a connection string (`defaultConnection`) em appsettings.json.
2. Executar:
   - docker compose -f docker-compose.monitoring.yml down
   - docker compose up -d
3. Rodar as migrations no banco do docker

Executar testes:
- dotnet test PicpaySimplificado.Tests

Arquitetura: Monolítica em camadas seguindo DDD
----------------------------------------------
A solução adota arquitetura monolítica organizada em camadas alinhadas a princípios DDD (Domain-Driven Design):

- Presentation (API)
  - Controllers: expõem rotas REST, recebem DTOs/requests, devolvem responses e códigos HTTP.
- Application / Services
  - Serviços de aplicação (ex.: ICarteiraService, ITransferenciaService, IAutorizadorService, INotificacaoService)
  - Orquestram casos de uso, regras de aplicação e coordenação entre repositórios e serviços externos.
- Domain
  - Entidades e objetos de valor (ex.: Carteira, Transferencia, enums de tipo de usuário)
  - Regras de negócio (validações de saldo, restrições de exclusão, invariantes)
- Infra
  - Persistência (ApplicationDbContext — EF Core)
  - Repositórios (ICarteiraRepository, ITransferenciaRepository) — implementam acesso ao banco
  - Integrations/HttpClients (AutorizadorService como HttpClient)

Por ser monolítico, todas as camadas estão no mesmo deploy/solução, mas claramente separadas por responsabilidades para manter coesão, testabilidade e facilitar evolução.

![alt text](/imagens/grafana_picpaysimplificado.png)
