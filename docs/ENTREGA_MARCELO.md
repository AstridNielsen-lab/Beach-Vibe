# AdmBeachApp – Documento de Entrega (Marcelo)

Versão: 1.0  
Data: 2025-10-02  
Plataforma alvo: Windows + Podman Desktop (WSL backend)

---

## 1) Visão Geral

O AdmBeachApp é um ambiente e aplicativo de administração da operação Beach Vibe. O repositório está preparado para desenvolvimento local com containers (PostgreSQL, Redis, pgAdmin e MailHog), facilitando a prototipação, testes e a integração do app (cliente .NET/MAUI ou serviços auxiliares) com a infraestrutura local.

Principais objetivos do setup:
- Provisionar banco de dados PostgreSQL com tabelas e dados iniciais via scripts SQL.
- Disponibilizar Redis para cache/filas/sessões.
- Fornecer pgAdmin para administração do banco.
- Fornecer MailHog para testes de e-mail sem saída externa.
- Scripts PowerShell para iniciar/parar/inspecionar o ambiente com um comando.

Relevante para Marcelo: o ambiente foi padronizado para iniciar em minutos no Windows, sem pré-requisitos além do Podman Desktop. A documentação abaixo detalha como executar, testar e como evoluir.

---

## 2) Arquitetura de Alto Nível

- Cliente/App: .NET (MAUI) – build local fora dos containers (Dockerfile disponível para cenários de CI/ambiente dev containerizado).
- Banco de dados: PostgreSQL 16 (container) com volume persistente.
- Cache: Redis 7 (container) com volume persistente.
- Administração do banco: pgAdmin 4 (container), com credenciais padrão de desenvolvimento.
- E-mail de desenvolvimento: MailHog (container) – UI web e servidor SMTP local.
- Orquestração local: Podman Compose usando o arquivo `docker-compose.yml`.
- Rede: bridge local criada pelo compose, isolando serviços.

Portas padrão expostas na máquina local:
- 5432/tcp – PostgreSQL
- 6379/tcp – Redis
- 8080/tcp – pgAdmin (HTTP)
- 8025/tcp – MailHog (HTTP)
- 1025/tcp – MailHog (SMTP)

Volumes persistentes:
- `admbeachapp_postgres-data`
- `admbeachapp_redis-data`
- `admbeachapp_pgadmin-data`

---

## 3) Estrutura do Repositório (trechos relevantes)

- `docker-compose.yml` – definição dos serviços.
- `Dockerfile` – imagem .NET para cenários de desenvolvimento/CI.
- `supabase_*.sql` – scripts de criação/seed do banco.
- `scripts/` – scripts auxiliares:
  - `dev.ps1` – utilidades para dia a dia.
  - `setup-podman.ps1` e `setup-podman-simple.ps1` – preparação do Podman.
- `admbeach-simple.ps1` – script principal de gerenciamento (criado nesta entrega).
- `README-AMBIENTE.md` – guia rápido (criado nesta entrega).

---

## 4) Pré-requisitos

- Windows 10/11 com PowerShell 5.1+ (ou PowerShell 7).
- Podman Desktop 1.21+ (backend WSL ativo).  
  Observação: Confirmado Podman 5.6.0 neste ambiente.

---

## 5) Como Executar (CLI)

1. Clonar o repositório e abrir um PowerShell dentro da pasta raiz do projeto.
2. Iniciar o ambiente:

```powershell
# path=null start=null
.\admbeach-simple.ps1 start
```

3. Verificar status:

```powershell
# path=null start=null
.\admbeach-simple.ps1 status
```

4. Abrir interfaces web (pgAdmin/MailHog):

```powershell
# path=null start=null
.\admbeach-simple.ps1 open
```

5. Parar o ambiente ao finalizar o dia:

```powershell
# path=null start=null
.\admbeach-simple.ps1 stop
```

Credenciais/Conexões padrão:
- PostgreSQL: host `localhost` porta `5432`, database `admbeachapp`, user `admbeach`, senha `dev123456`.
- pgAdmin: http://localhost:8080 (email `dev@admbeachapp.com`, senha `dev123456`).
- MailHog: http://localhost:8025 (SMTP em `localhost:1025`).

---

## 6) Banco de Dados

Scripts executados automaticamente no primeiro start do container PostgreSQL:
- `supabase_01_tabelas.sql`
- `supabase_02_indices_dados.sql`
- `supabase_03_seguranca.sql`
- `supabase_tables.sql`

Tabelas principais já existentes (confirmado):
- `produtos`
- `pedidos`
- `itens_pedido`

Testes rápidos de conectividade no terminal:

```powershell
# path=null start=null
podman exec -it admbeachapp-postgres psql -U admbeach -d admbeachapp -c "SELECT version();"

podman exec -it admbeachapp-postgres psql -U admbeach -d admbeachapp -c "\\dt"
```

Exemplo: inserir e consultar um produto (ambiente de DEV):

```powershell
# path=null start=null
podman exec -it admbeachapp-postgres psql -U admbeach -d admbeachapp -c "INSERT INTO produtos (id, nome, preco) VALUES (gen_random_uuid(), 'Agua de coco', 12.50);"

podman exec -it admbeachapp-postgres psql -U admbeach -d admbeachapp -c "SELECT nome, preco FROM produtos ORDER BY nome;"
```

---

## 7) Como Testar

### 7.1 Testes Manuais (Checklist)

- Banco de Dados:
  - Conecte com o `psql` (ou pgAdmin) e liste tabelas (`\\dt`).
  - Insira um registro em `produtos` e confirme a leitura.
