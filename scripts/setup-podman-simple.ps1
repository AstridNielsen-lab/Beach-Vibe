# Setup-Podman.ps1 - Script para configurar Podman no Windows para AdmBeachApp
# Execute com: .\scripts\setup-podman-simple.ps1

param(
    [switch]$Install,
    [switch]$Init,
    [switch]$Start,
    [switch]$Stop,
    [switch]$Status,
    [switch]$Clean,
    [switch]$Help
)

$ErrorActionPreference = "Continue"

function Show-Help {
    Write-Host "=== AdmBeachApp - Setup Podman ===" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "USAGE:" -ForegroundColor Yellow
    Write-Host "    .\scripts\setup-podman-simple.ps1 [-Install] [-Init] [-Start] [-Stop] [-Status] [-Clean] [-Help]"
    Write-Host ""
    Write-Host "OPCOES:" -ForegroundColor Yellow
    Write-Host "    -Install    Instala o Podman via Chocolatey (requer admin)"
    Write-Host "    -Init       Inicializa a maquina Podman"
    Write-Host "    -Start      Inicia os servicos de desenvolvimento"
    Write-Host "    -Stop       Para todos os containers"
    Write-Host "    -Status     Mostra o status dos containers"
    Write-Host "    -Clean      Remove todos os containers e volumes"
    Write-Host "    -Help       Mostra esta ajuda"
    Write-Host ""
    Write-Host "EXEMPLOS:" -ForegroundColor Yellow
    Write-Host "    # Primeira vez (requer admin):"
    Write-Host "    .\scripts\setup-podman-simple.ps1 -Install -Init"
    Write-Host ""
    Write-Host "    # Iniciar ambiente de desenvolvimento:"
    Write-Host "    .\scripts\setup-podman-simple.ps1 -Start"
}

function Test-PodmanInstalled {
    try {
        $null = podman --version 2>$null
        Write-Host "[OK] Podman ja instalado" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "[ERRO] Podman nao encontrado" -ForegroundColor Red
        return $false
    }
}

