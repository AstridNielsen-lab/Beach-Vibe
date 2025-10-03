# Script de controle do emulador Android
param(
    [Parameter(Position=0)]
    [string]$Command = "help"
)

$adbPath = "$env:ANDROID_SDK_ROOT\platform-tools\adb.exe"

function Show-Help {
    Write-Host @"

=== Controle do Emulador Android - Quiosque da Praia ===

COMANDOS DISPONÍVEIS:

   status      Mostra status do emulador
   logs        Mostra logs da aplicacao
   screenshot  Tira screenshot do emulador  
   install     Reinstala o APK
   uninstall   Remove o app do emulador
   restart     Reinicia o app
   kill        Fecha o emulador
   shell       Abre shell ADB
   help        Mostra esta ajuda

EXEMPLOS:
   .\controlar-emulador.ps1 status
   .\controlar-emulador.ps1 logs
   .\controlar-emulador.ps1 screenshot

"@ -ForegroundColor Cyan
}

function Show-Status {
    Write-Host "=== Status do Emulador ===" -ForegroundColor Yellow
    
    # Verificar dispositivos conectados
    Write-Host "`nDispositivos conectados:" -ForegroundColor Cyan
    & $adbPath devices
    
    # Verificar se o app esta instalado
    Write-Host "`nApps instalados (Quiosque):" -ForegroundColor Cyan
    $packages = & $adbPath shell pm list packages | Select-String "quiosque"
    if ($packages) {
        Write-Host $packages -ForegroundColor Green
    } else {
        Write-Host "App nao encontrado" -ForegroundColor Red
    }
    
    # Info do sistema
    Write-Host "`nInfo do sistema:" -ForegroundColor Cyan
    $apiLevel = & $adbPath shell getprop ro.build.version.sdk
    $version = & $adbPath shell getprop ro.build.version.release
    Write-Host "Android $version (API $apiLevel)" -ForegroundColor White
}

function Show-Logs {
    Write-Host "=== Logs da Aplicacao (pressione Ctrl+C para parar) ===" -ForegroundColor Yellow
    Write-Host "Filtrando logs do Quiosque da Praia..." -ForegroundColor Gray
    
    try {
        & $adbPath logcat -v time | Select-String -Pattern "quiosque|cliente|maui|xamarin" -CaseSensitive:$false
    }
    catch {
        Write-Host "Logs interrompidos" -ForegroundColor Yellow
    }
}

function Take-Screenshot {
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $screenshotPath = "screenshot_$timestamp.png"
    
    Write-Host "Tirando screenshot..." -ForegroundColor Yellow
    
    try {
        # Tirar screenshot no dispositivo
        & $adbPath shell screencap -p /sdcard/screenshot.png
        
        # Baixar para o PC
        & $adbPath pull /sdcard/screenshot.png $screenshotPath
        
        # Limpar do dispositivo
        & $adbPath shell rm /sdcard/screenshot.png
        
        Write-Host "Screenshot salvo: $screenshotPath" -ForegroundColor Green
    }
    catch {
        Write-Host "Erro ao tirar screenshot: $_" -ForegroundColor Red
    }
}

function Install-App {
    $apkPath = "BeachApp_Cliente\bin\Release\net9.0-android\publish\com.likelook.quiosque.cliente-Signed.apk"
    
    if (-not (Test-Path $apkPath)) {
        Write-Host "APK nao encontrado: $apkPath" -ForegroundColor Red
        return
    }
    
    Write-Host "Reinstalando APK..." -ForegroundColor Yellow
    
    try {
        & $adbPath install -r $apkPath
        Write-Host "APK reinstalado com sucesso!" -ForegroundColor Green
    }
    catch {
        Write-Host "Erro ao instalar: $_" -ForegroundColor Red
    }
}

function Uninstall-App {
    Write-Host "Removendo app..." -ForegroundColor Yellow
    
    try {
        & $adbPath uninstall com.likelook.quiosque.cliente
        Write-Host "App removido com sucesso!" -ForegroundColor Green
    }
    catch {
        Write-Host "Erro ao remover: $_" -ForegroundColor Red
    }
}

function Restart-App {
    Write-Host "Reiniciando app..." -ForegroundColor Yellow
    
    try {
        # Forcar parada
        & $adbPath shell am force-stop com.likelook.quiosque.cliente
        Start-Sleep 2
        
        # Iniciar novamente
        & $adbPath shell am start -n "com.likelook.quiosque.cliente/crc6489b3d5b9a4c40f9.MainActivity"
        Write-Host "App reiniciado!" -ForegroundColor Green
    }
    catch {
        Write-Host "Erro ao reiniciar: $_" -ForegroundColor Red
    }
}

function Kill-Emulator {
    Write-Host "Fechando emulador..." -ForegroundColor Yellow
    
    try {
        & $adbPath emu kill
        Write-Host "Emulador fechado!" -ForegroundColor Green
    }
    catch {
        Write-Host "Erro ao fechar emulador: $_" -ForegroundColor Red
    }
}

function Open-Shell {
    Write-Host "Abrindo shell ADB..." -ForegroundColor Yellow
    Write-Host "Digite 'exit' para sair" -ForegroundColor Gray
    
    & $adbPath shell
}

# Verificar se ADB existe
if (-not (Test-Path $adbPath)) {
    Write-Host "ERRO: ADB nao encontrado em: $adbPath" -ForegroundColor Red
    exit 1
}

# Executar comando
switch ($Command.ToLower()) {
    "status"     { Show-Status }
    "logs"       { Show-Logs }
    "screenshot" { Take-Screenshot }
    "install"    { Install-App }
    "uninstall"  { Uninstall-App }
    "restart"    { Restart-App }
    "kill"       { Kill-Emulator }
    "shell"      { Open-Shell }
    "help"       { Show-Help }
    default      { 
        Write-Host "Comando nao reconhecido: '$Command'" -ForegroundColor Red
        Show-Help 
    }
}
