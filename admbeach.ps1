# AdmBeach.ps1 - Script de gerenciamento do ambiente AdmBeachApp
# Execute: .\admbeach.ps1 [comando]

param(
    [Parameter(Position=0)]
    [string]$Command = "help",
    [string]$Service = ""
)

$ErrorActionPreference = "Continue"

function Show-Header {
    Write-Host @"
╔══════════════════════════════════════════════════════════════════╗
║                          🏖️  AdmBeachApp  🏖️                         ║
║                    Ambiente de Desenvolvimento                   ║
╚══════════════════════════════════════════════════════════════════╝
"@ -ForegroundColor Cyan
}

function Show-Help {
    Show-Header
    Write-Host @"

📋 COMANDOS DISPONÍVEIS:

   🚀 start        Inicia o ambiente completo
   🛑 stop         Para todos os serviços
   🔄 restart      Reinicia o ambiente
   📊 status       Mostra status dos containers
   📋 logs         Mostra logs dos serviços
   🐚 shell        Abre shell no PostgreSQL
   🌐 open         Abre interfaces web
   🧹 clean        Remove containers parados
   📱 app          Compila e roda a aplicação MAUI
   🔧 setup        Configura o ambiente pela primeira vez
   ❓ help         Mostra esta ajuda

📖 EXEMPLOS:
   .\admbeach.ps1 start          # Inicia ambiente
   .\admbeach.ps1 logs postgres  # Logs do PostgreSQL
   .\admbeach.ps1 shell          # Shell do banco
   .\admbeach.ps1 open           # Abre pgAdmin e MailHog

"@
}

function Test-PodmanRunning {
    try {
        $machineStatus = podman machine inspect --format "{{.State}}" 2>$null
        return $machineStatus -eq "running"
    }
    catch {
        return $false
    }
}

function Start-Environment {
    Show-Header
    Write-Host "🚀 Iniciando ambiente AdmBeachApp..." -ForegroundColor Yellow
    
    if (!(Test-PodmanRunning)) {
        Write-Host "⚡ Iniciando máquina Podman..." -ForegroundColor Cyan
        podman machine start
        Start-Sleep 3
    }
    
    Write-Host "📦 Subindo serviços..." -ForegroundColor Cyan
    podman compose up -d postgres redis pgadmin mailhog
    
    Start-Sleep 5
    Show-Services-Info
}

function Stop-Environment {
    Show-Header
    Write-Host "🛑 Parando ambiente..." -ForegroundColor Yellow
    podman compose down
    Write-Host "✅ Ambiente parado!" -ForegroundColor Green
}

function Restart-Environment {
    Show-Header
    Write-Host "🔄 Reiniciando ambiente..." -ForegroundColor Cyan
    Stop-Environment
    Start-Sleep 2
    Start-Environment
}

function Show-Status {
    Show-Header
    Write-Host "📊 Status do Ambiente" -ForegroundColor Yellow
    Write-Host "═══════════════════════" -ForegroundColor Yellow
    
    try {
        Write-Host "`n🖥️  MÁQUINA PODMAN:" -ForegroundColor Cyan
        $machineInfo = podman machine inspect --format "Estado: {{.State}} | CPUs: {{.Resources.CPUs}} | RAM: {{.Resources.Memory}}" 2>$null
        if ($machineInfo) {
            Write-Host "   $machineInfo" -ForegroundColor White
        } else {
            Write-Host "   ❌ Não inicializada" -ForegroundColor Red
        }
        
        Write-Host "`n🐳 CONTAINERS:" -ForegroundColor Cyan
        podman ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" | Select-String "admbeachapp|NAMES"
        
        Write-Host "`n💾 VOLUMES:" -ForegroundColor Cyan
        podman volume ls | Select-String "admbeachapp|DRIVER"
        
    }
    catch {
        Write-Host "❌ Erro ao obter status: $_" -ForegroundColor Red
    }
}

function Show-Logs {
    param([string]$ServiceName = "")
    
    Show-Header
    if ($ServiceName -eq "") {
        Write-Host "📋 Logs de todos os serviços:" -ForegroundColor Cyan
        podman compose logs --tail=50 -f
    } else {
        Write-Host "📋 Logs do serviço '$ServiceName':" -ForegroundColor Cyan
        podman compose logs --tail=50 -f $ServiceName
    }
}

function Open-Shell {
    Show-Header
    Write-Host "🐚 Conectando ao PostgreSQL..." -ForegroundColor Cyan
    Write-Host "💡 Para sair: digite \q e pressione Enter" -ForegroundColor Yellow
    Write-Host "─────────────────────────────────────────" -ForegroundColor Gray
    
    try {
        podman exec -it admbeachapp-postgres psql -U admbeach -d admbeachapp
    }
    catch {
        Write-Host "❌ Erro ao conectar: $_" -ForegroundColor Red
        Write-Host "💡 Certifique-se que o ambiente está rodando: .\admbeach.ps1 start" -ForegroundColor Yellow
    }
}

