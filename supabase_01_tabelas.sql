-- =============================================
-- PARTE 1: CRIAR TABELAS NO SUPABASE
-- Execute no SQL Editor do Supabase Dashboard
-- =============================================

-- Tabela produtos
CREATE TABLE IF NOT EXISTS produtos (
    id SERIAL PRIMARY KEY,
    nome TEXT NOT NULL,
    descricao TEXT,
    preco DECIMAL(10,2) NOT NULL,
    quantidade_estoque INTEGER NOT NULL DEFAULT 0,
    estoque_minimo INTEGER NOT NULL DEFAULT 5,
    categoria TEXT,
    ativo BOOLEAN NOT NULL DEFAULT true,
    data_criacao TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    data_atualizacao TIMESTAMP WITH TIME ZONE
);

-- Tabela pedidos
CREATE TABLE IF NOT EXISTS pedidos (
    id SERIAL PRIMARY KEY,
    numero_pedido TEXT NOT NULL UNIQUE,
    nome_cliente TEXT NOT NULL,
    telefone_cliente TEXT,
    mesa TEXT,
    status TEXT NOT NULL DEFAULT 'Pendente',
    total DECIMAL(10,2) NOT NULL,
    observacoes TEXT,
    data_pedido TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    data_atualizacao TIMESTAMP WITH TIME ZONE
);

-- Tabela itens_pedido
CREATE TABLE IF NOT EXISTS itens_pedido (
    id SERIAL PRIMARY KEY,
    pedido_id INTEGER NOT NULL REFERENCES pedidos(id) ON DELETE CASCADE,
    produto_id INTEGER NOT NULL REFERENCES produtos(id),
    quantidade INTEGER NOT NULL,
    preco_unitario DECIMAL(10,2) NOT NULL,
    observacoes_item TEXT
);
