# ?? Comandos Rápidos - PicpaySimplificado Docker

## Executar pela primeira vez

### Linux/Mac/WSL
```bash
# Dar permissão ao script
chmod +x docker.sh

# Subir tudo (MySQL + Aplicação)
./docker.sh start

# Verificar se as tabelas foram criadas
./docker.sh tables
```

### Windows PowerShell
```powershell
# Subir tudo (MySQL + Aplicação)
.\docker.ps1 start

# Verificar se as tabelas foram criadas
.\docker.ps1 tables
```

### Manualmente (qualquer sistema)
```bash
# Subir containers
docker-compose up -d

# Verificar status
docker-compose ps

# Ver tabelas criadas
docker-compose exec mysql mysql -uroot -ppassword picpaysimplificado -e "SHOW TABLES;"
```

---

## Acessar a aplicação

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Métricas Prometheus**: http://localhost:5000/metrics
- **MySQL**: localhost:3306
  - Database: `picpaysimplificado`
  - User: `root`
  - Password: `password`

---

## Comandos úteis

### Usando scripts

**Linux/Mac/WSL:**
```bash
./docker.sh start      # Iniciar containers
./docker.sh status     # Ver status
./docker.sh tables     # Ver tabelas do banco
./docker.sh logs       # Ver logs em tempo real
./docker.sh stop       # Parar containers
./docker.sh restart    # Reiniciar containers
./docker.sh rebuild    # Reconstruir e reiniciar
./docker.sh clean      # Limpar tudo (CUIDADO!)
```

**Windows PowerShell:**
```powershell
.\docker.ps1 start      # Iniciar containers
.\docker.ps1 status     # Ver status
.\docker.ps1 tables     # Ver tabelas do banco
.\docker.ps1 logs       # Ver logs em tempo real
.\docker.ps1 stop       # Parar containers
.\docker.ps1 restart    # Reiniciar containers
.\docker.ps1 rebuild    # Reconstruir e reiniciar
.\docker.ps1 clean      # Limpar tudo (CUIDADO!)
```

### Comandos Docker diretos

```bash
# Ver logs
docker-compose logs -f              # Todos os serviços
docker-compose logs -f app          # Apenas aplicação
docker-compose logs -f mysql        # Apenas MySQL

# Acessar container
docker-compose exec app bash        # Shell na aplicação
docker-compose exec mysql bash      # Shell no MySQL

# Conectar ao MySQL
docker-compose exec mysql mysql -uroot -ppassword picpaysimplificado

# Ver dados das tabelas
docker-compose exec mysql mysql -uroot -ppassword picpaysimplificado -e "SELECT * FROM Wallets;"
docker-compose exec mysql mysql -uroot -ppassword picpaysimplificado -e "SELECT * FROM Transfers;"

# Parar tudo
docker-compose stop

# Parar e remover containers
docker-compose down

# Remover tudo incluindo volumes (dados do banco)
docker-compose down -v

# Reconstruir containers
docker-compose up -d --build
```

---

## Teste rápido da API

### 1. Criar duas carteiras

**Carteira 1 (Usuário):**
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

**Carteira 2 (Lojista):**
```bash
curl -X POST http://localhost:5000/api/carteiras \
  -H "Content-Type: application/json" \
  -d '{
    "nomeCompleto": "Loja ABC",
    "cpfcnpj": "12345678000199",
    "email": "loja@email.com",
    "senha": "senha123",
    "userType": 1,
    "saldo": 0
  }'
```

### 2. Listar carteiras

```bash
curl http://localhost:5000/api/carteiras
```

### 3. Fazer transferência

```bash
curl -X POST http://localhost:5000/transfer \
  -H "Content-Type: application/json" \
  -d '{
    "emetenteId": 1,
    "destinatarioId": 2,
    "valor": 100
  }'
```

### 4. Listar transferências

```bash
curl http://localhost:5000/transfer
```

### 5. Ver métricas

```bash
curl http://localhost:5000/metrics
```

---

## Troubleshooting

### Porta 3306 já está em uso

Se você já tem MySQL rodando localmente:

1. **Opção 1:** Parar o MySQL local
2. **Opção 2:** Mudar a porta no `docker-compose.yml`:
   ```yaml
   ports:
     - "3307:3306"  # Usa porta 3307 no host
   ```

### Porta 5000 já está em uso

Mude a porta no `docker-compose.yml`:
```yaml
ports:
  - "5001:8080"  # Usa porta 5001 no host
```

### Containers não sobem

```bash
# Ver o que está errado
docker-compose logs

# Reconstruir do zero
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

### Tabelas não foram criadas

```bash
# Verificar se o script init.sql foi executado
docker-compose logs mysql | grep init.sql

# Recriar o banco do zero
docker-compose down -v
docker-compose up -d

# Aguardar 10 segundos e verificar
sleep 10
docker-compose exec mysql mysql -uroot -ppassword picpaysimplificado -e "SHOW TABLES;"
```

### Aplicação não conecta no banco

```bash
# Verificar se MySQL está saudável
docker-compose ps

# Testar conexão manualmente
docker-compose exec mysql mysql -uroot -ppassword -e "SHOW DATABASES;"

# Ver logs da aplicação
docker-compose logs app
```

---

## Limpar tudo e começar do zero

**Linux/Mac/WSL:**
```bash
./docker.sh clean
./docker.sh start
```

**Windows PowerShell:**
```powershell
.\docker.ps1 clean
.\docker.ps1 start
```

**Manualmente:**
```bash
docker-compose down -v
docker-compose up -d
```

---

## Próximos passos

1. ? Acesse o Swagger: http://localhost:5000/swagger
2. ? Teste os endpoints direto pela interface
3. ? Veja as métricas: http://localhost:5000/metrics
4. ?? Configure Grafana para visualizar métricas (veja MONITORING_README.md)

---

**Precisa de ajuda?** Veja a documentação completa em [DOCKER.md](DOCKER.md)
