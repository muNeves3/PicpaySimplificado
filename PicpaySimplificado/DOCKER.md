# PicpaySimplificado - Docker

## Arquivos criados

- `Dockerfile` - Imagem Docker para a aplicação .NET 9
- `docker-compose.yml` - Orquestração dos serviços (MySQL e aplicação)
- `init.sql` - Script SQL para criar tabelas, índices e constraints automaticamente

## Como executar

### 1. Subir todos os serviços (MySQL + Aplicação)

```bash
docker-compose up -d
```

Este comando irá:
- Criar e iniciar o container MySQL na porta 3306
- Executar automaticamente o script `init.sql` para criar as tabelas (Wallets e Transfers)
- Aguardar o MySQL estar saudável (healthcheck)
- Compilar e executar a aplicação na porta 5000

**Importante**: As tabelas são criadas automaticamente pelo script `init.sql`. Não é necessário rodar migrations manualmente!

### 2. Verificar status dos containers

```bash
docker-compose ps
```

### 3. Ver logs

```bash
# Logs de todos os serviços
docker-compose logs -f

# Logs apenas do MySQL
docker-compose logs -f mysql

# Logs apenas da aplicação
docker-compose logs -f app
```

### 4. Acessar a aplicação

- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Métricas Prometheus: http://localhost:5000/metrics

### 5. Verificar se as tabelas foram criadas

```bash
# Conectar ao MySQL e verificar tabelas
docker-compose exec mysql mysql -uroot -ppassword picpaysimplificado -e "SHOW TABLES;"

# Ver estrutura da tabela Wallets
docker-compose exec mysql mysql -uroot -ppassword picpaysimplificado -e "DESCRIBE Wallets;"

# Ver estrutura da tabela Transfers
docker-compose exec mysql mysql -uroot -ppassword picpaysimplificado -e "DESCRIBE Transfers;"
```

### 6. Parar os serviços

```bash
# Parar sem remover
docker-compose stop

# Parar e remover containers
docker-compose down

# Parar, remover containers e volumes (CUIDADO: remove dados do banco)
docker-compose down -v
```

### 7. Rebuildar a aplicação após mudanças no código

```bash
docker-compose up -d --build
```

### 8. Recriar banco de dados do zero

Se você modificar o `init.sql` e quiser recriar o banco:

```bash
# Parar e remover tudo incluindo volumes
docker-compose down -v

# Subir novamente (vai executar o init.sql atualizado)
docker-compose up -d
```

## Estrutura da rede

- **mysql**: Container do MySQL 8.0
  - Porta: 3306
  - Database: picpaysimplificado
  - User: root
  - Password: password
  - Script de inicialização: `init.sql` (executado automaticamente)

- **app**: Container da aplicação
  - Porta: 5000 (host) -> 8080 (container)
  - Connection String configurada para conectar no MySQL

## Estrutura do Banco de Dados

O script `init.sql` cria:

### Tabela Wallets
- `Id` (int, auto-increment, PK)
- `NomeCompleto` (longtext)
- `CPFCNPJ` (varchar(255))
- `Email` (varchar(255))
- `Senha` (longtext)
- `SaldoConta` (decimal(18,2))
- `UserType` (longtext)
- Índice único: `CPFCNPJ` + `Email`

### Tabela Transfers
- `IdTransferencia` (char(36), PK)
- `SenderId` (int, FK -> Wallets.Id)
- `ReciverId` (int, FK -> Wallets.Id)
- `Valor` (decimal(18,2))
- Índices: `SenderId`, `ReciverId`
- Constraints: `FK_Transaction_Sender`, `FK_Transaction_Reciver`

## Troubleshooting

### Erro de conexão com banco de dados

Se a aplicação não conseguir conectar ao MySQL, verifique:

```bash
# Verificar se o MySQL está healthy
docker-compose ps

# Testar conexão manualmente
docker-compose exec mysql mysql -uroot -ppassword -e "SHOW DATABASES;"
```

### Verificar se as tabelas foram criadas corretamente

```bash
docker-compose exec mysql mysql -uroot -ppassword picpaysimplificado -e "SELECT COUNT(*) as total_wallets FROM Wallets; SELECT COUNT(*) as total_transfers FROM Transfers;"
```

### Reconstruir tudo do zero

```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

### As tabelas não foram criadas

O script `init.sql` só é executado quando o volume do MySQL é criado pela primeira vez. Se você modificar o script e as tabelas não mudarem:

```bash
# Remover volumes e recriar
docker-compose down -v
docker-compose up -d
