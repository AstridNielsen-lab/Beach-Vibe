# Instruções de Execução - AdmBeachApp

## 🚀 Como Executar o AdmBeachApp

### Pré-requisitos

1. **.NET 8.0 SDK ou superior** instalado
   - Download: https://dotnet.microsoft.com/download
   
2. **Visual Studio 2022** (recomendado) ou **Visual Studio Code**
   - Visual Studio 2022: https://visualstudio.microsoft.com/
   - VS Code: https://code.visualstudio.com/

### Método 1: Executar via Visual Studio 2022

1. Abra o arquivo `AdmBeachApp.sln` ou `AdmBeachApp.csproj` no Visual Studio 2022
2. Certifique-se de que o projeto está configurado para **Windows**
3. Pressione `F5` para executar em modo debug ou `Ctrl+F5` para executar sem debug
4. O aplicativo será compilado e executado automaticamente

### Método 2: Executar via Linha de Comando

1. Abra o Terminal/PowerShell na pasta do projeto
2. Execute o comando:
   ```bash
   dotnet run --framework net9.0-windows10.0.19041.0
   ```
3. O aplicativo será compilado e executado

### Método 3: Compilar e Executar o Executável

1. Para compilar o projeto:
   ```bash
   dotnet build --framework net9.0-windows10.0.19041.0
   ```

2. Para executar o aplicativo compilado:
   ```bash
   dotnet run --framework net9.0-windows10.0.19041.0
   ```

## 🎯 Primeiro Uso

### Dados Iniciais
O aplicativo já vem com **5 produtos pré-cadastrados**:
- Água Mineral 500ml
- Refrigerante Lata 350ml  
- Sanduíche Natural
- Protetor Solar FPS 60
- Cerveja Long Neck

### Funcionalidades Disponíveis

1. **Dashboard** (`/`)
   - Visão geral das vendas do dia
   - Estatísticas de produtos
   - Alertas de estoque baixo
   - Vendas recentes

2. **Gestão de Produtos** (`/produtos`)
   - Listar todos os produtos
   - Adicionar novos produtos
   - Editar produtos existentes
   - Excluir produtos (soft delete)
   - Filtrar por categoria ou estoque baixo
   - Buscar produtos por nome

3. **Gestão de Vendas** (`/vendas`)
   - Registrar novas vendas
   - Adicionar múltiplos produtos por venda
   - Diferentes métodos de pagamento
   - Histórico de vendas
   - Filtrar vendas por período
   - Ver detalhes de vendas

## 🗃️ Banco de Dados

O aplicativo utiliza **SQLite** como banco de dados local:
- Arquivo: `beachapp.db`
- Localização: `%LOCALAPPDATA%\AdmBeachApp\beachapp.db`
- Criação automática na primeira execução

## 🛠️ Resolução de Problemas

### Erro: "Android SDK directory could not be found"
**Solução**: Execute apenas para Windows:
```bash
dotnet run --framework net9.0-windows10.0.19041.0
```

### Erro: "No runtime pack available"
**Solução**: Certifique-se de ter o .NET 8.0 ou superior instalado

### Erro de Compilação
**Solução**: 
1. Limpe e recompile o projeto:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

### Banco de Dados não Criado
**Solução**: O banco é criado automaticamente na primeira execução. Se houver problemas:
1. Verifique permissões de escrita na pasta `%LOCALAPPDATA%`
2. Execute o aplicativo como administrador

## 🔧 Configurações Avançadas

### Executar em Diferentes Plataformas

- **Windows**: `dotnet run --framework net9.0-windows10.0.19041.0`
- **Android** (requer Android SDK): `dotnet run --framework net9.0-android`
- **iOS** (requer macOS): `dotnet run --framework net9.0-ios`

### Modo de Produção

Para compilar em modo Release:
```bash
dotnet build --configuration Release --framework net9.0-windows10.0.19041.0
```

## 📞 Suporte

Para suporte técnico ou dúvidas:
- **Desenvolvedor**: Julio Campos Machado
- **Empresa**: Like Look Solutions
- **Gerente do Projeto**: Marcelo Oliveira Arrebola

---

**Nota**: Este é um software proprietário. Todos os direitos reservados © 2024 Like Look Solutions.

