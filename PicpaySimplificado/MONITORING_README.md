# Monitoramento PicPay Simplificado - Grafana + Prometheus

Este guia explica como configurar e usar o sistema de monitoramento da aplicação PicPay Simplificado com Prometheus e Grafana.

## ?? Métricas Monitoradas

### Métricas de Negócio
- **Total de Transferências**: Contador de todas as transferências realizadas (por status)
- **Carteiras Ativas**: Número atual de carteiras no sistema
- **Carteiras Criadas**: Contador de novas carteiras (por tipo de usuário)
- **Autorizações Negadas**: Contador de transferências não autorizadas
- **Valores de Transferência**: Distribuição dos valores transferidos
- **Duração de Transferências**: Tempo de processamento das transferências (P50, P95, P99)
- **Erros de Validação**: Tipos de erros encontrados durante validações

### Métricas Técnicas (HTTP)
- **Requisições HTTP**: Taxa de requisições por segundo
- **Códigos de Status HTTP**: Distribuição de respostas (200, 400, 500, etc)
- **Latência HTTP**: Tempo de resposta das requisições

## ?? Como Executar

### Pré-requisitos
- Docker e Docker Compose instalados
- API PicPay Simplificado rodando na porta 5000

### Passo 1: Subir os Contêineres

```bash
docker-compose -f docker-compose.monitoring.yml up -d
```

Isso irá iniciar:
- **Prometheus** na porta `9090`
- **Grafana** na porta `3000`

### Passo 2: Acessar o Grafana

1. Abra o navegador em: `http://localhost:3000`
2. Faça login com:
   - **Usuário**: `admin`
   - **Senha**: `admin123`

### Passo 3: Visualizar o Dashboard

O dashboard **"PicPay Simplificado - Dashboard Completo"** será carregado automaticamente.

## ?? Painéis do Dashboard

### 1. **Visão Geral (Top)**
- Total de Transferências
- Carteiras Ativas
- Autorizações Negadas
- Taxa de Requisições HTTP

### 2. **Análise de Transferências**
- Taxa de Transferências por Status (linha do tempo)
- Distribuição de Status (gráfico de pizza)
- Latência de Transferências (percentis P50, P95, P99)
- Distribuição de Valores

### 3. **Análise de Carteiras e Erros**
- Taxa de Criação de Carteiras por Tipo
- Taxa de Erros de Validação por Tipo
- Requisições HTTP por Código de Status (tabela)

## ?? Configuração

### Alterar Porta da API

Se sua API estiver em outra porta, edite o arquivo `prometheus.yml`:

```yaml
scrape_configs:
  - job_name: 'picpay-api'
    static_configs:
      - targets: ['host.docker.internal:SUA_PORTA']
```

### Alterar Credenciais do Grafana

Edite o arquivo `docker-compose.monitoring.yml`:

```yaml
environment:
  - GF_SECURITY_ADMIN_USER=seu_usuario
  - GF_SECURITY_ADMIN_PASSWORD=sua_senha
```

## ?? Acessar Prometheus Diretamente

Para queries personalizadas, acesse: `http://localhost:9090`

### Exemplos de Queries Úteis

```promql
# Taxa de transferências bem-sucedidas
rate(picpay_transferencias_total{status="sucesso"}[5m])

# Percentil 95 de latência
histogram_quantile(0.95, sum(rate(picpay_transferencia_duracao_segundos_bucket[5m])) by (le))

# Total de erros de validação
sum(picpay_erros_validacao_total)

# Carteiras criadas por tipo
sum by(tipo_usuario) (picpay_carteiras_criadas_total)
```

## ?? Parar os Serviços

```bash
docker-compose -f docker-compose.monitoring.yml down
```

Para remover também os volumes (dados persistidos):

```bash
docker-compose -f docker-compose.monitoring.yml down -v
```

## ?? Troubleshooting

### Grafana não mostra dados

1. Verifique se a API está rodando: `curl http://localhost:5000/metrics`
2. Verifique se o Prometheus está coletando dados: `http://localhost:9090/targets`
3. Certifique-se de que `host.docker.internal` está acessível (Windows/Mac)
   - No Linux, use `172.17.0.1` ou configure network_mode: "host"

### Métricas não aparecem

1. Execute algumas operações na API (criar carteiras, fazer transferências)
2. Aguarde 5-10 segundos (intervalo de scrape)
3. Refresh no dashboard do Grafana

## ?? Personalização

### Adicionar Novas Métricas

1. Edite `PicpaySimplificado/Metrics/ApplicationMetrics.cs`
2. Adicione novos contadores/histogramas/gauges
3. Use as métricas nos seus serviços
4. Crie novos painéis no Grafana apontando para as novas métricas

### Exemplo de Nova Métrica

```csharp
public static readonly Counter MinhaMetrica = Prometheus.Metrics
    .CreateCounter("picpay_minha_metrica_total", "Descrição da métrica");

// Usar no código
ApplicationMetrics.MinhaMetrica.Inc();
```

## ?? Alertas (Configuração Futura)

Para configurar alertas, você pode:
1. Usar Alertmanager do Prometheus
2. Configurar alertas no Grafana
3. Integrar com Slack, Email, PagerDuty, etc.

## ?? Referências

- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [prometheus-net Library](https://github.com/prometheus-net/prometheus-net)
