# 🚀 AdmBeachApp - Setup com Podman

Este documento descreve como configurar e usar **Podman** no Windows para desenvolvimento do AdmBeachApp, um aplicativo .NET MAUI Blazor Hybrid.

## 📖 Índice

- [Por que Podman?](#-por-que-podman)
- [Pré-requisitos](#-pré-requisitos)  
- [Instalação](#-instalação)
- [Primeira Configuração](#-primeira-configuração)
- [Uso Diário](#-uso-diário)
- [Serviços Disponíveis](#-serviços-disponíveis)
- [Comandos Úteis](#-comandos-úteis)
- [Troubleshooting](#-troubleshooting)

## 🤔 Por que Podman?

**Podman** é uma alternativa ao Docker que oferece:
- ✅ **Rootless containers** - mais seguro
- ✅ **Compatible com Docker** - mesmo comandos e Dockerfile
- ✅ **Não requer daemon** - menos overhead
- ✅ **Suporte nativo no Windows** via WSL2

Para este projeto MAUI, usamos Podman para:
- 🗄️ **Banco PostgreSQL local** (substitui/complementa Supabase)
- 🔴 **Redis** para cache
- 📧 **MailHog** para testes de email  
- 🔧 **pgAdmin** interface web para banco
- 🏗️ **Ambiente de build consistente**

## 📋 Pré-requisitos

### Windows 10/11
- **PowerShell 5.1+** (já incluso no Windows)
- **WSL2** habilitado
- **4GB+ RAM** disponível
- **10GB+ espaço** em disco

### Verificar WSL2
```powershell
wsl --list --verbose
```
Se não tiver WSL2 instalado:
```powershell
wsl --install
```

## 🔧 Instalação

### Opção 1: Instalação Automática (Recomendada)

Execute como **Administrador**:
```powershell
.\scripts\setup-podman.ps1 -Install -Init
```

### Opção 2: Instalação Manual

1. **Instalar Podman Desktop**:
   - Baixe de: https://podman.io/getting-started/installation
   - Ou via Chocolatey: `choco install podman-desktop`

2. **Inicializar Podman**:
   ```powershell
   podman machine init --cpus=4 --memory=8192 --disk-size=50
   podman machine start
   ```

3. **Instalar podman-compose** (opcional):
   ```powershell
   pip3 install podman-compose
   ```

## 🚀 Primeira Configuração

Após instalar, configure o ambiente:

```powershell
# 1. Verificar instalação
podman --version
podman machine list

# 2. Inicializar ambiente do projeto  
.\scripts\setup-podman.ps1 -Init

# 3. Fazer primeiro build das imagens
podman compose build

# 4. Iniciar serviços essenciais
.\scripts\dev.ps1 start
```

## 🎯 Uso Diário

### Script Simplificado (`dev.ps1`)

Para desenvolvimento diário, use o script simplificado:

```powershell
# Iniciar ambiente completo
.\scripts\dev.ps1 start

# Parar ambiente  
.\scripts\dev.ps1 stop

# Ver status
.\scripts\dev.ps1 status

# Ver logs
.\scripts\dev.ps1 logs
.\scripts\dev.ps1 logs postgres

# Abrir shell no container
.\scripts\dev.ps1 shell

# Conectar no PostgreSQL
.\scripts\dev.ps1 db
```

### Script Completo (`setup-podman.ps1`)

Para tarefas administrativas:

```powershell
# Instalar Podman (como admin)
.\scripts\setup-podman.ps1 -Install

# Inicializar máquina Podman  
.\scripts\setup-podman.ps1 -Init

# Iniciar serviços
.\scripts\setup-podman.ps1 -Start

# Limpeza completa (cuidado!)
.\scripts\setup-podman.ps1 -Clean
```

## 🌐 Serviços Disponíveis

Após executar `.\scripts\dev.ps1 start`, você terá acesso a:

| Serviço | URL/Endpoint | Credenciais |
|---------|-------------|-------------|
| **PostgreSQL** | `localhost:5432` | `admbeach` / `dev123456` |
| **pgAdmin** | http://localhost:8080 | `dev@admbeachapp.com` / `dev123456` |
| **Redis** | `localhost:6379` | sem senha |
| **MailHog SMTP** | `localhost:1025` | sem auth |
| **MailHog Web** | http://localhost:8025 | sem auth |

### Conectando no PostgreSQL

#### Via pgAdmin (Recomendado)
1. Acesse: http://localhost:8080
2. Login: `dev@admbeachapp.com` / `dev123456`
3. Adicionar servidor:
   - **Nome**: AdmBeachApp Local
   - **Host**: `postgres` (nome do container)
   - **Port**: `5432`
   - **Database**: `admbeachapp`
   - **Username**: `admbeach`
   - **Password**: `dev123456`

#### Via Comando
```powershell
# Conectar diretamente
.\scripts\dev.ps1 db

# Ou manualmente
podman exec -it admbeachapp-postgres psql -U admbeach -d admbeachapp
```

#### Via Aplicação .NET
```csharp
// Connection string para desenvolvimento local
"Host=localhost;Port=5432;Database=admbeachapp;Username=admbeach;Password=dev123456"
```

## 📜 Comandos Úteis

### Comandos do Podman

```powershell
# Ver containers rodando
podman ps

# Ver todos os containers  
podman ps -a

# Ver logs de um container
podman logs admbeachapp-postgres

# Executar comando em container
podman exec -it admbeachapp-postgres bash

# Ver imagens
podman images

# Ver volumes
podman volume ls
```

### Comandos Docker Compose

```powershell
# Iniciar serviços
podman compose up -d

# Iniciar serviços específicos
podman compose up -d postgres redis

# Ver logs
podman compose logs -f postgres

# Parar serviços
podman compose down

# Rebuild imagens
podman compose build --no-cache
```

### Comandos de Limpeza

```powershell
# Remover containers parados
podman container prune

# Remover imagens não utilizadas  
podman image prune

# Remover volumes não utilizados
podman volume prune

# Limpeza completa (CUIDADO!)
podman system prune -a --volumes
```

## 🔧 Desenvolvimento

### Executando o App MAUI

O Podman **NÃO** substitui a execução do app MAUI (que precisa rodar nativamente no Windows). Use Podman para:

1. **Serviços de apoio** (banco, cache, etc.)
2. **Ambiente de build** consistente
3. **Testes automatizados**

Execute o MAUI normalmente:
```powershell
# Com os serviços Podman rodando
.\scripts\dev.ps1 start

# Execute o MAUI no Visual Studio ou via CLI
dotnet run --framework net9.0-windows10.0.19041.0
```

### Configuração do Banco

Os scripts SQL em `supabase_*.sql` são automaticamente executados quando o PostgreSQL inicializa pela primeira vez.

Para reinicializar o banco:
```powershell
# Para containers
.\scripts\dev.ps1 stop

# Remove volume do PostgreSQL (PERDERÁ DADOS!)
podman volume rm admbeachapp_postgres-data

# Reinicia (vai recriar banco)
.\scripts\dev.ps1 start
```

## 🐛 Troubleshooting

### Problema: "podman command not found"
```powershell
# Verifique se está no PATH
$env:PATH -split ';' | Where-Object { $_ -like "*podman*" }

# Reinicie o terminal
# Ou adicione manualmente ao PATH
```

### Problema: "machine not running"  
```powershell
# Verifique status da máquina
podman machine list

# Inicie se necessário
podman machine start
```

### Problema: "port already in use"
```powershell
# Verifique quais portas estão em uso
netstat -ano | findstr :5432

# Pare o serviço conflitante ou mude a porta no docker-compose.yml
```

### Problema: Performance lenta
```powershell
# Aumente recursos da máquina Podman
podman machine stop
podman machine rm
podman machine init --cpus=6 --memory=12288 --disk-size=100
podman machine start
```

### Problema: "no space left on device"
```powershell
# Limpe imagens e containers não utilizados
podman system prune -a

# Ou aumente disk-size da máquina
```

### Problema: Container não conecta na rede
```powershell
# Reset da rede
podman network prune
.\scripts\dev.ps1 restart
```

## 📚 Recursos Adicionais

- **Documentação Podman**: https://podman.io/
- **Podman Desktop**: https://podman-desktop.io/
- **Migração Docker → Podman**: https://podman.io/getting-started/migration

## 🔄 Migração do Docker

Se já usa Docker, pode migrar facilmente:

1. **Alias** (opcional):
   ```powershell
   # No PowerShell Profile
   Set-Alias docker podman
   Set-Alias docker-compose "podman compose"
   ```

2. **Importar imagens Docker**:
   ```powershell
   # Salvar do Docker
   docker save myimage:latest > myimage.tar
   
   # Carregar no Podman  
   podman load < myimage.tar
   ```

3. **Usar mesmos comandos**: A maioria dos comandos são idênticos!

## 🆘 Suporte

Para problemas específicos do projeto:
1. Verifique este README
2. Execute `.\scripts\dev.ps1 status` 
3. Verifique logs com `.\scripts\dev.ps1 logs`
4. Consulte documentação oficial do Podman

---

**✨ Bom desenvolvimento!** 🚀