- Redis:
  - Verifique o status do container: `.\admbeach-simple.ps1 status` (deve aparecer HEALTHY).
- E-mail (MailHog):
  - Configure a aplicação para enviar SMTP para `localhost:1025`.
  - Envie um e-mail de teste e verifique a UI do MailHog (`http://localhost:8025`).
- App Cliente (.NET/MAUI ou Console/API):
  - Utilize a connection string: `Host=localhost;Database=admbeachapp;Username=admbeach;Password=dev123456`.
  - Execute um fluxo CRUD básico (ex.: criar produto, criar pedido com itens, consultar).

### 7.2 Testes Automatizados (sugestão)

Caso deseje criar uma suíte de testes .NET:

```powershell
# path=null start=null
# criar projeto de testes (ex.: xUnit)
dotnet new xunit -n AdmBeachApp.Tests
cd AdmBeachApp.Tests

# adicionar referência ao projeto principal (ajuste o caminho)
dotnet add reference ..\AdmBeachApp.csproj

# adicionar pacotes úteis
dotnet add package FluentAssertions

dotnet test
```

Para testes de integração com banco, rodar os containers e usar a connection string de testes. O `docker-compose.yml` já possui um profile `testing` (postgres-test e maui-test) que pode ser habilitado conforme necessidade.

Exemplo de execução do profile de testes (quando aplicável):

```powershell
# path=null start=null
podman compose --profile testing up --build --abort-on-container-exit
```

### 7.3 Testes de Saúde (Healthchecks)

- PostgreSQL: healthcheck já configurado com `pg_isready`.
- Redis: healthcheck `redis-cli ping`.
- Sugestão: expor endpoint de health na aplicação e adicioná-lo no compose.

---

## 8) Operação do Dia a Dia (Comandos Úteis)

```powershell
# path=null start=null
# Subir serviços essenciais
.\admbeach-simple.ps1 start

# Status resumido
.\admbeach-simple.ps1 status

# Logs gerais
.\admbeach-simple.ps1 logs

# Conectar ao PostgreSQL
.\admbeach-simple.ps1 shell

# Parar tudo
.\admbeach-simple.ps1 stop

# Limpar containers parados
.\admbeach-simple.ps1 clean
```

---

## 9) Troubleshooting

- Portas em uso:
  - `5432` (Postgres), `8080` (pgAdmin), `6379` (Redis), `8025`/`1025` (MailHog).
  - Verifique com:

```powershell
# path=null start=null
netstat -ano | findstr ":5432"
```

- Máquina Podman não iniciada:

```powershell
# path=null start=null
podman machine start
```

- Reset de ambiente (atenção: remove volumes e dados):

```powershell
# path=null start=null
podman compose down -v
podman system prune -a -f
```

- Logs específicos:

```powershell
# path=null start=null
podman compose logs postgres
podman compose logs redis
podman compose logs pgadmin
```

---

## 10) Plano de Melhorias (Roadmap Técnico)

1) Banco de Dados e Migrations
- Padronizar migrations (EF Core ou Flyway) em vez de múltiplos scripts soltos.
- Adicionar seeds consistentes por ambiente (Development/Test).
- Controlar versionamento do schema com histórico.

2) Configuração e Segurança
- Mover senhas/ports para `.env` e criar `.env.example`.
- Considerar `secrets`/`configs` (Podman/Docker) para credenciais.
- Revisar exposição de portas; restringir quando possível (ex.: bind 127.0.0.1).

3) Observabilidade
- Padronizar logging estruturado (Serilog/MEL).
- Avaliar métricas/health endpoints (Prometheus/OpenTelemetry).

4) Testes e Qualidade
- Adicionar testes unitários e de integração com banco efêmero (profile `testing`).
- Criar dados de teste determinísticos.

5) CI/CD
- Pipeline (GitHub Actions/Azure DevOps): restore, build, test, lint, publish.
- Job opcional para rodar containers e executar testes de integração.

6) DevX
- Task runner (Makefile/Taskfile/psake) para padronizar comandos.
- Dev container (VS Code) com Podman remoto/local.

7) Banco de Dados
- Índices adicionais conforme consultas reais (EXPLAIN/ANALYZE).
- Políticas de retenção/backup (scripts já esboçados em README-AMBIENTE).

8) Contêineres
- Salvar configuração de servidores do pgAdmin via `servers.json` montado em volume.
- Adicionar healthcheck para MailHog.
- Adicionar profile `proxy` (nginx) somente quando necessário.

---

## 11) Critérios de Aceite (Entrega)

- [x] Ambiente local sobe com um comando (`.\admbeach-simple.ps1 start`).
- [x] Serviços ficam acessíveis nas portas documentadas.
- [x] Banco de dados inicializado com tabelas confirmadas (`produtos`, `pedidos`, `itens_pedido`).
- [x] Scripts de uso e guia rápido adicionados (`admbeach-simple.ps1`, `README-AMBIENTE.md`).
- [x] Documentação de entrega criada (este arquivo).

---

## 12) Anexos e Referências

Arquivos principais desta entrega:
- `admbeach-simple.ps1` – gerenciamento do ambiente.
- `README-AMBIENTE.md` – guia rápido com comandos.
- `docker-compose.yml` – definição dos serviços.
- `supabase_*.sql` – scripts do banco.
- `Dockerfile` – imagem .NET para build/CI.

Para qualquer dúvida operacional:
1) ` .\admbeach-simple.ps1 status`  
2) ` .\admbeach-simple.ps1 logs`  
3) ` .\admbeach-simple.ps1 shell`

— Fim —

