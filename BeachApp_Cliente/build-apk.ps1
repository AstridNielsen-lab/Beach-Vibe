# Script para gerar APK do Quiosque da Praia - Cliente
# Execute após configurar o ambiente com setup-android.ps1

param(
    [string]$Configuration = "Release"
)

Write-Host "=== Gerando APK do Quiosque da Praia - Cliente ===" -ForegroundColor Green
Write-Host "Configuração: $Configuration" -ForegroundColor Yellow

# Verificar se as variáveis de ambiente estão configuradas
if (-not $env:ANDROID_SDK_ROOT) {
    Write-Host "ANDROID_SDK_ROOT não está configurado. Execute setup-android.ps1 primeiro." -ForegroundColor Red
    exit 1
}

if (-not $env:JAVA_HOME) {
    Write-Host "JAVA_HOME não está configurado. Execute setup-android.ps1 primeiro." -ForegroundColor Red
    exit 1
}

Write-Host "Android SDK: $env:ANDROID_SDK_ROOT" -ForegroundColor Green
Write-Host "Java JDK: $env:JAVA_HOME" -ForegroundColor Green

# Limpar builds anteriores
Write-Host "Limpando builds anteriores..." -ForegroundColor Yellow
dotnet clean

# Restaurar dependências
Write-Host "Restaurando dependências..." -ForegroundColor Yellow
dotnet restore

# Build do projeto
Write-Host "Compilando projeto..." -ForegroundColor Yellow
dotnet build -c $Configuration -f net9.0-android

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro na compilação. Verifique o log de erros acima." -ForegroundColor Red
    exit 1
}

# Publicar APK
Write-Host "Gerando APK..." -ForegroundColor Yellow
dotnet publish -c $Configuration -f net9.0-android

if ($LASTEXITCODE -eq 0) {
    Write-Host "=== APK Gerado com Sucesso! ===" -ForegroundColor Green
    
    $outputPath = "bin\$Configuration\net9.0-android\publish\"
    Write-Host "Local do APK: $outputPath" -ForegroundColor Green
    
    # Tentar encontrar o APK gerado
    $apkFiles = Get-ChildItem -Path $outputPath -Name "*.apk" -ErrorAction SilentlyContinue
    if ($apkFiles) {
        Write-Host "Arquivo APK: $($apkFiles[0])" -ForegroundColor Green
    }
    
    Write-Host "Você pode instalar o APK em um dispositivo Android ou emulador." -ForegroundColor Yellow
} else {
    Write-Host "Erro ao gerar APK. Verifique os logs acima." -ForegroundColor Red
    exit 1
}
