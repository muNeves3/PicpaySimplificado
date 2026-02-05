# ?? Quick Start - Monitoramento Grafana

## Passo a Passo Rápido

### 1 Iniciar a API
```bash
cd PicpaySimplificado
dotnet run
```

A API deve estar rodando em `http://localhost:5000`

### 2 Verificar se as métricas estão disponíveis
Abra no navegador: `http://localhost:5000/metrics`

Você deve ver algo como:
```
# HELP picpay_transferencias_total Total de transferências realizadas
# TYPE picpay_transferencias_total counter
picpay_transferencias_total{status="sucesso"} 0
...
```

### 3 Iniciar Prometheus e Grafana
```bash
docker-compose -f docker-compose.monitoring.yml up -d
```

### 4 Acessar Grafana
1. Abra: `http://localhost:3000`
2. Login: `admin` / `admin123`
3. O dashboard já estará disponível automaticamente!

### 5 Gerar Dados para o Dashboard
Execute algumas operações na API:

**Criar carteira:**
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

**Fazer transferência:**
```bash
curl -X POST http://localhost:5000/transfer \
  -H "Content-Type: application/json" \
  -d '{
    "emetenteId": 1,
    "destinatarioId": 2,
    "valor": 100
  }'
```

### 6 Ver as Métricas no Dashboard
Volte ao Grafana e veja os gráficos sendo atualizados em tempo real! 

##  Painéis Principais

- **Total de Transferências**: Quantas transferências foram feitas
- **Taxa de Sucesso/Falha**: Linha do tempo de transferências
- **Latência**: Quanto tempo as transferências levam
- **Distribuição de Valores**: Quais valores são mais transferidos
- **Carteiras Ativas**: Quantas carteiras existem
- **Erros**: Tipos de erros que ocorrem

##  Parar Tudo
```bash
docker-compose -f docker-compose.monitoring.yml down
```

##  Notas Importantes

- O Grafana atualiza a cada 5 segundos
- O Prometheus coleta dados da API a cada 5 segundos
- Os dados são persistidos nos volumes Docker
- Para limpar tudo: `docker-compose -f docker-compose.monitoring.yml down -v`

##  Problemas?

**Dashboard vazio?**
- Execute operações na API (criar carteiras, transferências)
- Aguarde 5-10 segundos
- Dê refresh na página

**Prometheus não conecta?**
- Verifique se a API está rodando: `curl http://localhost:5000/metrics`
- No Linux, edite `prometheus.yml` e troque `host.docker.internal` por `172.17.0.1`

---

Para documentação completa, veja [MONITORING_README.md](MONITORING_README.md)
