# AdmBeach.ps1 - Script de gerenciamento do ambiente AdmBeachApp
# Execute: .\admbeach-simple.ps1 [comando]

param(
    [Parameter(Position=0)]
    [string]$Command = "help"
)

$ErrorActionPreference = "Continue"

function Show-Help {
    Write-Host @"

=== AdmBeachApp - Ambiente de Desenvolvimento ===

COMANDOS DISPONÍVEIS:

   start        Inicia o ambiente completo
   stop         Para todos os servicos
   restart      Reinicia o ambiente
   status       Mostra status dos containers
   logs         Mostra logs dos servicos
   shell        Abre shell no PostgreSQL
   open         Abre interfaces web
   clean        Remove containers parados
   help         Mostra esta ajuda

EXEMPLOS:
   .\admbeach-simple.ps1 start          # Inicia ambiente
   .\admbeach-simple.ps1 logs           # Logs dos servicos
   .\admbeach-simple.ps1 shell          # Shell do banco
   .\admbeach-simple.ps1 open           # Abre pgAdmin e MailHog

"@ -ForegroundColor Cyan
}

function Start-Environment {
    Write-Host "Iniciando ambiente AdmBeachApp..." -ForegroundColor Yellow
    
    # Verifica se Podman esta rodando
    try {
        $machineStatus = podman machine inspect --format "{{.State}}" 2>$null
        if ($machineStatus -ne "running") {
            Write-Host "Iniciando maquina Podman..." -ForegroundColor Cyan
            podman machine start
            Start-Sleep 3
        }
    }
    catch {
        Write-Host "Erro: Podman nao encontrado!" -ForegroundColor Red
        return
    }
    
    Write-Host "Subindo servicos..." -ForegroundColor Cyan
    podman compose up -d postgres redis pgadmin mailhog
    
    Start-Sleep 5
    Show-Services-Info
}

function Stop-Environment {
    Write-Host "Parando ambiente..." -ForegroundColor Yellow
    podman compose down
    Write-Host "Ambiente parado!" -ForegroundColor Green
}

function Restart-Environment {
    Write-Host "Reiniciando ambiente..." -ForegroundColor Cyan
    Stop-Environment
    Start-Sleep 2
    Start-Environment
}

function Show-Status {
    Write-Host "Status do Ambiente" -ForegroundColor Yellow
    Write-Host "==================" -ForegroundColor Yellow
    
    try {
        Write-Host "`nMaquina Podman:" -ForegroundColor Cyan
        $machineInfo = podman machine inspect --format "Estado: {{.State}} | CPUs: {{.Resources.CPUs}} | RAM: {{.Resources.Memory}}" 2>$null
        if ($machineInfo) {
            Write-Host "   $machineInfo" -ForegroundColor White
        } else {
            Write-Host "   Nao inicializada" -ForegroundColor Red
        }
        
        Write-Host "`nContainers:" -ForegroundColor Cyan
        podman ps --format "table {{.Names}}`t{{.Status}}`t{{.Ports}}" | Select-String "admbeachapp|NAMES"
        
        Write-Host "`nVolumes:" -ForegroundColor Cyan
        podman volume ls | Select-String "admbeachapp|DRIVER"
        
    }
    catch {
        Write-Host "Erro ao obter status: $_" -ForegroundColor Red
    }
}

function Show-Logs {
    Write-Host "Logs dos servicos:" -ForegroundColor Cyan
    podman compose logs --tail=50
}

function Open-Shell {
    Write-Host "Conectando ao PostgreSQL..." -ForegroundColor Cyan
    Write-Host "Para sair: digite \q e pressione Enter" -ForegroundColor Yellow
    Write-Host "-----------------------------------------" -ForegroundColor Gray
    
    try {
        podman exec -it admbeachapp-postgres psql -U admbeach -d admbeachapp
    }
    catch {
        Write-Host "Erro ao conectar: $_" -ForegroundColor Red
        Write-Host "Certifique-se que o ambiente esta rodando: .\admbeach-simple.ps1 start" -ForegroundColor Yellow
    }
}

function Open-WebInterfaces {
    Write-Host "Abrindo interfaces web..." -ForegroundColor Cyan
    
    $urls = @(
        "http://localhost:8080",  # pgAdmin
        "http://localhost:8025"   # MailHog
    )
    
    foreach ($url in $urls) {
        try {
            Start-Process $url
            Write-Host "Abrindo: $url" -ForegroundColor Green
        }
        catch {
            Write-Host "Erro ao abrir: $url" -ForegroundColor Red
        }
    }
    
    Write-Host "`nCredenciais:" -ForegroundColor Yellow
    Write-Host "   pgAdmin: dev@admbeachapp.com / dev123456" -ForegroundColor Gray
}

function Clean-Environment {
    Write-Host "Limpando containers parados..." -ForegroundColor Cyan
    
    try {
        $stopped = podman ps -aq --filter status=exited
        if ($stopped) {
            podman rm $stopped
            Write-Host "Containers removidos!" -ForegroundColor Green
        } else {
            Write-Host "Nenhum container parado encontrado." -ForegroundColor Green
        }
    }
    catch {
        Write-Host "Erro durante limpeza: $_" -ForegroundColor Red
    }
}

function Show-Services-Info {
    Write-Host @"

AMBIENTE PRONTO!

========================================================
                SERVICOS DISPONIVEIS
========================================================
PostgreSQL    localhost:5432
   Database:  admbeachapp
   User:      admbeach
   Password:  dev123456

Redis         localhost:6379

pgAdmin       http://localhost:8080
   Email:     dev@admbeachapp.com
   Password:  dev123456

MailHog       http://localhost:8025
   SMTP:      localhost:1025
========================================================

PROXIMOS PASSOS:
   .\admbeach-simple.ps1 shell   # Conectar ao PostgreSQL
   .\admbeach-simple.ps1 open    # Abrir pgAdmin e MailHog
   .\admbeach-simple.ps1 logs    # Ver logs dos servicos

"@ -ForegroundColor White
}

# Execucao principal
switch ($Command.ToLower()) {
    "start"   { Start-Environment }
    "stop"    { Stop-Environment }
    "restart" { Restart-Environment }
    "status"  { Show-Status }
    "logs"    { Show-Logs }
    "shell"   { Open-Shell }
    "open"    { Open-WebInterfaces }
    "clean"   { Clean-Environment }
    "help"    { Show-Help }
    default   { 
        Write-Host "Comando nao reconhecido: '$Command'" -ForegroundColor Red
        Show-Help 
    }
}
