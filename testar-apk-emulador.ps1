# Script para testar APK no emulador Android
param(
    [string]$AvdName = "QuiosqueApp",
    [string]$ApkPath = "BeachApp_Cliente\bin\Release\net9.0-android\publish\com.likelook.quiosque.cliente-Signed.apk"
)

Write-Host "=== Testando APK do Quiosque da Praia no Emulador ===" -ForegroundColor Green
Write-Host "Emulador: $AvdName" -ForegroundColor Cyan
Write-Host "APK: $ApkPath" -ForegroundColor Cyan

# Verificar se o APK existe
if (-not (Test-Path $ApkPath)) {
    Write-Host "ERRO: APK nao encontrado em: $ApkPath" -ForegroundColor Red
    Write-Host "Execute o build primeiro: cd BeachApp_Cliente; .\build-apk-simple.ps1" -ForegroundColor Yellow
    exit 1
}

# Configurar caminhos do Android SDK
$emulatorPath = "$env:ANDROID_SDK_ROOT\emulator\emulator.exe"
$adbPath = "$env:ANDROID_SDK_ROOT\platform-tools\adb.exe"

# Verificar ferramentas
if (-not (Test-Path $emulatorPath)) {
    Write-Host "ERRO: Emulator nao encontrado em: $emulatorPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $adbPath)) {
    Write-Host "ERRO: ADB nao encontrado em: $adbPath" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "1. Iniciando emulador Android..." -ForegroundColor Yellow
Write-Host "   Emulador: $AvdName" -ForegroundColor Gray

# Iniciar emulador em background
$emulatorProcess = Start-Process -FilePath $emulatorPath -ArgumentList @("-avd", $AvdName, "-no-snapshot-save") -PassThru -WindowStyle Normal

Write-Host "   PID do emulador: $($emulatorProcess.Id)" -ForegroundColor Gray
Write-Host "   Aguardando inicializacao completa..." -ForegroundColor Gray

# Aguardar o emulador inicializar
$maxWaitTime = 120  # 2 minutos
$waitTime = 0
$bootCompleted = $false

do {
    Start-Sleep 5
    $waitTime += 5
    
    # Verificar se o device esta online
    try {
        $devices = & $adbPath devices 2>$null
        if ($devices -match "emulator.*device") {
            # Verificar se o boot foi completado
            $bootStatus = & $adbPath shell getprop sys.boot_completed 2>$null
            if ($bootStatus -eq "1") {
                $bootCompleted = $true
                Write-Host "   Emulador inicializado com sucesso!" -ForegroundColor Green
            }
        }
    }
    catch {
        # Ignorar erros durante inicializacao
    }
    
    if ($waitTime % 15 -eq 0) {
        Write-Host "   Aguardando... ($waitTime/$maxWaitTime segundos)" -ForegroundColor Gray
    }
    
} while (-not $bootCompleted -and $waitTime -lt $maxWaitTime)

if (-not $bootCompleted) {
    Write-Host "TIMEOUT: Emulador nao inicializou em $maxWaitTime segundos" -ForegroundColor Red
    Write-Host "Tente manualmente: emulator -avd $AvdName" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "2. Instalando APK..." -ForegroundColor Yellow
Write-Host "   APK: $(Split-Path $ApkPath -Leaf)" -ForegroundColor Gray

try {
    $installResult = & $adbPath install -r $ApkPath 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   APK instalado com sucesso!" -ForegroundColor Green
    } else {
        Write-Host "ERRO na instalacao:" -ForegroundColor Red
        Write-Host $installResult -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "ERRO ao instalar APK: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "3. Abrindo aplicativo..." -ForegroundColor Yellow

# Abrir o app
try {
    & $adbPath shell am start -n "com.likelook.quiosque.cliente/crc6489b3d5b9a4c40f9.MainActivity" 2>$null
    Write-Host "   App 'Quiosque da Praia' aberto no emulador!" -ForegroundColor Green
}
catch {
    Write-Host "Aviso: Nao foi possivel abrir automaticamente." -ForegroundColor Yellow
    Write-Host "Abra manualmente o app 'Quiosque da Praia' no emulador." -ForegroundColor Gray
}

Write-Host ""
Write-Host "=== TESTE CONCLUIDO! ===" -ForegroundColor Green
Write-Host ""
Write-Host "EMULADOR RODANDO:" -ForegroundColor Cyan
Write-Host "- O emulador Android esta ativo" -ForegroundColor White
Write-Host "- APK instalado: Quiosque da Praia" -ForegroundColor White
Write-Host "- Teste a aplicacao no emulador" -ForegroundColor White
Write-Host ""
Write-Host "PROXIMOS PASSOS:" -ForegroundColor Yellow
Write-Host "1. Teste o login com nome/CPF" -ForegroundColor White  
Write-Host "2. Navegue pelo cardapio" -ForegroundColor White
Write-Host "3. Teste funcionalidades" -ForegroundColor White
Write-Host "4. Para fechar: adb emu kill" -ForegroundColor Gray
Write-Host ""
Write-Host "COMANDOS UTEIS:" -ForegroundColor Yellow
Write-Host "- Ver logs: adb logcat | Select-String 'QuiosqueDaPraia'" -ForegroundColor Gray
Write-Host "- Reinstalar: adb install -r `"$ApkPath`"" -ForegroundColor Gray
Write-Host "- Desinstalar: adb uninstall com.likelook.quiosque.cliente" -ForegroundColor Gray
