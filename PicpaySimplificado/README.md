# PicpaySimplificado

## ?? Resumo

Aplicação API REST monolítica em camadas (DDD) para gerenciamento de carteiras e transferências simplificadas, com autorizações, notificações e observabilidade completa via Prometheus/Grafana. Contém projeto principal (API) e projeto de testes automatizados com cobertura de entidades, serviços e controllers.

---

## ?? O que a aplicação faz

- **Gerencia carteiras digitais** com CPF/CNPJ, email, tipo de usuário (USUARIO/LOJISTA) e saldo
- **Processa transferências** entre carteiras com validações de saldo e regras de negócio
- **Valida operações** através de serviço externo de autorização
- **Envia notificações** após transferências bem-sucedidas
- **Monitora métricas** em tempo real (transferências, carteiras, erros, latência)
- **Documentação automática** via Swagger (ambiente de desenvolvimento)
- **Observabilidade completa** com Prometheus e Grafana

---

## ??? Principais Rotas

### Carteiras
```
GET    /api/carteiras         — Lista todas as carteiras
GET    /api/carteiras/{id}    — Obtém carteira por ID
POST   /api/carteiras         — Cria nova carteira
PUT    /api/carteiras/{id}    — Atualiza carteira
DELETE /api/carteiras/{id}    — Remove carteira
```

### Transferências
```
GET    /transfer              — Lista transferências
GET    /transfer/{id}         — Obtém transferência por ID
POST   /transfer              — Realiza transferência
```

### Observabilidade
```
GET    /metrics               — Métricas Prometheus
GET    /swagger               — Documentação Swagger
```

---

## ??? Tecnologias e Bibliotecas

### Backend
- **.NET 9** - Framework principal
- **C#** - Linguagem de programação
- **ASP.NET Core Web API** - Framework web
- **Entity Framework Core 9.0** - ORM
- **Pomelo.EntityFrameworkCore.MySql 9.0** - Provider MySQL
- **MySQL 8.x** - Banco de dados

### Observabilidade
- **Prometheus.AspNetCore 8.2** - Exportação de métricas
- **OpenTelemetry 1.15** - Telemetria e tracing
- **Grafana** - Visualização de métricas
- **Prometheus** - Coleta e armazenamento de métricas

### Testes
- **xUnit 2.9** - Framework de testes
- **Moq 4.20** - Biblioteca de mocking
- **FluentAssertions 8.8** - Assertions fluentes

### Outros
- **Swashbuckle.AspNetCore 10.1** - Documentação Swagger
- **SourceFlow.Stores.EntityFramework** - Padrão Repository

---

## ?? Configuração e Execução

### Pré-requisitos
- **.NET 9 SDK** instalado
- **MySQL 8.x** rodando
- **Docker** (opcional, para observabilidade)

### 1?? Configurar Banco de Dados

Edite `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "defaultConnection": "Server=localhost;Port=3306;Database=picpaysimplificado;User=root;Password=sua_senha"
  }
}
```

### 2?? Aplicar Migrations

```bash
cd PicpaySimplificado
dotnet ef database update
```

### 3?? Executar a API

```bash
dotnet run --project PicpaySimplificado
```

A API estará disponível em:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger**: `http://localhost:5000/swagger`
- **Métricas**: `http://localhost:5000/metrics`

### 4?? Executar Testes

```bash
dotnet test
```

---

## ?? Observabilidade e Monitoramento

### Iniciar Prometheus e Grafana

```bash
docker-compose -f docker-compose.monitoring.yml up -d
```

### Acessar Dashboards

- **Grafana**: `http://localhost:3000`
  - Usuário: `admin`
  - Senha: `admin123`
  
- **Prometheus**: `http://localhost:9090`

### Métricas Disponíveis

