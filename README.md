# 🏖️ AdmBeachApp

📌 **Visão Geral**

O AdmBeachApp é um software proprietário desenvolvido para a gestão de quiosques de praia, facilitando o controle de vendas, estoque e operações do dia a dia. O aplicativo oferece autenticação segura via Google OAuth, armazenamento com PostgreSQL/Supabase, interface responsiva com .NET MAUI e Blazor, e uma gestão eficiente de produtos e transações.

✨ **Novidades:** Agora com **ambiente de desenvolvimento containerizado com Podman** para maior produtividade e consistência!

💡 **Gerente do Projeto:** Marcelo Oliveira Arrebola  
💻 **Desenvolvedor:** Julio Campos Machado – Like Look Solutions

## 🎯 Principais Funcionalidades

✔️ **Login Seguro e Rápido** – Autenticação via Google OAuth  
✔️ **Gestão de Vendas** – Registro de transações em tempo real  
✔️ **Controle de Estoque** – Monitoramento de produtos e níveis de reposição  
✔️ **Banco de Dados Integrado** – Armazenamento local com SQLite  
✔️ **Interface Responsiva** – Experiência fluida com .NET MAUI e Blazor  

## 🚀 Benefícios do AdmBeachApp

📊 Maior controle sobre as operações do quiosque  
🔒 Segurança no acesso e armazenamento de dados  
📱 Acesso fácil de qualquer dispositivo compatível  
📈 Eficiência na gestão de estoque e vendas  

## 🛠️ Tecnologias Utilizadas

### 🎯 Core Technologies
🔹 **.NET 9.0 MAUI** – Desenvolvimento multiplataforma  
🔹 **Blazor Hybrid** – Interface web moderna e dinâmica  
🔹 **C#** – Linguagem principal do projeto  
🔹 **Entity Framework Core** – ORM para acesso a dados  

### 🗄️ Banco de Dados
🔹 **SQLite** – Armazenamento local  
🔹 **PostgreSQL** – Banco principal (via Supabase)  
🔹 **Supabase** – Backend-as-a-Service  

### 🔐 Autenticação & APIs
🔹 **Google OAuth 2.0** – Autenticação segura  
🔹 **SignalR** – Comunicação em tempo real  
🔹 **ASP.NET Core** – APIs e serviços  

### 🎨 Interface & Design
🔹 **Bootstrap 5** – Framework CSS responsivo  
🔹 **Font Awesome** – Ícones modernos  
🔹 **CSS3 & HTML5** – Interface customizada  

### 🐳 DevOps & Containers
🔹 **Podman** – Containerização (alternativa ao Docker)  
🔹 **PostgreSQL Container** – Banco local para desenvolvimento  
🔹 **Redis Container** – Cache e sessões  
🔹 **MailHog** – Servidor de email para testes  
🔹 **pgAdmin** – Interface web para PostgreSQL

## 📁 Estrutura do Projeto

```
AdmBeachApp/
├── Components/              # Componentes Blazor
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   └── Pages/
│       ├── Home.razor           # Dashboard principal
│       ├── Produtos.razor       # Gestão de produtos
│       └── Vendas.razor         # Gestão de vendas
├── Data/                    # Contexto e configurações BD
│   └── BeachAppContext.cs
├── Models/                  # Modelos de domínio
│   ├── Produto.cs
│   └── Venda.cs
├── Services/                # Lógica de negócio
│   ├── ProdutoService.cs
│   └── VendaService.cs
├── scripts/                 # Scripts de desenvolvimento
│   ├── setup-podman.ps1     # Setup completo Podman
│   └── dev.ps1               # Comandos de desenvolvimento
├── wwwroot/                 # Assets web
│   ├── css/
│   │   └── app.css
│   └── index.html
├── Dockerfile               # Container para desenvolvimento
├── docker-compose.yml       # Orquestração de serviços
├── .containerignore         # Exclusões do container
├── PODMAN_SETUP.md          # Documentação completa Podman
└── supabase_*.sql           # Scripts SQL para desenvolvimento
```

## 🚀 Como Executar

### 📋 Pré-requisitos

#### Para Desenvolvimento
- **.NET 9.0 SDK** ou superior
- **Visual Studio 2022** (17.8+) ou **Visual Studio Code**
- **PowerShell 5.1+** (Windows)
- **WSL2** habilitado (para Podman)

#### Para Testes Mobile
- **Emulador Android/iOS** ou **dispositivo físico**

### 🐳 Opção 1: Desenvolvimento com Podman (Recomendado)

**Vantagens:** Banco PostgreSQL local, Redis, MailHog, pgAdmin e ambiente isolado.

1. **Clone o repositório**
   ```powershell
   git clone https://github.com/seu-usuario/AdmBeachApp.git
   cd AdmBeachApp
   ```

2. **Setup Podman (primeira vez apenas)**
   ```powershell
   # Execute como Administrador
   .\scripts\setup-podman.ps1 -Install -Init
   ```

3. **Iniciar ambiente de desenvolvimento**
   ```powershell
   # Inicia PostgreSQL, Redis, pgAdmin, MailHog
   .\scripts\dev.ps1 start
   ```

