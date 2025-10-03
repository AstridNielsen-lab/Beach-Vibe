-- Script SQL para criar as tabelas do BeachApp no Supabase

-- Tabela de produtos
CREATE TABLE IF NOT EXISTS produtos (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(500),
    preco DECIMAL(10,2) NOT NULL,
    quantidade_estoque INTEGER NOT NULL DEFAULT 0,
    estoque_minimo INTEGER NOT NULL DEFAULT 5,
    categoria VARCHAR(50),
    ativo BOOLEAN NOT NULL DEFAULT true,
    data_criacao TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    data_atualizacao TIMESTAMP WITH TIME ZONE
);

-- Tabela de pedidos
CREATE TABLE IF NOT EXISTS pedidos (
    id SERIAL PRIMARY KEY,
    numero_pedido VARCHAR(20) NOT NULL UNIQUE,
    nome_cliente VARCHAR(100) NOT NULL,
    telefone_cliente VARCHAR(20),
    mesa VARCHAR(50),
    status VARCHAR(20) NOT NULL DEFAULT 'Pendente',
    total DECIMAL(10,2) NOT NULL DEFAULT 0,
    observacoes TEXT,
    data_pedido TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    data_atualizacao TIMESTAMP WITH TIME ZONE
);

-- Tabela de itens do pedido
CREATE TABLE IF NOT EXISTS itens_pedido (
    id SERIAL PRIMARY KEY,
    pedido_id INTEGER NOT NULL REFERENCES pedidos(id) ON DELETE CASCADE,
    produto_id INTEGER NOT NULL REFERENCES produtos(id) ON DELETE RESTRICT,
    quantidade INTEGER NOT NULL,
    preco_unitario DECIMAL(10,2) NOT NULL,
    observacoes_item VARCHAR(200)
);

-- Índices para melhor performance
CREATE INDEX IF NOT EXISTS idx_produtos_ativo ON produtos(ativo);
CREATE INDEX IF NOT EXISTS idx_produtos_categoria ON produtos(categoria);
CREATE INDEX IF NOT EXISTS idx_pedidos_data ON pedidos(data_pedido);
CREATE INDEX IF NOT EXISTS idx_pedidos_status ON pedidos(status);
CREATE INDEX IF NOT EXISTS idx_itens_pedido_id ON itens_pedido(pedido_id);

-- Habilitar RLS (Row Level Security)
ALTER TABLE produtos ENABLE ROW LEVEL SECURITY;
ALTER TABLE pedidos ENABLE ROW LEVEL SECURITY;
ALTER TABLE itens_pedido ENABLE ROW LEVEL SECURITY;

-- Políticas de segurança (permitir acesso público para leitura/escrita)
CREATE POLICY "Permitir acesso público aos produtos" ON produtos FOR ALL USING (true);
CREATE POLICY "Permitir acesso público aos pedidos" ON pedidos FOR ALL USING (true);
CREATE POLICY "Permitir acesso público aos itens do pedido" ON itens_pedido FOR ALL USING (true);

-- Inserir dados de exemplo
INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria, ativo) VALUES
('Água de Coco', 'Água de coco gelada e refrescante', 8.00, 50, 10, 'Bebidas', true),
('Caipirinha', 'Caipirinha tradicional com limão', 15.00, 30, 5, 'Bebidas', true),
('Porção de Camarão', 'Camarão empanado com molho especial', 35.00, 20, 3, 'Petiscos', true),
('Açaí na Tigela', 'Açaí com granola, banana e mel', 18.00, 25, 5, 'Sobremesas', true),
('Sanduíche Natural', 'Sanduíche natural com peito de peru', 12.00, 15, 3, 'Lanches', true)
ON CONFLICT DO NOTHING;
