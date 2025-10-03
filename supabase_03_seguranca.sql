-- =============================================
-- PARTE 3: CONFIGURAR SEGURANÇA (RLS)
-- Execute no SQL Editor do Supabase Dashboard
-- =============================================

-- Habilitar Row Level Security (RLS)
ALTER TABLE produtos ENABLE ROW LEVEL SECURITY;
ALTER TABLE pedidos ENABLE ROW LEVEL SECURITY;
ALTER TABLE itens_pedido ENABLE ROW LEVEL SECURITY;

-- Remover policies existentes se houver (ignorar erros se não existirem)
DROP POLICY IF EXISTS "Permitir acesso completo a produtos" ON produtos;
DROP POLICY IF EXISTS "Permitir acesso completo a pedidos" ON pedidos;
DROP POLICY IF EXISTS "Permitir acesso completo a itens_pedido" ON itens_pedido;

-- Criar policies para permitir acesso total
-- ATENÇÃO: Em produção, você deve criar policies mais restritivas!
CREATE POLICY "Permitir acesso completo a produtos" ON produtos
    FOR ALL USING (true);

CREATE POLICY "Permitir acesso completo a pedidos" ON pedidos
    FOR ALL USING (true);

CREATE POLICY "Permitir acesso completo a itens_pedido" ON itens_pedido
    FOR ALL USING (true);
