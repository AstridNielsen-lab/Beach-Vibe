# Script para configurar o ambiente Android para .NET MAUI
# Execute como Administrador para instalar as dependências necessárias

Write-Host "=== Configuração do Ambiente Android para .NET MAUI ===" -ForegroundColor Green

# Instalar Java JDK
Write-Host "Instalando Microsoft OpenJDK 17..." -ForegroundColor Yellow
try {
    winget install Microsoft.OpenJDK.17 --accept-source-agreements --accept-package-agreements
    Write-Host "JDK instalado com sucesso!" -ForegroundColor Green
} catch {
    Write-Host "Erro ao instalar JDK. Instale manualmente em: https://learn.microsoft.com/java/openjdk/download" -ForegroundColor Red
}

# Configurar variáveis de ambiente
$androidSdkPath = "$env:LOCALAPPDATA\Android\Sdk"
$javaPath = "${env:ProgramFiles}\Microsoft\jdk-17.*"

Write-Host "Configurando variáveis de ambiente..." -ForegroundColor Yellow

# Android SDK
if (Test-Path $androidSdkPath) {
    [Environment]::SetEnvironmentVariable("ANDROID_SDK_ROOT", $androidSdkPath, "User")
    [Environment]::SetEnvironmentVariable("ANDROID_HOME", $androidSdkPath, "User")
    Write-Host "Android SDK configurado: $androidSdkPath" -ForegroundColor Green
} else {
    Write-Host "Android SDK não encontrado. Instale o Android Studio ou o SDK standalone." -ForegroundColor Red
}

# Java JDK
$jdkPaths = Get-ChildItem -Path "${env:ProgramFiles}\Microsoft\" -Name "jdk-*" -ErrorAction SilentlyContinue
if ($jdkPaths) {
    $jdkPath = "${env:ProgramFiles}\Microsoft\" + $jdkPaths[0]
    [Environment]::SetEnvironmentVariable("JAVA_HOME", $jdkPath, "User")
    Write-Host "Java JDK configurado: $jdkPath" -ForegroundColor Green
} else {
    Write-Host "Java JDK não encontrado automaticamente. Configure manualmente JAVA_HOME." -ForegroundColor Red
}

Write-Host "=== Configuração Concluída ===" -ForegroundColor Green
Write-Host "Reinicie o PowerShell e execute 'dotnet build' no diretório do projeto." -ForegroundColor Yellow
