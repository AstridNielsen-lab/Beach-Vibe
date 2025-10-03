# Setup-Podman.ps1 - Script para configurar Podman no Windows para AdmBeachApp
# Execute com: .\scripts\setup-podman.ps1

param(
    [switch]$Install,
    [switch]$Init,
    [switch]$Start,
    [switch]$Stop,
    [switch]$Status,
    [switch]$Clean,
    [switch]$Help
)

$ErrorActionPreference = "Stop"

function Show-Help {
    Write-Host "=== AdmBeachApp - Setup Podman ===" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "    .\scripts\setup-podman.ps1 [-Install] [-Init] [-Start] [-Stop] [-Status] [-Clean] [-Help]"
    Write-Host ""
    Write-Host "OPÇÕES:" -ForegroundColor Yellow
    Write-Host "    -Install    Instala o Podman via Chocolatey (requer admin)"
    Write-Host "    -Init       Inicializa a máquina Podman"
    Write-Host "    -Start      Inicia os serviços de desenvolvimento"
    Write-Host "    -Stop       Para todos os containers"
    Write-Host "    -Status     Mostra o status dos containers"
    Write-Host "    -Clean      Remove todos os containers e volumes"
    Write-Host "    -Help       Mostra esta ajuda"
    Write-Host ""
    Write-Host "EXEMPLOS:" -ForegroundColor Yellow
    Write-Host "    # Primeira vez (requer admin):"
    Write-Host "    .\scripts\setup-podman.ps1 -Install -Init"
    Write-Host ""
    Write-Host "    # Iniciar ambiente de desenvolvimento:"
    Write-Host "    .\scripts\setup-podman.ps1 -Start"
    Write-Host ""
    Write-Host "    # Ver status:"
    Write-Host "    .\scripts\setup-podman.ps1 -Status"
    Write-Host ""
    Write-Host "    # Parar tudo:"
    Write-Host "    .\scripts\setup-podman.ps1 -Stop"
}

function Test-PodmanInstalled {
    try {
        $version = podman --version 2>$null
        Write-Host "✓ Podman já instalado: $version" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "✗ Podman não encontrado" -ForegroundColor Red
        return $false
    }
}

function Install-Podman {
    Write-Host "=== Instalando Podman ===" -ForegroundColor Yellow
    
    # Verifica se tem privilégios de administrador
    if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
        Write-Host "❌ Este comando requer privilégios de administrador." -ForegroundColor Red
        Write-Host "💡 Execute o PowerShell como Administrador e tente novamente." -ForegroundColor Yellow
        return
    }

    # Verifica se Chocolatey está instalado
    try {
        choco --version | Out-Null
        Write-Host "✓ Chocolatey já instalado" -ForegroundColor Green
    }
    catch {
        Write-Host "📦 Instalando Chocolatey..." -ForegroundColor Yellow
        Set-ExecutionPolicy Bypass -Scope Process -Force
        [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072
        iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
        Write-Host "✓ Chocolatey instalado!" -ForegroundColor Green
    }

    Write-Host "🐳 Instalando Podman via Chocolatey..." -ForegroundColor Yellow
    choco install podman-desktop -y
    
    Write-Host "✓ Podman instalado com sucesso!" -ForegroundColor Green
    Write-Host "🔄 Reinicie o terminal e execute: .\scripts\setup-podman.ps1 -Init" -ForegroundColor Cyan
}

function Initialize-Podman {
    Write-Host "=== Inicializando Podman ===" -ForegroundColor Yellow
    
    if (!(Test-PodmanInstalled)) {
        Write-Host "❌ Podman não está instalado. Execute com -Install primeiro." -ForegroundColor Red
        return
    }

    try {
        # Inicializa a máquina podman
        Write-Host "🚀 Inicializando máquina Podman..." -ForegroundColor Cyan
        podman machine init --cpus=4 --memory=8192 --disk-size=50
        
        # Inicia a máquina
        Write-Host "▶️ Iniciando máquina Podman..." -ForegroundColor Cyan
        podman machine start
        
        Write-Host "✅ Podman inicializado com sucesso!" -ForegroundColor Green
    }
    catch {
        Write-Host "⚠️ Máquina Podman já pode estar inicializada. Tentando iniciar..." -ForegroundColor Yellow
        try {
            podman machine start
            Write-Host "✅ Máquina Podman iniciada!" -ForegroundColor Green
        }
        catch {
            Write-Host "❌ Erro ao iniciar máquina Podman: $_" -ForegroundColor Red
        }
    }
}

