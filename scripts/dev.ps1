# dev.ps1 - Script simplificado para desenvolvimento diário com Podman
# Execute: .\scripts\dev.ps1 [comando]

param(
    [string]$Command = "help"
)

function Show-Commands {
    Write-Host @"
=== AdmBeachApp - Comandos de Desenvolvimento ===

COMANDOS DISPONÍVEIS:

  start     Inicia o ambiente de desenvolvimento
  stop      Para o ambiente de desenvolvimento
  restart   Reinicia o ambiente de desenvolvimento
  status    Mostra o status atual
  logs      Mostra os logs dos containers
  shell     Abre shell no container de desenvolvimento
  db        Conecta no PostgreSQL local
  clean     Remove containers parados
  help      Mostra esta ajuda

EXEMPLOS:
  .\scripts\dev.ps1 start
  .\scripts\dev.ps1 logs postgres
  .\scripts\dev.ps1 shell
"@
}

function Start-Dev {
    Write-Host "🚀 Iniciando ambiente de desenvolvimento..." -ForegroundColor Cyan
    
    try {
        # Verifica se a máquina Podman está rodando
        $machineStatus = podman machine inspect --format "{{.State}}" 2>$null
        if ($machineStatus -ne "running") {
            Write-Host "Iniciando máquina Podman..." -ForegroundColor Yellow
            podman machine start
            Start-Sleep 5
        }

        # Inicia os serviços essenciais
        podman compose up -d postgres redis pgadmin mailhog
        
        Write-Host "✅ Ambiente iniciado com sucesso!" -ForegroundColor Green
        Write-Host @"

🔗 SERVIÇOS DISPONÍVEIS:
   PostgreSQL:  localhost:5432 (admbeach/dev123456)
   Redis:       localhost:6379
   pgAdmin:     http://localhost:8080
   MailHog:     http://localhost:8025
"@
    }
    catch {
        Write-Error "❌ Erro ao iniciar ambiente: $_"
    }
}

function Stop-Dev {
    Write-Host "🛑 Parando ambiente de desenvolvimento..." -ForegroundColor Yellow
    
    try {
        podman compose down
        Write-Host "✅ Ambiente parado com sucesso!" -ForegroundColor Green
    }
    catch {
        Write-Error "❌ Erro ao parar ambiente: $_"
    }
}

function Restart-Dev {
    Write-Host "🔄 Reiniciando ambiente..." -ForegroundColor Cyan
    Stop-Dev
    Start-Sleep 2
    Start-Dev
}

function Show-Status {
    Write-Host "📊 Status do ambiente:" -ForegroundColor Cyan
    
    try {
        Write-Host "`n=== CONTAINERS ===" -ForegroundColor Yellow
        podman ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" | Where-Object { $_ -match "admbeachapp" -or $_ -match "NAMES" }
        
        Write-Host "`n=== MÁQUINA PODMAN ===" -ForegroundColor Yellow
        $machineInfo = podman machine inspect --format "State: {{.State}}, CPUs: {{.Resources.CPUs}}, Memory: {{.Resources.Memory}}" 2>$null
        if ($machineInfo) {
            Write-Host $machineInfo
        } else {
            Write-Host "Máquina não encontrada ou não inicializada" -ForegroundColor Red
        }
    }
    catch {
        Write-Error "❌ Erro ao obter status: $_"
    }
}

function Show-Logs {
    param([string]$Service = "")
    
    if ($Service -eq "") {
        Write-Host "📋 Logs de todos os serviços:" -ForegroundColor Cyan
        podman compose logs --tail=50
    }
    else {
        Write-Host "📋 Logs do serviço '$Service':" -ForegroundColor Cyan
        podman compose logs --tail=50 $Service
    }
}

function Open-Shell {
    Write-Host "🐚 Abrindo shell no container de desenvolvimento..." -ForegroundColor Cyan
    
    try {
        # Verifica se o container de desenvolvimento está rodando
        $devContainer = podman ps --filter name=admbeachapp-dev --format "{{.Names}}" --quiet
        
        if ($devContainer) {
            podman exec -it admbeachapp-dev /bin/bash
        }
        else {
            Write-Host "Container de desenvolvimento não está rodando." -ForegroundColor Yellow
            Write-Host "Iniciando container temporário..." -ForegroundColor Cyan
            podman run -it --rm --name temp-dev -v "${PWD}:/app" -w /app mcr.microsoft.com/dotnet/sdk:9.0 /bin/bash
        }
    }
    catch {
        Write-Error "❌ Erro ao abrir shell: $_"
    }
}

function Connect-Database {
    Write-Host "🗄️ Conectando ao PostgreSQL..." -ForegroundColor Cyan
    
    try {
        $pgContainer = podman ps --filter name=admbeachapp-postgres --format "{{.Names}}" --quiet
        
        if ($pgContainer) {
            Write-Host "💡 Para sair do PostgreSQL, digite: \q" -ForegroundColor Yellow
            podman exec -it admbeachapp-postgres psql -U admbeach -d admbeachapp
        }
        else {
            Write-Host "❌ Container PostgreSQL não está rodando. Execute: .\scripts\dev.ps1 start" -ForegroundColor Red
        }
    }
    catch {
        Write-Error "❌ Erro ao conectar no banco: $_"
    }
}

function Clean-Containers {
    Write-Host "🧹 Limpando containers parados..." -ForegroundColor Cyan
    
    try {
        $stoppedContainers = podman ps -aq --filter status=exited
        if ($stoppedContainers) {
            podman rm $stoppedContainers
            Write-Host "✅ Containers removidos!" -ForegroundColor Green
        }
        else {
            Write-Host "✨ Nenhum container parado encontrado." -ForegroundColor Green
        }
    }
    catch {
        Write-Error "❌ Erro durante limpeza: $_"
    }
}

# Execução principal
switch ($Command.ToLower()) {
    "start" { Start-Dev }
    "stop" { Stop-Dev }
    "restart" { Restart-Dev }
    "status" { Show-Status }
    "logs" { 
        if ($args.Count -gt 0) {
            Show-Logs -Service $args[0]
        } else {
            Show-Logs
        }
    }
    "shell" { Open-Shell }
    "db" { Connect-Database }
    "clean" { Clean-Containers }
    "help" { Show-Commands }
    default { 
        Write-Host "❌ Comando não reconhecido: $Command" -ForegroundColor Red
        Show-Commands 
    }
}
