# AdmBeachApp

📌 **Visão Geral**

O AdmBeachApp é um software proprietário desenvolvido para a gestão de quiosques de praia, facilitando o controle de vendas, estoque e operações do dia a dia. O aplicativo oferece autenticação segura via Google OAuth, armazenamento local com SQLite, interface responsiva com .NET MAUI e Blazor, e uma gestão eficiente de produtos e transações.

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

🔹 **.NET MAUI** – Desenvolvimento multiplataforma  
🔹 **Blazor** – Interface web moderna e dinâmica  
🔹 **SQLite** – Banco de dados local e eficiente  
🔹 **Google OAuth** – Autenticação segura  
🔹 **C#** – Linguagem principal do projeto  
🔹 **Entity Framework Core** – ORM para acesso a dados  
🔹 **Bootstrap** – Framework CSS para interface responsiva  

## 📁 Estrutura do Projeto

```
AdmBeachApp/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   └── Pages/
│       ├── Home.razor           # Dashboard principal
│       ├── Produtos.razor       # Gestão de produtos
│       └── Vendas.razor         # Gestão de vendas
├── Data/
│   └── BeachAppContext.cs       # Contexto do banco de dados
├── Models/
│   ├── Produto.cs               # Modelo de produto
│   └── Venda.cs                 # Modelo de venda e itens
├── Services/
│   ├── ProdutoService.cs        # Serviços de produtos
│   └── VendaService.cs          # Serviços de vendas
└── wwwroot/
    ├── css/
    │   └── app.css              # Estilos personalizados
    └── index.html
```

## 🚀 Como Executar

### Pré-requisitos

- .NET 8.0 SDK ou superior
- Visual Studio 2022 ou Visual Studio Code
- Emulador Android/iOS (para teste mobile) ou Windows (para desktop)

### Passos para executar

1. **Clone o repositório**
   ```bash
   git clone [url-do-repositorio]
   cd AdmBeachApp
   ```

2. **Restaurar pacotes NuGet**
   ```bash
   dotnet restore
   ```

3. **Executar o projeto**
   ```bash
   dotnet run
   ```

   Ou no Visual Studio: `F5` para debug ou `Ctrl+F5` para executar sem debug

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

O aplicativo utiliza **SQLite** para armazenamento local com:
- **Produtos**: Nome, descrição, preço, estoque, categoria
- **Vendas**: Data, cliente, método de pagamento, total
- **Itens de Venda**: Quantidade, preço unitário, subtotal
- **Dados iniciais**: 5 produtos pré-cadastrados

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