function Start-Services {
    Write-Host "=== Iniciando Serviços de Desenvolvimento ===" -ForegroundColor Yellow
    
    if (!(Test-PodmanInstalled)) {
        Write-Host "❌ Podman não está instalado." -ForegroundColor Red
        return
    }

    try {
        # Verifica se podman-compose está disponível
        try {
            podman-compose --version | Out-Null
            $composeCmd = "podman-compose"
            Write-Host "📦 Usando podman-compose" -ForegroundColor Green
        }
        catch {
            Write-Host "📦 podman-compose não encontrado, usando podman compose..." -ForegroundColor Yellow
            $composeCmd = "podman compose"
        }

        Write-Host "🚀 Iniciando containers..." -ForegroundColor Cyan
        Invoke-Expression "$composeCmd up -d postgres redis pgadmin mailhog"
        
        Write-Host "✅ Serviços iniciados!" -ForegroundColor Green
        Write-Host ""
        Write-Host "=== SERVIÇOS DISPONÍVEIS ===" -ForegroundColor Cyan
        Write-Host "🐘 PostgreSQL:     localhost:5432" -ForegroundColor White
        Write-Host "   - Database:     admbeachapp" -ForegroundColor Gray
        Write-Host "   - User:         admbeach" -ForegroundColor Gray
        Write-Host "   - Password:     dev123456" -ForegroundColor Gray
        Write-Host ""
        Write-Host "🔴 Redis:          localhost:6379" -ForegroundColor White
        Write-Host "🌐 pgAdmin:        http://localhost:8080" -ForegroundColor White
        Write-Host "   - Email:        dev@admbeachapp.com" -ForegroundColor Gray
        Write-Host "   - Password:     dev123456" -ForegroundColor Gray
        Write-Host ""
        Write-Host "📧 MailHog:        http://localhost:8025" -ForegroundColor White
        Write-Host "   - SMTP:         localhost:1025" -ForegroundColor Gray
    }
    catch {
        Write-Host "❌ Erro ao iniciar serviços: $_" -ForegroundColor Red
    }
}

function Stop-Services {
    Write-Host "=== Parando Serviços ===" -ForegroundColor Yellow
    
    try {
        try {
            podman-compose down
        }
        catch {
            podman compose down
        }
        
        Write-Host "✅ Serviços parados!" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Erro ao parar serviços: $_" -ForegroundColor Red
    }
}

function Show-Status {
    Write-Host "=== Status dos Containers ===" -ForegroundColor Yellow
    
    try {
        podman ps -a --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
        
        Write-Host "`n=== Volumes ===" -ForegroundColor Yellow
        podman volume ls
    }
    catch {
        Write-Host "❌ Erro ao obter status: $_" -ForegroundColor Red
    }
}

function Clean-All {
    Write-Host "=== Limpeza Completa ===" -ForegroundColor Red
    
    $confirm = Read-Host "⚠️ Isso irá remover TODOS os containers e volumes. Continuar? (y/N)"
    if ($confirm -ne "y" -and $confirm -ne "Y") {
        Write-Host "🚫 Operação cancelada." -ForegroundColor Yellow
        return
    }
    
    try {
        Write-Host "🛑 Parando todos os containers..." -ForegroundColor Cyan
        $containers = podman ps -aq
        if ($containers) {
            podman stop $containers
        }
        
        Write-Host "🗑️ Removendo containers..." -ForegroundColor Cyan
        if ($containers) {
            podman rm $containers
        }
        
        Write-Host "🧹 Removendo volumes..." -ForegroundColor Cyan
        podman volume prune -f
        
        Write-Host "🖼️ Removendo imagens não utilizadas..." -ForegroundColor Cyan
        podman image prune -a -f
        
        Write-Host "✅ Limpeza concluída!" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Erro durante limpeza: $_" -ForegroundColor Red
    }
}

# Main execution
if ($Help -or (!$Install -and !$Init -and !$Start -and !$Stop -and !$Status -and !$Clean)) {
    Show-Help
    exit
}

if ($Install) { Install-Podman }
if ($Init) { Initialize-Podman }
if ($Start) { Start-Services }
if ($Stop) { Stop-Services }
if ($Status) { Show-Status }
if ($Clean) { Clean-All }
