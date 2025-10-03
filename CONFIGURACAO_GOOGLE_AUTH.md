# Configuração da Autenticação Google no Supabase

## ✅ Dados do Projeto

- **URL do Supabase**: `https://givgbgiynnkmxpxojthx.supabase.co`
- **Client ID Google**: `{{GOOGLE_CLIENT_ID}}`
- **Client Secret Google**: `{{GOOGLE_CLIENT_SECRET}}`
- **URL de Callback**: `https://givgbgiynnkmxpxojthx.supabase.co/auth/v1/callback`

## 📋 Passos para Configuração

### 1. Acessar o Painel do Supabase

1. Vá para [https://supabase.com](https://supabase.com)
2. Faça login na sua conta
3. Selecione o projeto `givgbgiynnkmxpxojthx`

### 2. Configurar Provider Google

1. No painel do Supabase, vá para **Authentication** → **Providers**
2. Encontre o provider **Google** na lista
3. Clique no switch para **habilitar** o Google
4. Preencha os campos:

   ```
   Client ID (for OAuth): {{GOOGLE_CLIENT_ID}}
   Client Secret (for OAuth): {{GOOGLE_CLIENT_SECRET}}
   ```

5. Em **Redirect URLs**, adicione:
   ```
   https://givgbgiynnkmxpxojthx.supabase.co/auth/v1/callback
   ```

6. Clique em **Save** para salvar as configurações

### 3. Configurar URLs Permitidas (Site URL)

1. Ainda na seção **Authentication**, vá para **URL Configuration**
2. Em **Site URL**, adicione:
   ```
   https://givgbgiynnkmxpxojthx.supabase.co
   ```

3. Em **Redirect URLs**, adicione as URLs da sua aplicação:
   ```
   https://givgbgiynnkmxpxojthx.supabase.co/auth/v1/callback
   http://localhost:5000
   http://localhost:5001
   ```

### 4. Configurar Banco de Dados (Execute os Scripts SQL)

1. Vá para **SQL Editor** no painel do Supabase
2. Execute os scripts na seguinte ordem:

   **Passo 1:** Execute `supabase_01_tabelas.sql` para criar as tabelas
   **Passo 2:** Execute `supabase_02_indices_dados.sql` para criar índices e inserir dados
   **Passo 3:** Execute `supabase_03_seguranca.sql` para configurar segurança

3. Verifique se as tabelas foram criadas:

   ```sql
   -- Verificar se as tabelas foram criadas
   SELECT table_name FROM information_schema.tables 
   WHERE table_schema = 'public' 
   AND table_name IN ('produtos', 'pedidos', 'itens_pedido');
   
   -- Verificar se há produtos de exemplo
   SELECT COUNT(*) FROM produtos;
   ```

### 5. Testar Autenticação

1. Execute a aplicação:
   ```bash
   dotnet run
   ```

2. Navegue até `/login` na aplicação
3. Clique em "Entrar com Google"
4. Deve abrir uma janela do Google para autenticação

## 🔧 Configurações no Google Cloud Console

### Verificar Configurações Existentes

1. Acesse [Google Cloud Console](https://console.cloud.google.com)
2. Vá para **APIs & Services** → **Credentials**
3. Encontre o cliente OAuth com ID `656962033397-...`
4. Verifique se as **Authorized redirect URIs** incluem:
   ```
   https://givgbgiynnkmxpxojthx.supabase.co/auth/v1/callback
   ```

### Se Precisar Criar Novo Cliente OAuth

1. Clique em **+ CREATE CREDENTIALS** → **OAuth client ID**
2. Escolha **Web application**
3. Nome: `Beach Vibe Admin - Supabase`
4. **Authorized redirect URIs**:
   ```
   https://givgbgiynnkmxpxojthx.supabase.co/auth/v1/callback
   ```

## 🚀 Testando a Integração

### 1. Verificar Conexão com Supabase

```csharp
// No código, verificar se a conexão está funcionando
var supabaseClient = serviceProvider.GetService<Supabase.Client>();
var user = await supabaseClient.Auth.GetUser();
```

### 2. Testar Login

1. Abra a aplicação
2. Vá para a página de login (`/login`)
3. Clique em "Entrar com Google"
4. Complete o fluxo de autenticação
5. Deve redirecionar para a página principal (`/`)

### 3. Verificar Dados no Supabase

1. No painel do Supabase, vá para **Authentication** → **Users**
2. Deve aparecer o usuário logado com os dados do Google
3. Em **Table Editor** → **auth** → **users**, verifique os metadados

## 🛠️ Solução de Problemas

### Erro: "Invalid redirect URI"
- Verifique se a URL de callback está corretamente configurada no Google Cloud Console
- Confirme se o Supabase está usando a URL correta

### Erro: "OAuth provider not configured"
- Verifique se o provider Google está habilitado no Supabase
- Confirme se Client ID e Secret estão corretos

### Erro: "Session not found"
- Limpe o cache/cookies do navegador
- Verifique se as URLs estão corretas no Supabase

### Erro de CORS
- Adicione as URLs necessárias nas configurações do Supabase
- Verifique se a aplicação está rodando nas URLs permitidas

## 📱 Configurações para Produção

Quando for fazer deploy da aplicação, adicione as URLs de produção:

1. **Supabase** → **Authentication** → **URL Configuration**
2. Adicione as URLs de produção em **Redirect URLs**
3. **Google Cloud Console** → adicione URLs de produção nas **Authorized redirect URIs**

## ✨ Recursos Disponíveis

Após a configuração, a aplicação terá:

- ✅ Login com Google
- ✅ Logout automático
- ✅ Persistência de sessão
- ✅ Dados do usuário sincronizados
- ✅ Interface de usuário com avatar
- ✅ Redirecionamento automático após login/logout

## 📊 Monitoramento

Para monitorar a autenticação:

1. **Supabase** → **Authentication** → **Users** (visualizar usuários logados)
2. **Supabase** → **Logs** (verificar logs de autenticação)
3. **Google Cloud Console** → **APIs & Services** → **Credentials** (monitorar uso da API)
