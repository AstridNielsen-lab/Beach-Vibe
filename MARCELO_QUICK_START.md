# 🏖️ AdmBeachApp - Quick Start para Marcelo

**Versão:** 1.0 - Entrega Completa  
**Ambiente:** Windows + Podman  
**Status:** ✅ PRONTO PARA USO

---

## 🚀 Comandos Essenciais

### Iniciar tudo:
```powershell
.\admbeach-simple.ps1 start
```

### Ver status:
```powershell
.\admbeach-simple.ps1 status
```

### Parar tudo:
```powershell
.\admbeach-simple.ps1 stop
```

---

## 🌐 Acessar Serviços

- **pgAdmin** (admin do banco): http://localhost:8080
  - Email: `dev@admbeachapp.com`
  - Senha: `dev123456`

- **MailHog** (email de teste): http://localhost:8025

- **PostgreSQL** (conexão direta):
  - Host: `localhost:5432`
  - Database: `admbeachapp`
  - User: `admbeach`
  - Password: `dev123456`

---

## 🔧 Comandos Avançados

```powershell
# Conectar ao banco PostgreSQL via terminal
.\admbeach-simple.ps1 shell

# Ver logs dos serviços
.\admbeach-simple.ps1 logs

# Abrir pgAdmin e MailHog no browser
.\admbeach-simple.ps1 open

# Limpar containers parados
.\admbeach-simple.ps1 clean
```

---

## 📋 Checklist de Validação

- [ ] Executar `.\admbeach-simple.ps1 start`
- [ ] Acessar http://localhost:8080 (pgAdmin)
- [ ] Acessar http://localhost:8025 (MailHog)
- [ ] Executar `.\admbeach-simple.ps1 shell` e listar tabelas (`\dt`)
- [ ] Verificar tabelas: `produtos`, `pedidos`, `itens_pedido`

---

## 📁 Documentação Completa

- **`docs/ENTREGA_MARCELO.md`** - Documentação técnica completa
- **`README-AMBIENTE.md`** - Guia operacional detalhado
- **`admbeach-simple.ps1`** - Script principal de gerenciamento

---

## 🆘 Problemas Comuns

**Erro "podman machine not running":**
```powershell
podman machine start
```

**Portas ocupadas:**
```powershell
netstat -ano | findstr ":5432"
```

**Reset completo (remove dados):**
```powershell
podman compose down -v
podman system prune -a -f
.\admbeach-simple.ps1 start
```

---

**🎯 TUDO FUNCIONAL - AMBIENTE PRONTO PARA DESENVOLVIMENTO!**