#### Métricas de Negócio
- `picpay_transferencias_total` - Total de transferências (por status)
- `picpay_carteiras_criadas_total` - Carteiras criadas (por tipo)
- `picpay_carteiras_ativas` - Quantidade de carteiras ativas
- `picpay_autorizacoes_negadas_total` - Autorizações negadas
- `picpay_transferencia_valor` - Distribuição de valores
- `picpay_transferencia_duracao_segundos` - Duração das transferências
- `picpay_erros_validacao_total` - Erros de validação (por tipo)

#### Métricas HTTP
- `http_requests_received_total` - Total de requisições HTTP
- `http_request_duration_seconds` - Duração das requisições

?? **Documentação completa**: [MONITORING_README.md](MONITORING_README.md)  
?? **Guia rápido**: [MONITORING_QUICKSTART.md](MONITORING_QUICKSTART.md)

---

## ?? Testes Automatizados

### Estrutura de Testes

```
PicpaySimplificado.Tests/
??? Models/
?   ??? TransferenciaTests.cs          # Testes da entidade Transferencia
??? Services/
?   ??? CarteiraServiceTests.cs        # Testes do serviço de carteiras
?   ??? TransferenciaServiceTests.cs   # Testes do serviço de transferências
??? Controllers/
?   ??? CarteiraControllerTests.cs     # Testes do controller de carteiras
?   ??? TransferenciaControllerTests.cs # Testes do controller de transferências
??? Utils/
    ??? CPFCNPJValidatorTests.cs       # Testes de validação CPF/CNPJ
```

### Cobertura de Testes

#### Entidades (Models)
- ? Criação de transferências
- ? Geração de IDs únicos
- ? Validação de valores
- ? Navigation properties

#### Serviços (Services)
- ? Criação de carteiras
- ? Validação de duplicidade
- ? Autorização de transferências
- ? Validação de saldo
- ? Restrições de lojistas
- ? Transações e rollback
- ? Notificações

#### Controllers
- ? Retornos HTTP corretos
- ? Validação de requests
- ? Tratamento de erros
- ? Serialização de respostas

### Executar Testes com Cobertura

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

---

## ??? Arquitetura: Monolítica em Camadas (DDD)

A solução segue os princípios de **Domain-Driven Design (DDD)** com separação clara de responsabilidades:

### ?? Camadas da Aplicação

#### 1. **Presentation (API)**
- **Controllers**: Expõem endpoints REST
- **DTOs**: Objetos de transferência de dados
- **Requests/Responses**: Contratos de entrada/saída

**Exemplo**: `CarteiraController`, `TransferenciaController`

#### 2. **Application (Services)**
- **Serviços de Aplicação**: Orquestram casos de uso
- **Integração Externa**: Comunicação com APIs externas
- **Validações de Negócio**: Regras de aplicação

**Exemplo**: `CarteiraService`, `TransferenciaService`, `AutorizadorService`, `NotificacaoService`

#### 3. **Domain (Models)**
- **Entidades**: Objetos de negócio com identidade
- **Value Objects**: Objetos imutáveis
- **Enums**: Tipos enumerados
- **Regras de Negócio**: Lógica de domínio

**Exemplo**: `Carteira`, `Transferencia`, `UserType`

#### 4. **Infrastructure (Infra)**
- **Persistência**: Context do EF Core
- **Repositórios**: Acesso a dados
- **Migrations**: Versionamento do schema
- **Configurações**: DbContext e mapeamentos

**Exemplo**: `ApplicationDbContext`, `CarteiraRepository`, `TransferenciaRepository`

#### 5. **Cross-Cutting (Observabilidade)**
- **Métricas**: Coleta de dados Prometheus
- **Logging**: Registro de eventos
- **Tracing**: Rastreamento de requisições

**Exemplo**: `ApplicationMetrics`, OpenTelemetry

### ?? Fluxo de uma Transferência

