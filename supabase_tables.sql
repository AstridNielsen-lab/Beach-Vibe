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

-- Índices para otimização
CREATE INDEX IF NOT EXISTS idx_produtos_ativo ON produtos(ativo);
CREATE INDEX IF NOT EXISTS idx_produtos_categoria ON produtos(categoria);
CREATE INDEX IF NOT EXISTS idx_produtos_nome ON produtos(nome);

CREATE INDEX IF NOT EXISTS idx_pedidos_status ON pedidos(status);
CREATE INDEX IF NOT EXISTS idx_pedidos_data_pedido ON pedidos(data_pedido);
CREATE INDEX IF NOT EXISTS idx_pedidos_numero_pedido ON pedidos(numero_pedido);

CREATE INDEX IF NOT EXISTS idx_itens_pedido_pedido_id ON itens_pedido(pedido_id);
CREATE INDEX IF NOT EXISTS idx_itens_pedido_produto_id ON itens_pedido(produto_id);

-- Inserir alguns produtos de exemplo
INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Água Mineral 500ml', 'Água mineral natural', 3.50, 100, 20, 'Bebidas'),
('Refrigerante Lata 350ml', 'Refrigerante gelado', 5.00, 50, 15, 'Bebidas'),
('Cerveja Long Neck', 'Cerveja gelada 355ml', 8.00, 30, 10, 'Bebidas'),
('Sanduíche Natural', 'Sanduíche integral com peito de peru', 12.00, 25, 5, 'Lanches'),
('Hambúrguer Artesanal', 'Hambúrguer com carne 180g', 18.00, 15, 5, 'Lanches'),
('Batata Frita', 'Porção de batata frita crocante', 8.00, 40, 10, 'Petiscos'),
('Camarão Empanado', 'Porção de camarão empanado 300g', 25.00, 20, 5, 'Frutos do Mar'),
('Peixe Grelhado', 'Peixe grelhado com legumes', 22.00, 18, 3, 'Pratos Principais'),
('Picolé de Fruta', 'Picolé natural de frutas', 4.00, 60, 15, 'Sobremesas'),
('Açaí na Tigela', 'Açaí com frutas e granola', 15.00, 25, 5, 'Sobremesas')
ON CONFLICT (nome) DO NOTHING;

-- Habilitar Row Level Security (RLS) para segurança
ALTER TABLE produtos ENABLE ROW LEVEL SECURITY;
ALTER TABLE pedidos ENABLE ROW LEVEL SECURITY;
ALTER TABLE itens_pedido ENABLE ROW LEVEL SECURITY;

-- Criar policies para permitir acesso total (você pode ajustar conforme necessário)
-- Remover policies existentes se houver
DROP POLICY IF EXISTS "Permitir acesso completo a produtos" ON produtos;
DROP POLICY IF EXISTS "Permitir acesso completo a pedidos" ON pedidos;
DROP POLICY IF EXISTS "Permitir acesso completo a itens_pedido" ON itens_pedido;

-- Criar novas policies
CREATE POLICY "Permitir acesso completo a produtos" ON produtos
    FOR ALL USING (true);

CREATE POLICY "Permitir acesso completo a pedidos" ON pedidos
    FOR ALL USING (true);

CREATE POLICY "Permitir acesso completo a itens_pedido" ON itens_pedido
    FOR ALL USING (true);
