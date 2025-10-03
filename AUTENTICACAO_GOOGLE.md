# Sistema de Autenticação Segmentada - AdmBeachApp

## Visão Geral

O sistema foi modificado para implementar autenticação segmentada, onde:

- **Área do Cliente** (`/cliente`) - Requer autenticação com Google
- **Áreas Operacionais** (`/bar`, `/cozinha`, `/pedidos`, etc.) - Funcionam SEM autenticação do Google

## Estrutura Implementada

### 1. Serviços de Autenticação

#### GoogleClientAuthService
- **Arquivo**: `Services/GoogleClientAuthService.cs`
- **Interface**: `IGoogleClientAuthService`
- **Função**: Gerencia autenticação específica para clientes usando Google OAuth
- **Registrado em**: `MauiProgram.cs` como `Scoped`

#### AuthService (Existente)
- **Função**: Sistema de auth existente para funcionários/administração
- **Usado em**: Páginas administrativas que precisam de autenticação Supabase

### 2. Componentes de Proteção

#### ClientAuthGuard
- **Arquivo**: `Components/Auth/ClientAuthGuard.razor`
- **Função**: Protege páginas da área do cliente, exigindo login Google
- **Características**:
  - Interface amigável de login
  - Opção de fallback para login simples
  - Gerenciamento de estado de autenticação
  - Design responsivo

#### AuthGuard (Existente)
- **Função**: Protege páginas administrativas com auth Supabase
- **Usado em**: Páginas como Home, Produtos, etc.

### 3. Páginas

#### /cliente (Nova - Com Google Auth)
- **Arquivo**: `Components/Pages/ClienteGoogle.razor`
- **Proteção**: `<ClientAuthGuard>`
- **Características**:
  - Login obrigatório com Google
  - Interface personalizada com dados do usuário Google
  - Nome do cliente obtido automaticamente da conta Google
  - Botão de logout visível

#### /cliente-simples (Antiga página)
- **Arquivo**: `Components/Pages/Cliente.razor` (renomeada)
- **Proteção**: Nenhuma
- **Características**:
  - Acesso direto sem login
  - Cliente precisa informar nome manualmente
  - Funciona como fallback

#### /bar, /cozinha, /pedidos (Operacionais)
- **Proteção**: Nenhuma (Google Auth)
- **Status**: Funcionam normalmente sem qualquer autenticação Google
- **Acesso**: Direto para funcionários

### 4. Navegação

A navegação foi atualizada para refletir a nova estrutura:

```
🏖️ ÁREA DO CLIENTE
├── 🔐 Pedidos com Google (/cliente)
└── Pedidos Simples (/cliente-simples)

⚡ OPERAÇÕES  
├── 🍳 Cozinha (/cozinha)
└── 🍺 Bar (/bar)
```

## Fluxo de Funcionamento

### Para Clientes (Fazer Pedidos)

1. **Opção 1 - Com Google** (`/cliente`):
   - Cliente acessa `/cliente`
   - `ClientAuthGuard` verifica autenticação
   - Se não logado: Tela de login Google
   - Se logado: Acesso ao cardápio com dados do Google

2. **Opção 2 - Simples** (`/cliente-simples`):
   - Cliente acessa `/cliente-simples`
   - Acesso direto ao cardápio
   - Precisa informar nome manualmente

### Para Funcionários (Operações)

1. **Acesso Direto**:
   - Funcionário acessa `/bar` ou `/cozinha`
   - Acesso imediato sem qualquer login
   - Funcionalidade completa disponível

## Implementação Técnica

### GoogleClientAuthService

```csharp
// Login simulado (por enquanto)
public async Task<GoogleClientInfo?> LoginWithGoogleAsync()
{
    // TODO: Integrar com Google OAuth real
    var client = new GoogleClientInfo
    {
        Id = Guid.NewGuid().ToString(),
        Name = "Cliente Google",
        Email = "cliente@gmail.com",
        PictureUrl = "",
        LoginTime = DateTime.Now
    };
    
    _currentClient = client;
    ClientAuthStateChanged?.Invoke(_currentClient);
    return client;
}
```

### ClientAuthGuard

```razor
@if (isClientAuthenticated)
{
    @ChildContent  <!-- Mostra conteúdo protegido -->
}
else
{
    <!-- Tela de login Google -->
    <button @onclick="LoginWithGoogle">
        <i class="bi bi-google"></i> Entrar com Google
    </button>
}
```

## Próximos Passos

### 1. Integração Google OAuth Real

Para implementar Google OAuth real, você precisará:

1. **Configurar Google Cloud Console**:
   - Criar projeto
   - Habilitar Google+ API
   - Configurar OAuth 2.0 credentials

2. **Instalar Pacotes NuGet**:
   ```xml
   <PackageReference Include="Google.Apis.Auth" Version="1.60.0" />
   <PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="7.0.0" />
   ```

3. **Implementar OAuth Real**:
   ```csharp
   // No GoogleClientAuthService
   var result = await GoogleAuth.DefaultInstance.SignInAsync();
   if (result.IsSuccess)
   {
       var googleUser = result.User;
       // Processar dados reais do usuário
   }
   ```

### 2. Persistência de Sessão

Adicionar persistência local para manter login:

```csharp
// Salvar no Preferences
await SecureStorage.SetAsync("google_client_token", token);

// Recuperar na inicialização
var savedToken = await SecureStorage.GetAsync("google_client_token");
```

### 3. Sincronização com Backend

Integrar com Supabase para salvar pedidos com dados do cliente Google:

```csharp
var pedido = new Pedido
{
    NomeCliente = currentClient.Name,
    EmailCliente = currentClient.Email,
    GoogleUserId = currentClient.Id,
    // ... outros dados
};
```

## Vantagens da Implementação

### ✅ Para o Negócio
- **Área do cliente** com identificação segura
- **Área operacional** continua ágil sem logins desnecessários
- **Flexibilidade** - cliente pode escolher login Google ou simples
- **Dados do cliente** mais precisos para marketing

### ✅ Para os Desenvolvedores
- **Separação clara** de responsabilidades
- **Escalabilidade** - fácil adicionar novas áreas protegidas
- **Manutenibilidade** - cada serviço de auth é independente
- **Testes** - cada área pode ser testada separadamente

### ✅ Para os Usuários
- **Clientes**: Login rápido com Google ou opção simples
- **Funcionários**: Acesso imediato às ferramentas de trabalho
- **Flexibilidade**: Cada tipo de usuário tem a experiência adequada

## Troubleshooting

### Problema: "Serviço não registrado"
**Solução**: Verificar se `IGoogleClientAuthService` está registrado em `MauiProgram.cs`

### Problema: "Página não carrega"
**Solução**: Verificar se o componente `ClientAuthGuard` tem a sintaxe correta

### Problema: "Login não funciona"
**Solução**: Por enquanto está simulado - implementar Google OAuth real

---

**Documentação criada em**: Janeiro 2025  
**Versão**: 1.0  
**Status**: Implementação base concluída, aguardando OAuth real