```
1. TransferenciaController (Presentation)
   ? recebe TransferenciaRequest
2. TransferenciaService (Application)
   ? valida e orquestra
3. AutorizadorService (Application)
   ? autoriza externamente
4. Carteira (Domain)
   ? aplica regras de negócio (débito/crédito)
5. CarteiraRepository (Infrastructure)
   ? persiste no banco
6. NotificacaoService (Application)
   ? notifica usuários
7. ApplicationMetrics (Cross-Cutting)
   ? registra métricas
8. Response ? Cliente
```

### ? Benefícios da Arquitetura

- ? **Separação de Responsabilidades**: Cada camada tem propósito claro
- ? **Testabilidade**: Fácil criar testes unitários e de integração
- ? **Manutenibilidade**: Mudanças isoladas em camadas específicas
- ? **Escalabilidade**: Preparado para evolução (microserviços no futuro)
- ? **Domínio Rico**: Lógica de negócio centralizada no Domain

---

## ?? Exemplos de Uso

### Criar Carteira

```bash
curl -X POST http://localhost:5000/api/carteiras \
  -H "Content-Type: application/json" \
  -d '{
    "nomeCompleto": "João Silva",
    "cpfcnpj": "12345678901",
    "email": "joao@email.com",
    "senha": "senha123",
    "userType": 0,
    "saldo": 1000
  }'
```

### Fazer Transferência

```bash
curl -X POST http://localhost:5000/transfer \
  -H "Content-Type: application/json" \
  -d '{
    "emetenteId": 1,
    "destinatarioId": 2,
    "valor": 100
  }'
```

### Ver Métricas

```bash
curl http://localhost:5000/metrics
```

---

## ?? Regras de Negócio

### Carteiras
- ? CPF/CNPJ e Email devem ser únicos
- ? Dois tipos: USUARIO (0) e LOJISTA (1)
- ? Saldo inicial pode ser zero ou positivo

### Transferências
- ? Apenas usuários comuns podem transferir (lojistas apenas recebem)
- ? Saldo do emetente deve ser suficiente
- ? Requer autorização de serviço externo
- ? Transacional: falha reverte todas as operações
- ? Notificação enviada após sucesso

---

## ?? Troubleshooting

### Erro de Conexão com Banco de Dados
```bash
# Verificar se MySQL está rodando
mysql -u root -p

# Criar banco manualmente se necessário
CREATE DATABASE picpaysimplificado;
```

### Dashboard Grafana Vazio
```bash
# 1. Verificar se API está expondo métricas
curl http://localhost:5000/metrics

# 2. Verificar se Prometheus está coletando
# Abra http://localhost:9090/targets

# 3. Executar operações na API para gerar dados
# 4. Aguardar 5-10 segundos e dar refresh
```

### Testes Falhando
```bash
# Restaurar pacotes NuGet
dotnet restore

# Limpar e rebuildar
dotnet clean
dotnet build

# Executar testes novamente
dotnet test
```

---

## ?? Contribuição

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

### Padrões de Código
- Seguir convenções C# e .NET
- Manter separação de camadas DDD
- Adicionar testes para novas funcionalidades
- Documentar mudanças importantes

---

## ?? Licença

Projeto sem licença especificada. Adicione um arquivo `LICENSE` conforme necessário.

---

## ?? Documentação Adicional

- [MONITORING_README.md](MONITORING_README.md) - Guia completo de monitoramento
- [MONITORING_QUICKSTART.md](MONITORING_QUICKSTART.md) - Início rápido Grafana
- [Swagger](http://localhost:5000/swagger) - Documentação da API (quando rodando)

---

## ?? Roadmap Futuro

- [ ] Autenticação e autorização (JWT)
- [ ] Rate limiting
- [ ] Cache distribuído (Redis)
- [ ] Testes de integração com banco real
- [ ] CI/CD pipeline
- [ ] Containerização completa (Docker)
- [ ] Health checks avançados
- [ ] Alertas automáticos (Alertmanager)

---

**Desenvolvido com ?? usando .NET 9**
