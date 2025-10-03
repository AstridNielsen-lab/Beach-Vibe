-- =============================================
-- PARTE 2: CRIAR ÍNDICES E INSERIR DADOS (VERSÃO SIMPLES)
-- Execute no SQL Editor do Supabase Dashboard
-- =============================================

-- Índices para otimização
CREATE INDEX IF NOT EXISTS idx_produtos_ativo ON produtos(ativo);
CREATE INDEX IF NOT EXISTS idx_produtos_categoria ON produtos(categoria);
CREATE INDEX IF NOT EXISTS idx_produtos_nome ON produtos(nome);

CREATE INDEX IF NOT EXISTS idx_pedidos_status ON pedidos(status);
CREATE INDEX IF NOT EXISTS idx_pedidos_data_pedido ON pedidos(data_pedido);
CREATE INDEX IF NOT EXISTS idx_pedidos_numero_pedido ON pedidos(numero_pedido);

CREATE INDEX IF NOT EXISTS idx_itens_pedido_pedido_id ON itens_pedido(pedido_id);
CREATE INDEX IF NOT EXISTS idx_itens_pedido_produto_id ON itens_pedido(produto_id);

-- Limpar produtos existentes se houver (opcional)
-- DELETE FROM produtos;

-- Inserir produtos de exemplo
INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Água Mineral 500ml', 'Água mineral natural', 3.50, 100, 20, 'Bebidas');

INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Refrigerante Lata 350ml', 'Refrigerante gelado', 5.00, 50, 15, 'Bebidas');

INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Cerveja Long Neck', 'Cerveja gelada 355ml', 8.00, 30, 10, 'Bebidas');

INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Sanduíche Natural', 'Sanduíche integral com peito de peru', 12.00, 25, 5, 'Lanches');

INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Hambúrguer Artesanal', 'Hambúrguer com carne 180g', 18.00, 15, 5, 'Lanches');

INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Batata Frita', 'Porção de batata frita crocante', 8.00, 40, 10, 'Petiscos');

INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Camarão Empanado', 'Porção de camarão empanado 300g', 25.00, 20, 5, 'Frutos do Mar');

INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Peixe Grelhado', 'Peixe grelhado com legumes', 22.00, 18, 3, 'Pratos Principais');

INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Picolé de Fruta', 'Picolé natural de frutas', 4.00, 60, 15, 'Sobremesas');

INSERT INTO produtos (nome, descricao, preco, quantidade_estoque, estoque_minimo, categoria) VALUES
('Açaí na Tigela', 'Açaí com frutas e granola', 15.00, 25, 5, 'Sobremesas');