4. **Restaurar e executar o projeto**
   ```powershell
   dotnet restore
   dotnet run --framework net9.0-windows10.0.19041.0
   ```

5. **Acessar serviços**
   - **App MAUI**: Executando no Windows
   - **PostgreSQL**: `localhost:5432` (admbeach/dev123456)
   - **pgAdmin**: http://localhost:8080
   - **MailHog**: http://localhost:8025

### 🛠️ Opção 2: Desenvolvimento Tradicional

1. **Clone o repositório**
   ```bash
   git clone https://github.com/seu-usuario/AdmBeachApp.git
   cd AdmBeachApp
   ```

2. **Restaurar pacotes**
   ```bash
   dotnet restore
   ```

3. **Executar o projeto**
   ```bash
   dotnet run
   ```
   
   Ou no **Visual Studio**: `F5` (debug) ou `Ctrl+F5` (sem debug)

### 📈 Comandos Úteis para Desenvolvimento

```powershell
# Status do ambiente
.\scripts\dev.ps1 status

# Ver logs dos containers
.\scripts\dev.ps1 logs postgres

# Conectar no banco PostgreSQL
.\scripts\dev.ps1 db

# Parar ambiente
.\scripts\dev.ps1 stop

# Ajuda completa
.\scripts\setup-podman.ps1 -Help
```

### 📚 Documentação Detalhada

Para instruções completas sobre Podman, consulte: **[PODMAN_SETUP.md](./PODMAN_SETUP.md)**

## 📊 Funcionalidades Implementadas

### Dashboard
- ✅ Visão geral das vendas do dia
- ✅ Contadores de produtos ativos
- ✅ Alertas de estoque baixo
- ✅ Lista de vendas recentes

### Gestão de Produtos
- ✅ Cadastro, edição e exclusão de produtos
- ✅ Controle de estoque
- ✅ Categorização de produtos
- ✅ Filtros e busca
- ✅ Alertas de estoque baixo

### Gestão de Vendas
- ✅ Registro de vendas com múltiplos itens
- ✅ Diferentes métodos de pagamento
- ✅ Histórico de vendas
- ✅ Filtros por período
- ✅ Cálculo automático de totais
- ✅ Atualização automática de estoque

## 🔧 Funcionalidades Futuras

- 🔲 Autenticação Google OAuth
- 🔲 Relatórios avançados
- 🔲 Backup automático
- 🔲 Sincronização em nuvem
- 🔲 Modo offline
- 🔲 Impressão de cupons
- 🔲 Gestão de clientes
- 🔲 Controle de caixa

## 🎨 Interface

O AdmBeachApp possui uma interface moderna e intuitiva com:
- **Tema de praia** com cores oceânicas
- **Design responsivo** para dispositivos móveis e desktop
- **Ícones Bootstrap** para melhor experiência visual
- **Animações suaves** para transições
- **Cards informativos** para melhor organização

## 🗃️ Banco de Dados

### 📊 Esquema de Dados
O aplicativo suporta múltiplas opções de armazenamento:

#### **Estrutura Principal**
- 📦 **Produtos**: Nome, descrição, preço, estoque, categoria, ativo
- 📊 **Vendas**: Data, cliente, método de pagamento, total, status
- 📄 **Itens de Venda**: Quantidade, preço unitário, subtotal, produto_id
- 🗅 **Dados iniciais**: 5 produtos pré-cadastrados para demonstração

### 🎯 Opções de Armazenamento

#### 1. **SQLite** (Padrão - Local)
📌 Ideal para: Desenvolvimento local, testes, demos  
📁 Localização: `Data/beach_app.db`  
⚙️ Migration: Automática no primeiro uso  

#### 2. **PostgreSQL** (Produção via Supabase)
🌐 Ideal para: Ambiente de produção, sincronização  
🔗 Conexão: Via Supabase ou instância própria  
🔐 Segurança: Row Level Security (RLS), autenticação  

#### 3. **PostgreSQL Local** (Desenvolvimento com Podman)
🐳 Ideal para: Desenvolvimento avançado, testes de integração  
🏠 Host: `localhost:5432`  
🔑 Acesso: `admbeach` / `dev123456`  
🛠️ Ferramentas: pgAdmin em http://localhost:8080  

### 📑 Scripts de Inicialização
- `supabase_01_tabelas.sql` - Estrutura das tabelas
- `supabase_02_indices_dados.sql` - Índices e dados iniciais
- `supabase_03_seguranca.sql` - Políticas de segurança

## 📱 Plataformas Suportadas

- ✅ **Windows** (Desktop)
- ✅ **Android** (Mobile)
- ✅ **iOS** (Mobile)
- ✅ **macOS** (Desktop)

## 🔐 Segurança

- Validação de dados no frontend e backend
- Transações de banco de dados com rollback
- Soft delete para produtos (não remove fisicamente)
- Validações de negócio (estoque, valores, etc.)

## 🤝 Contribuição

Este é um projeto proprietário da Like Look Solutions. Para sugestões ou melhorias, entre em contato com a equipe de desenvolvimento.

## 📄 Licença

Todos os direitos reservados © 2024 Like Look Solutions  
Desenvolvido por Julio Campos Machado

---

**AdmBeachApp** - Transformando a gestão de quiosques de praia! 🏖️🌊

