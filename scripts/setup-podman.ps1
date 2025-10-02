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
    Write-Host @"
=== AdmBeachApp - Setup Podman ===

USAGE:
    .\scripts\setup-podman.ps1 [-Install] [-Init] [-Start] [-Stop] [-Status] [-Clean] [-Help]

OPÇÕES:
    -Install    Instala o Podman via Chocolatey (requer admin)
    -Init       Inicializa a máquina Podman
    -Start      Inicia os serviços de desenvolvimento
    -Stop       Para todos os containers
    -Status     Mostra o status dos containers
    -Clean      Remove todos os containers e volumes
    -Help       Mostra esta ajuda

EXEMPLOS:
    # Primeira vez (requer admin):
    .\scripts\setup-podman.ps1 -Install -Init

    # Iniciar ambiente de desenvolvimento:
    .\scripts\setup-podman.ps1 -Start

    # Ver status:
    .\scripts\setup-podman.ps1 -Status

    # Parar tudo:
    .\scripts\setup-podman.ps1 -Stop
"@
}

function Test-PodmanInstalled {
    try {
        $version = podman --version
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
        Write-Error "Este comando requer privilégios de administrador. Execute o PowerShell como Administrador."
        return
    }

    # Verifica se Chocolatey está instalado
    try {
        choco --version | Out-Null
    }
    catch {
        Write-Host "Instalando Chocolatey..." -ForegroundColor Yellow
        Set-ExecutionPolicy Bypass -Scope Process -Force
        [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072
        iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
    }

    Write-Host "Instalando Podman via Chocolatey..." -ForegroundColor Yellow
    choco install podman-desktop -y
    
    Write-Host "✓ Podman instalado com sucesso!" -ForegroundColor Green
    Write-Host "Reinicie o terminal e execute: .\scripts\setup-podman.ps1 -Init" -ForegroundColor Cyan
}

function Initialize-Podman {
    Write-Host "=== Inicializando Podman ===" -ForegroundColor Yellow
    
    if (!(Test-PodmanInstalled)) {
        Write-Error "Podman não está instalado. Execute com -Install primeiro."
        return
    }

    try {
        # Inicializa a máquina podman
        Write-Host "Inicializando máquina Podman..." -ForegroundColor Cyan
        podman machine init --cpus=4 --memory=8192 --disk-size=50
        
        # Inicia a máquina
        Write-Host "Iniciando máquina Podman..." -ForegroundColor Cyan
        podman machine start
        
        Write-Host "✓ Podman inicializado com sucesso!" -ForegroundColor Green
    }
    catch {
        Write-Host "Máquina Podman já pode estar inicializada. Tentando iniciar..." -ForegroundColor Yellow
        try {
            podman machine start
            Write-Host "✓ Máquina Podman iniciada!" -ForegroundColor Green
        }
        catch {
            Write-Host "⚠ Erro ao iniciar máquina Podman: $_" -ForegroundColor Yellow
        }
    }
}

function Start-Services {
    Write-Host "=== Iniciando Serviços de Desenvolvimento ===" -ForegroundColor Yellow
    
    if (!(Test-PodmanInstalled)) {
        Write-Error "Podman não está instalado."
        return
    }

    try {
        # Verifica se podman-compose está disponível
        try {
            podman-compose --version | Out-Null
            $composeCmd = "podman-compose"
        }
        catch {
            Write-Host "podman-compose não encontrado, usando podman compose..." -ForegroundColor Yellow
            $composeCmd = "podman compose"
        }

        Write-Host "Iniciando containers..." -ForegroundColor Cyan
        Invoke-Expression "$composeCmd up -d postgres redis pgadmin mailhog"
        
        Write-Host "✓ Serviços iniciados!" -ForegroundColor Green
        Write-Host @"

=== SERVIÇOS DISPONÍVEIS ===
PostgreSQL:     localhost:5432
  - Database:   admbeachapp
  - User:       admbeach
  - Password:   dev123456

Redis:          localhost:6379
pgAdmin:        http://localhost:8080
  - Email:      dev@admbeachapp.com
  - Password:   dev123456

MailHog:        http://localhost:8025
  - SMTP:       localhost:1025
"@
    }
    catch {
        Write-Error "Erro ao iniciar serviços: $_"
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
        
        Write-Host "✓ Serviços parados!" -ForegroundColor Green
    }
    catch {
        Write-Error "Erro ao parar serviços: $_"
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
        Write-Error "Erro ao obter status: $_"
    }
}

function Clean-All {
    Write-Host "=== Limpeza Completa ===" -ForegroundColor Red
    
    $confirm = Read-Host "Isso irá remover TODOS os containers e volumes. Continuar? (y/N)"
    if ($confirm -ne "y" -and $confirm -ne "Y") {
        Write-Host "Operação cancelada." -ForegroundColor Yellow
        return
    }
    
    try {
        Write-Host "Parando todos os containers..." -ForegroundColor Cyan
        podman stop $(podman ps -aq) 2>$null
        
        Write-Host "Removendo containers..." -ForegroundColor Cyan
        podman rm $(podman ps -aq) 2>$null
        
        Write-Host "Removendo volumes..." -ForegroundColor Cyan
        podman volume prune -f
        
        Write-Host "Removendo imagens não utilizadas..." -ForegroundColor Cyan
        podman image prune -a -f
        
        Write-Host "✓ Limpeza concluída!" -ForegroundColor Green
    }
    catch {
        Write-Error "Erro durante limpeza: $_"
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
