# Script para baixar e configurar JDK portátil
param(
    [string]$JdkVersion = "17",
    [string]$InstallPath = "$PWD\jdk"
)

Write-Host "=== Configurando JDK Portátil para Android Build ===" -ForegroundColor Green

$jdkUrl = switch ($JdkVersion) {
    "17" { "https://download.java.net/java/GA/jdk17.0.2/dfd4a8d0985749f896bed50d7138ee7f/8/GPL/openjdk-17.0.2_windows-x64_bin.zip" }
    "21" { "https://download.java.net/java/GA/jdk21.0.2/f2283984656d49d69e91c558476027ac/13/GPL/openjdk-21.0.2_windows-x64_bin.zip" }
    default { "https://download.java.net/java/GA/jdk17.0.2/dfd4a8d0985749f896bed50d7138ee7f/8/GPL/openjdk-17.0.2_windows-x64_bin.zip" }
}

$zipFile = "$env:TEMP\openjdk-$JdkVersion.zip"

# Verificar se já existe
if (Test-Path "$InstallPath\bin\java.exe") {
    Write-Host "JDK já configurado em: $InstallPath" -ForegroundColor Green
    $env:JAVA_HOME = $InstallPath
    Write-Host "JAVA_HOME: $env:JAVA_HOME" -ForegroundColor Green
    return
}

try {
    Write-Host "Baixando OpenJDK $JdkVersion..." -ForegroundColor Yellow
    Invoke-WebRequest -Uri $jdkUrl -OutFile $zipFile -UseBasicParsing
    
    Write-Host "Extraindo JDK..." -ForegroundColor Yellow
    if (Test-Path $InstallPath) {
        Remove-Item -Path $InstallPath -Recurse -Force
    }
    
    # Criar diretório temporário para extração
    $tempExtract = "$env:TEMP\jdk-extract-$JdkVersion"
    if (Test-Path $tempExtract) {
        Remove-Item -Path $tempExtract -Recurse -Force
    }
    
    Expand-Archive -Path $zipFile -DestinationPath $tempExtract
    
    # Mover para o local final (OpenJDK normalmente cria uma pasta com nome da versão)
    $extractedFolder = Get-ChildItem -Path $tempExtract -Directory | Select-Object -First 1
    Move-Item -Path $extractedFolder.FullName -Destination $InstallPath
    
    # Limpeza
    Remove-Item -Path $zipFile -Force -ErrorAction SilentlyContinue
    Remove-Item -Path $tempExtract -Recurse -Force -ErrorAction SilentlyContinue
    
    # Verificar instalação
    if (Test-Path "$InstallPath\bin\java.exe") {
        Write-Host "✅ JDK instalado com sucesso!" -ForegroundColor Green
        $env:JAVA_HOME = $InstallPath
        Write-Host "JAVA_HOME configurado para: $env:JAVA_HOME" -ForegroundColor Green
        
        # Testar Java
        $javaVersion = & "$InstallPath\bin\java.exe" -version 2>&1
        Write-Host "Versão do Java:" -ForegroundColor Yellow
        Write-Host $javaVersion -ForegroundColor Gray
    } else {
        throw "Falha na instalação do JDK"
    }
}
catch {
    Write-Host "❌ Erro ao configurar JDK: $_" -ForegroundColor Red
    exit 1
}