function Open-WebInterfaces {
    Show-Header
    Write-Host "🌐 Abrindo interfaces web..." -ForegroundColor Cyan
    
    $urls = @(
        "http://localhost:8080",  # pgAdmin
        "http://localhost:8025"   # MailHog
    )
    
    foreach ($url in $urls) {
        try {
            Start-Process $url
            Write-Host "✅ Abrindo: $url" -ForegroundColor Green
        }
        catch {
            Write-Host "❌ Erro ao abrir: $url" -ForegroundColor Red
        }
    }
    
    Write-Host "`n🔑 Credenciais:" -ForegroundColor Yellow
    Write-Host "   pgAdmin: dev@admbeachapp.com / dev123456" -ForegroundColor Gray
}

function Clean-Environment {
    Show-Header
    Write-Host "🧹 Limpando containers parados..." -ForegroundColor Cyan
    
    try {
        $stopped = podman ps -aq --filter status=exited
        if ($stopped) {
            podman rm $stopped
            Write-Host "✅ Containers removidos!" -ForegroundColor Green
        } else {
            Write-Host "✨ Nenhum container parado encontrado." -ForegroundColor Green
        }
    }
    catch {
        Write-Host "❌ Erro durante limpeza: $_" -ForegroundColor Red
    }
}

function Build-App {
    Show-Header
    Write-Host "📱 Compilando aplicacao MAUI..." -ForegroundColor Cyan
    
    try {
        Write-Host "🔧 Restaurando pacotes..." -ForegroundColor Yellow
        dotnet restore
        
        Write-Host "🏗️  Compilando..." -ForegroundColor Yellow  
        dotnet build --configuration Debug
        
        Write-Host "✅ Compilacao concluida!" -ForegroundColor Green
        Write-Host "💡 Para executar: dotnet run" -ForegroundColor Cyan
    }
    catch {
        Write-Host "❌ Erro na compilacao: $_" -ForegroundColor Red
    }
}

function Setup-Environment {
    Show-Header
    Write-Host "🔧 Configuração inicial do ambiente..." -ForegroundColor Yellow
    
    # Verifica se Podman está instalado
    try {
        $version = podman --version
        Write-Host "✅ Podman encontrado: $version" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Podman não instalado!" -ForegroundColor Red
        Write-Host "💡 Execute como Admin: .\scripts\setup-podman-simple.ps1 -Install" -ForegroundColor Yellow
        return
    }
    
    # Verifica máquina Podman
    if (!(Test-PodmanRunning)) {
        Write-Host "⚡ Inicializando Podman..." -ForegroundColor Cyan
        try {
            podman machine start
        }
        catch {
            Write-Host "💡 Execute: .\scripts\setup-podman-simple.ps1 -Init" -ForegroundColor Yellow
            return
        }
    }
    
    Write-Host "🚀 Iniciando ambiente pela primeira vez..." -ForegroundColor Cyan
    Start-Environment
}

function Show-Services-Info {
    Write-Host @"

🎉 AMBIENTE PRONTO!

┌─────────────────────────────────────────────────────────────┐
│                    🌐 SERVIÇOS DISPONÍVEIS                    │
├─────────────────────────────────────────────────────────────┤
│ 🐘 PostgreSQL    localhost:5432                             │
│    Database:     admbeachapp                                │
│    User:         admbeach                                   │
│    Password:     dev123456                                  │
│                                                             │
│ 🔴 Redis         localhost:6379                             │
│                                                             │
│ 🌐 pgAdmin       http://localhost:8080                      │
│    Email:        dev@admbeachapp.com                        │
│    Password:     dev123456                                  │
│                                                             │
│ 📧 MailHog       http://localhost:8025                      │
│    SMTP:         localhost:1025                             │
└─────────────────────────────────────────────────────────────┘

💡 PRÓXIMOS PASSOS:
   .\admbeach.ps1 app     # Compilar aplicação MAUI
   .\admbeach.ps1 shell   # Conectar ao PostgreSQL
   .\admbeach.ps1 open    # Abrir pgAdmin e MailHog
   .\admbeach.ps1 logs    # Ver logs dos serviços

"@ -ForegroundColor White
}

# ═══════════════════════════════════════════════════════════
# EXECUÇÃO PRINCIPAL
# ═══════════════════════════════════════════════════════════

switch ($Command.ToLower()) {
    "start"   { Start-Environment }
    "stop"    { Stop-Environment }
    "restart" { Restart-Environment }
    "status"  { Show-Status }
    "logs"    { Show-Logs -ServiceName $Service }
    "shell"   { Open-Shell }
    "open"    { Open-WebInterfaces }
    "clean"   { Clean-Environment }
    "app"     { Build-App }
    "setup"   { Setup-Environment }
    "help"    { Show-Help }
    default   { 
        Write-Host "❌ Comando não reconhecido: '$Command'" -ForegroundColor Red
        Show-Help 
    }
}
