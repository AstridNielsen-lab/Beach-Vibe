# 🏖️ Quiosque da Praia - Aplicativo Cliente

Este é o aplicativo cliente do sistema do Quiosque da Praia, desenvolvido em .NET MAUI com Blazor. O app permite que os clientes façam pedidos diretamente de suas mesas na praia.

## 📱 Funcionalidades

- **Cardápio Digital**: Visualização completa dos produtos disponíveis
- **Filtro por Categoria**: Bebidas, Petiscos, Lanches, Sobremesas
- **Carrinho de Compras**: Adicionar itens e gerenciar quantidades
- **Informações do Cliente**: Nome, telefone e mesa
- **Finalizar Pedido**: Sistema de pedidos com numeração automática
- **Interface Responsiva**: Design otimizado para dispositivos móveis

## 🚀 Como Usar

### Para Clientes
1. Abra o app no seu celular
2. Preencha suas informações (nome, telefone, mesa)
3. Navegue pelo cardápio e adicione itens ao carrinho
4. Adicione observações se necessário
5. Finalize seu pedido
6. Aguarde a confirmação e acompanhe o preparo

### Para Desenvolvedores

## 📋 Pré-requisitos

- .NET 9.0 SDK
- Visual Studio 2022 ou Visual Studio Code
- Android SDK (para compilar APK)
- Java JDK 17+ (para Android)

## ⚙️ Configuração do Ambiente

### Opção 1: Script Automático
Execute o script de configuração como administrador:

```powershell
# Execute como Administrador
.\setup-android.ps1
```

### Opção 2: Configuração Manual

1. **Instalar .NET MAUI Workloads:**
```bash
dotnet workload install maui
```

2. **Instalar Android SDK:**
   - Instale o Android Studio ou
   - Baixe o Android SDK standalone

3. **Instalar Java JDK:**
   - Download: https://learn.microsoft.com/java/openjdk/download
   - Ou via winget: `winget install Microsoft.OpenJDK.17`

4. **Configurar Variáveis de Ambiente:**
   - `ANDROID_SDK_ROOT`: Caminho para o Android SDK
   - `JAVA_HOME`: Caminho para o Java JDK

## 🔧 Compilação e Build

### Build de Desenvolvimento
```bash
dotnet restore
dotnet build
```

### Gerar APK
```powershell
# Usando o script automatizado
.\build-apk.ps1

# Ou manualmente
dotnet publish -c Release -f net9.0-android
```

O APK será gerado em: `bin/Release/net9.0-android/publish/`

## 📁 Estrutura do Projeto

```
BeachApp_Cliente/
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor       # Layout principal
│   ├── Pages/
│   │   └── Home.razor            # Página do cardápio
│   ├── Routes.razor              # Roteamento
│   └── _Imports.razor            # Imports globais
├── Data/
│   └── BeachAppContext.cs        # Contexto do banco de dados
├── Models/
│   ├── Produto.cs               # Model do produto
│   └── Pedido.cs               # Models do pedido e itens
├── Services/
│   ├── ProdutoService.cs       # Serviço de produtos
│   └── PedidoService.cs       # Serviço de pedidos
├── Resources/                  # Recursos (ícones, imagens)
├── wwwroot/                   # Arquivos estáticos
└── Platforms/                 # Código específico por plataforma
```

## 🎨 Design e UI

- **Tema**: Inspirado em praias e ambiente relaxante
- **Cores**: Azul oceano, dourado (sol), vermelho (guarda-sol)
- **Ícones**: Emojis temáticos (🏖️, 🍽️, 🛒)
- **Layout**: Responsivo com Bootstrap 5

## 💾 Banco de Dados

Utiliza SQLite local com as seguintes tabelas:
- **Produtos**: Cardápio com preços e estoque
- **Pedidos**: Informações dos pedidos
- **ItensPedido**: Itens individuais de cada pedido

## 🔄 Sincronização

O app cliente funciona de forma independente, mas pode ser integrado com:
- Sistema administrativo do quiosque
- API para sincronização de produtos
- Sistema de pagamento online

## 🚀 Deploy e Distribuição

### APK para Android
1. Execute o build com `.\build-apk.ps1`
2. O APK será gerado em `bin/Release/net9.0-android/publish/`
3. Instale no dispositivo via ADB ou transferência direta

### Play Store (Futuro)
- Configure certificados de produção
- Teste em dispositivos variados
- Siga as diretrizes do Google Play

## 📝 Customização

### Adicionar Novos Produtos
Edite o método `OnModelCreating` em `BeachAppContext.cs`

### Modificar Layout
Ajuste os estilos em `MainLayout.razor` e `wwwroot/css/app.css`

### Personalizar Ícones
Substitua os arquivos em `Resources/AppIcon/` e `Resources/Splash/`

## 🐛 Solução de Problemas

### Erro de Android SDK
```
error XA5300: Não foi possível encontrar o diretório do SDK do Android
```
**Solução**: Execute `.\setup-android.ps1` ou configure `ANDROID_SDK_ROOT`

### Erro de Java JDK
```
error XA5300: Não foi possível encontrar o diretório do SDK do Java
```
**Solução**: Instale Java JDK 17+ e configure `JAVA_HOME`

### Problemas de Compilação
1. Limpe o projeto: `dotnet clean`
2. Restaure dependências: `dotnet restore`
3. Verifique se todos os workloads estão instalados: `dotnet workload list`

## 📞 Suporte

Para dúvidas ou problemas:
1. Verifique os logs de erro
2. Consulte a documentação do .NET MAUI
3. Execute o script de diagnóstico incluído

## 🔄 Próximas Versões

- [ ] Integração com sistema de pagamento
- [ ] Push notifications para status do pedido
- [ ] Cache offline de cardápio
- [ ] Modo escuro
- [ ] Suporte a múltiplos idiomas

---

**Desenvolvido para o Quiosque da Praia** 🏖️