function Install-Podman {
    Write-Host "=== Instalando Podman ===" -ForegroundColor Yellow
    
    # Verifica se tem privilegios de administrador
    $isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
    
    if (-NOT $isAdmin) {
        Write-Host "[ERRO] Este comando requer privilegios de administrador." -ForegroundColor Red
        Write-Host "[INFO] Execute o PowerShell como Administrador e tente novamente." -ForegroundColor Yellow
        return
    }

    # Verifica se Chocolatey esta instalado
    try {
        $null = choco --version 2>$null
        Write-Host "[OK] Chocolatey ja instalado" -ForegroundColor Green
    }
    catch {
        Write-Host "[INFO] Instalando Chocolatey..." -ForegroundColor Yellow
        Set-ExecutionPolicy Bypass -Scope Process -Force
        [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072
        iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
        Write-Host "[OK] Chocolatey instalado!" -ForegroundColor Green
    }

    Write-Host "[INFO] Instalando Podman via Chocolatey..." -ForegroundColor Yellow
    choco install podman-desktop -y
    
    Write-Host "[OK] Podman instalado com sucesso!" -ForegroundColor Green
    Write-Host "[INFO] Reinicie o terminal e execute: .\scripts\setup-podman-simple.ps1 -Init" -ForegroundColor Cyan
}

function Initialize-Podman {
    Write-Host "=== Inicializando Podman ===" -ForegroundColor Yellow
    
    if (!(Test-PodmanInstalled)) {
        Write-Host "[ERRO] Podman nao esta instalado. Execute com -Install primeiro." -ForegroundColor Red
        return
    }

    try {
        Write-Host "[INFO] Inicializando maquina Podman..." -ForegroundColor Cyan
        podman machine init --cpus=4 --memory=8192 --disk-size=50
        
        Write-Host "[INFO] Iniciando maquina Podman..." -ForegroundColor Cyan
        podman machine start
        
        Write-Host "[OK] Podman inicializado com sucesso!" -ForegroundColor Green
    }
    catch {
        Write-Host "[WARN] Maquina Podman ja pode estar inicializada. Tentando iniciar..." -ForegroundColor Yellow
        try {
            podman machine start
            Write-Host "[OK] Maquina Podman iniciada!" -ForegroundColor Green
        }
        catch {
            Write-Host "[ERRO] Erro ao iniciar maquina Podman: $_" -ForegroundColor Red
        }
    }
}

function Start-Services {
    Write-Host "=== Iniciando Servicos de Desenvolvimento ===" -ForegroundColor Yellow
    
    if (!(Test-PodmanInstalled)) {
        Write-Host "[ERRO] Podman nao esta instalado." -ForegroundColor Red
        return
    }

    try {
        # Verifica se podman-compose esta disponivel
        try {
            $null = podman-compose --version 2>$null
            $composeCmd = "podman-compose"
            Write-Host "[INFO] Usando podman-compose" -ForegroundColor Green
        }
        catch {
            Write-Host "[WARN] podman-compose nao encontrado, usando podman compose..." -ForegroundColor Yellow
            $composeCmd = "podman compose"
        }

        Write-Host "[INFO] Iniciando containers..." -ForegroundColor Cyan
        Invoke-Expression "$composeCmd up -d postgres redis pgadmin mailhog"
        
        Write-Host "[OK] Servicos iniciados!" -ForegroundColor Green
        Write-Host ""
        Write-Host "=== SERVICOS DISPONIVEIS ===" -ForegroundColor Cyan
        Write-Host "PostgreSQL:     localhost:5432" -ForegroundColor White
        Write-Host "  - Database:   admbeachapp" -ForegroundColor Gray
        Write-Host "  - User:       admbeach" -ForegroundColor Gray
        Write-Host "  - Password:   dev123456" -ForegroundColor Gray
        Write-Host ""
        Write-Host "Redis:          localhost:6379" -ForegroundColor White
        Write-Host "pgAdmin:        http://localhost:8080" -ForegroundColor White
        Write-Host "  - Email:      dev@admbeachapp.com" -ForegroundColor Gray
        Write-Host "  - Password:   dev123456" -ForegroundColor Gray
        Write-Host ""
        Write-Host "MailHog:        http://localhost:8025" -ForegroundColor White
        Write-Host "  - SMTP:       localhost:1025" -ForegroundColor Gray
    }
    catch {
        Write-Host "[ERRO] Erro ao iniciar servicos: $_" -ForegroundColor Red
    }
}

function Stop-Services {
    Write-Host "=== Parando Servicos ===" -ForegroundColor Yellow
    
    try {
        try {
            podman-compose down 2>$null
        }
        catch {
            podman compose down 2>$null
        }
        
        Write-Host "[OK] Servicos parados!" -ForegroundColor Green
    }
    catch {
        Write-Host "[ERRO] Erro ao parar servicos: $_" -ForegroundColor Red
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
        Write-Host "[ERRO] Erro ao obter status: $_" -ForegroundColor Red
    }
}

function Clean-All {
    Write-Host "=== Limpeza Completa ===" -ForegroundColor Red
    
    $confirm = Read-Host "[WARN] Isso ira remover TODOS os containers e volumes. Continuar? (y/N)"
    if ($confirm -ne "y" -and $confirm -ne "Y") {
        Write-Host "[INFO] Operacao cancelada." -ForegroundColor Yellow
        return
    }
    
    try {
        Write-Host "[INFO] Parando todos os containers..." -ForegroundColor Cyan
        $containers = podman ps -aq 2>$null
        if ($containers) {
            podman stop $containers 2>$null
        }
        
        Write-Host "[INFO] Removendo containers..." -ForegroundColor Cyan
        if ($containers) {
            podman rm $containers 2>$null
        }
        
        Write-Host "[INFO] Removendo volumes..." -ForegroundColor Cyan
        podman volume prune -f 2>$null
        
        Write-Host "[INFO] Removendo imagens nao utilizadas..." -ForegroundColor Cyan
        podman image prune -a -f 2>$null
        
        Write-Host "[OK] Limpeza concluida!" -ForegroundColor Green
    }
    catch {
        Write-Host "[ERRO] Erro durante limpeza: $_" -ForegroundColor Red
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
