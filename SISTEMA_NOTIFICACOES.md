# 🔔 Sistema de Notificações em Tempo Real - AdmBeachApp

## 📋 Visão Geral

O sistema de notificações foi implementado para automaticamente distribuir pedidos para os setores apropriados (Cozinha, Bar e Caixa) assim que um cliente faz um pedido.

## 🏗️ Arquitetura

### Componentes Principais

1. **EventNotificacaoService** - Serviço principal de notificações baseado em eventos
2. **PedidoServiceComNotificacao** - Wrapper que integra notificações aos pedidos
3. **Páginas de Setores** - Interfaces dedicadas para Cozinha e Bar
4. **Modelos de Notificação** - Estruturas de dados para categorizar itens por setor

### Fluxo de Funcionamento

```
Cliente faz pedido → PedidoService → NotificacaoService → Setores (Cozinha/Bar/Caixa)
```

## 🎯 Funcionalidades

### 1. Distribuição Automática por Setor

Os produtos são automaticamente categorizados:

- **🍳 Cozinha**: Comidas, sanduíches, lanches, petiscos, pratos
- **🍺 Bar**: Bebidas, cerveja, refrigerante, água, sucos, drinks  
- **🛒 Balcão**: Protetor solar, acessórios, diversos

### 2. Notificações em Tempo Real

- ✅ Novos pedidos são enviados instantaneamente
- ✅ Atualizações de status são propagadas em tempo real
- ✅ Cancelamentos são notificados imediatamente

### 3. Interfaces Especializadas

#### Página da Cozinha (`/cozinha`)
- Visualiza apenas itens relevantes para a cozinha
- Permite iniciar preparo e marcar como pronto
- Mostra tempo de espera em tempo real
- Exibe observações específicas dos itens

#### Página do Bar (`/bar`)
- Filtra apenas bebidas e itens do bar
- Interface otimizada para preparação rápida
- Calcula totais específicos de bebidas
- Categoria de produtos destacada

### 4. Recursos de UX

- 🔊 **Sons de Notificação**: Alerta sonoro para novos pedidos
- 🎨 **Cores por Setor**: Visual distintivo para cada área
- 📱 **Interface Responsiva**: Funciona em tablets e celulares
- ⏰ **Tempo Real**: Atualizações automáticas sem refresh

## 🛠️ Como Usar

### Para Operadores da Cozinha

1. Acesse `/cozinha` no menu "⚡ OPERAÇÕES"
2. Visualize pedidos pendentes com itens da cozinha
3. Clique "Iniciar Preparo" para marcar como em preparo
4. Clique "Pronto!" quando o item estiver pronto
5. As notificações aparecerão automaticamente no topo

### Para Operadores do Bar

1. Acesse `/bar` no menu "⚡ OPERAÇÕES"
2. Visualize pedidos com bebidas
3. Gerencie o status dos pedidos
4. Veja totais específicos de bebidas

### Para o Caixa

- O caixa continua vendo todos os pedidos na página principal
- Recebe notificações de status de todos os setores
- Pode finalizar pedidos quando todos itens estão prontos

## ⚙️ Configuração Técnica

### Registros de Serviços (MauiProgram.cs)

```csharp
// Serviço de notificações
builder.Services.AddSingleton<IEventNotificacaoService, EventNotificacaoService>();

// PedidoService com notificações integradas
builder.Services.AddScoped<IPedidoService>(provider => {
    var baseService = provider.GetRequiredService<SupabasePedidoService>();
    var notificacaoService = provider.GetRequiredService<IEventNotificacaoService>();
    var logger = provider.GetRequiredService<ILogger<PedidoServiceComNotificacao>>();
    return new PedidoServiceComNotificacao(baseService, notificacaoService, logger);
});
```

### Categorização de Produtos

```csharp
public static TipoSetor DeterminarSetor(this Produto produto)
{
    return produto.Categoria?.ToLower() switch
    {
        "bebida" or "cerveja" or "refrigerante" or "água" or "suco" or "drink" => TipoSetor.Bar,
        "comida" or "sanduíche" or "lanche" or "petisco" or "porção" or "prato" => TipoSetor.Cozinha,
        "protetor solar" or "acessório" or "diversos" => TipoSetor.Balcao,
        _ => TipoSetor.Balcao
    };
}
```

## 🎨 Recursos Visuais

### Ícones por Setor
- 🍳 Cozinha: Ícone de ovo frito
- 🍺 Bar: Ícone de copo com canudo
- 🛒 Balcão: Ícone de carrinho de compras
- 📢 Todos: Ícone de megafone

### Esquema de Cores
- **Cozinha**: Amarelo (warning)
- **Bar**: Azul (info)
- **Balcão**: Verde (success)
- **Geral**: Azul primário (primary)

## 🔧 Desenvolvimento e Extensões

### Adicionando Novos Setores

1. Adicione novo valor ao enum `TipoSetor`
2. Atualize método `DeterminarSetor()` 
3. Adicione cores e ícones correspondentes
4. Crie página especializada se necessário

### Personalizando Notificações

O sistema permite fácil customização:

```csharp
// Exemplo de notificação personalizada
await NotificacaoService.NotificarNovoPedido(pedido);
```

### JavaScript APIs Disponíveis

- `playNotificationSound()` - Toca som de notificação
- `showToast(type, title, message)` - Mostra toast na tela
- `vibrate(pattern)` - Vibração em dispositivos móveis
- `showSystemNotification(title, message)` - Notificação nativa do sistema

## 📊 Logs e Monitoramento

O sistema registra logs detalhados:

- ✅ Criação de novos pedidos
- ✅ Distribuição para setores
- ✅ Atualizações de status
- ✅ Erros de processamento

## 🔒 Considerações de Segurança

- Notificações são processadas no lado do servidor
- Não há exposição de dados sensíveis
- Cada setor vê apenas seus itens relevantes

## 🚀 Próximos Passos

Melhorias futuras sugeridas:

- [ ] Persistência de notificações em banco
- [ ] Integração com WhatsApp/SMS
- [ ] Dashboard de métricas por setor
- [ ] Notificações push para dispositivos móveis
- [ ] Sistema de prioridades para pedidos urgentes

---

**Sistema desenvolvido para otimizar o fluxo de pedidos e melhorar a comunicação entre os setores do Beach Vibe! 🏖️**
