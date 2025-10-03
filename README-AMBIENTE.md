# 🏖️ AdmBeachApp - Ambiente de Desenvolvimento

Este projeto está configurado para usar **Podman** no Windows, oferecendo um ambiente completo de desenvolvimento com PostgreSQL, Redis, pgAdmin e MailHog.

## 🚀 Quick Start

### 1. Iniciar o ambiente
```powershell
.\admbeach-simple.ps1 start
```

### 2. Ver status
```powershell  
.\admbeach-simple.ps1 status
```

### 3. Acessar os serviços
- **pgAdmin**: http://localhost:8080 (dev@admbeachapp.com / dev123456)
- **MailHog**: http://localhost:8025
- **PostgreSQL**: localhost:5432 (admbeach / dev123456)
- **Redis**: localhost:6379

## 📋 Comandos Disponíveis

| Comando | Descrição |
|---------|-----------|
| `start` | Inicia todos os serviços |
| `stop` | Para todos os serviços |
| `restart` | Reinicia o ambiente |
| `status` | Mostra status dos containers |
| `logs` | Exibe logs dos serviços |
| `shell` | Conecta ao PostgreSQL |
| `open` | Abre pgAdmin e MailHog no browser |
| `clean` | Remove containers parados |

### Exemplos de uso:
```powershell
# Iniciar ambiente completo
.\admbeach-simple.ps1 start

# Ver logs dos serviços
.\admbeach-simple.ps1 logs

# Conectar ao banco PostgreSQL
.\admbeach-simple.ps1 shell

# Abrir interfaces web
.\admbeach-simple.ps1 open

# Ver status detalhado
.\admbeach-simple.ps1 status
```

## 🗄️ Banco de Dados

### Conexão PostgreSQL
- **Host**: localhost
- **Porta**: 5432
- **Database**: admbeachapp
- **Usuário**: admbeach
- **Senha**: dev123456

### Tabelas criadas automaticamente
O banco já vem com as seguintes tabelas:
- `produtos`
- `pedidos` 
- `itens_pedido`

### Conectar via linha de comando:
```powershell
# Via script (recomendado)
.\admbeach-simple.ps1 shell

# Via psql direto
podman exec -it admbeachapp-postgres psql -U admbeach -d admbeachapp
```

## 🌐 Interfaces Web

### pgAdmin
- **URL**: http://localhost:8080
- **Email**: dev@admbeachapp.com  
- **Senha**: dev123456

Para conectar ao PostgreSQL no pgAdmin:
- **Host**: admbeachapp-postgres (nome do container)
- **Porta**: 5432
- **Database**: admbeachapp
- **Username**: admbeach
- **Password**: dev123456

### MailHog (Servidor de Email de Desenvolvimento)
- **Interface Web**: http://localhost:8025
- **SMTP**: localhost:1025

## 📦 Estrutura dos Containers

```yaml
Services:
  postgres:     # PostgreSQL 16 + dados iniciais
  redis:        # Redis 7 para cache
  pgadmin:      # Interface web para PostgreSQL
  mailhog:      # Servidor de email para testes
```

## 🔧 Configuração Avançada

### Scripts disponíveis:
- `admbeach-simple.ps1` - Script principal (recomendado)
- `scripts/setup-podman.ps1` - Setup completo do Podman
- `scripts/setup-podman-simple.ps1` - Setup simplificado
- `scripts/dev.ps1` - Comandos de desenvolvimento
- `docker-compose.yml` - Configuração dos serviços

### Volumes persistentes:
- `admbeachapp_postgres-data` - Dados do PostgreSQL
- `admbeachapp_redis-data` - Dados do Redis  
- `admbeachapp_pgadmin-data` - Configurações do pgAdmin

### Comandos Podman diretos:
```powershell
# Listar containers
podman ps

# Logs específicos
podman compose logs postgres
podman compose logs redis

# Parar tudo
podman compose down

# Subir serviços específicos
podman compose up -d postgres redis
```

## 🔨 Desenvolvimento da Aplicação MAUI

### Requisitos:
- .NET 9.0 SDK
- Workload MAUI instalado

### Compilar o projeto:
```powershell
# Restaurar pacotes
dotnet restore

# Compilar
dotnet build

# Executar (se aplicável)
dotnet run
```

### Configuração da string de conexão:
```csharp
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=admbeachapp;Username=admbeach;Password=dev123456"
}
```

## 🧹 Manutenção

### Limpar ambiente:
```powershell
# Remove containers parados
.\admbeach-simple.ps1 clean

# Reset completo (CUIDADO: remove todos os dados!)
podman compose down -v
podman system prune -a -f
```

### Backup do banco:
```powershell
# Backup
podman exec admbeachapp-postgres pg_dump -U admbeach admbeachapp > backup.sql

# Restore
podman exec -i admbeachapp-postgres psql -U admbeach -d admbeachapp < backup.sql
```

## 🚨 Troubleshooting

### Problemas comuns:

**1. "Podman machine not running"**
```powershell
podman machine start
```

**2. "Port already in use"**
```powershell
# Verificar processos usando as portas
netstat -ano | findstr ":5432"
netstat -ano | findstr ":8080"
```

**3. "Container won't start"**
```powershell
# Ver logs detalhados
podman compose logs [service-name]
```

**4. "Reset complete environment"**
```powershell
.\admbeach-simple.ps1 stop
podman system prune -a -f
.\admbeach-simple.ps1 start
```

### Logs úteis:
```powershell
# Logs de todos os serviços
.\admbeach-simple.ps1 logs

# Logs específicos
podman compose logs -f postgres
podman compose logs -f pgadmin
```

## ⚙️ Configurações Personalizadas

### Alterar portas:
Edite o `docker-compose.yml` e altere as seções `ports`:

```yaml
postgres:
  ports:
    - "5433:5432"  # Muda porta local para 5433
```

### Alterar senhas:
Modifique as variáveis de ambiente no `docker-compose.yml`:

```yaml
environment:
  POSTGRES_PASSWORD: nova_senha
```

## 📱 Integração com VS Code

### Extensões recomendadas:
- C# Dev Kit
- .NET MAUI
- PostgreSQL
- Docker

### settings.json sugerido:
```json
{
    "dotnet.completion.showCompletionItemsFromUnimportedNamespaces": true,
    "omnisharp.enableEditorConfigSupport": true
}
```

---

## 🆘 Suporte

Para dúvidas ou problemas:

1. Verifique o status: `.\admbeach-simple.ps1 status`
2. Consulte os logs: `.\admbeach-simple.ps1 logs`
3. Teste a conectividade: `.\admbeach-simple.ps1 shell`

**Arquivos importantes:**
- `docker-compose.yml` - Configuração dos serviços
- `admbeach-simple.ps1` - Script principal de gerenciamento
- `supabase_*.sql` - Scripts de inicialização do banco
- `Dockerfile` - Imagem personalizada (se necessário)

---
*Ambiente configurado com Podman 5.6.0 + PostgreSQL 16 + Redis 7 + .NET 9.0*
