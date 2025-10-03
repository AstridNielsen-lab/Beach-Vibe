# Script simples para gerar APK do BeachApp Cliente
param(
    [string]$Configuration = "Release"
)

Write-Host "=== Gerando APK do Quiosque da Praia - Cliente ===" -ForegroundColor Green
Write-Host "Configuracao: $Configuration" -ForegroundColor Yellow

# Verificar se JDK esta configurado
if (-not $env:JAVA_HOME) {
    Write-Host "Configurando JDK portatil..." -ForegroundColor Yellow
    .\setup-jdk-portable.ps1
}

Write-Host "JAVA_HOME: $env:JAVA_HOME" -ForegroundColor Green

# Limpar builds anteriores
Write-Host "Limpando builds anteriores..." -ForegroundColor Yellow
dotnet clean -c $Configuration

# Restaurar dependencias
Write-Host "Restaurando dependencias..." -ForegroundColor Yellow
dotnet restore

# Build do projeto
Write-Host "Compilando projeto..." -ForegroundColor Yellow
dotnet build -c $Configuration -f net9.0-android

if ($LASTEXITCODE -eq 0) {
    # Publicar APK
    Write-Host "Gerando APK..." -ForegroundColor Yellow
    dotnet publish -c $Configuration -f net9.0-android
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "=== APK Gerado com Sucesso! ===" -ForegroundColor Green
        
        # Encontrar o APK gerado
        $publishDir = "bin\$Configuration\net9.0-android\publish"
        if (Test-Path $publishDir) {
            Write-Host "Diretorio de publicacao: $publishDir" -ForegroundColor Green
            
            $apkFiles = Get-ChildItem -Path $publishDir -Recurse -Name "*.apk" -ErrorAction SilentlyContinue
            if ($apkFiles) {
                foreach ($apk in $apkFiles) {
                    $fullPath = Join-Path $publishDir $apk
                    Write-Host "APK: $apk" -ForegroundColor Green
                    Write-Host "Local: $fullPath" -ForegroundColor Cyan
                }
            } else {
                Write-Host "APK nao encontrado no diretorio de publicacao" -ForegroundColor Yellow
                Write-Host "Listando conteudo do diretorio publish:" -ForegroundColor Gray
                Get-ChildItem -Path $publishDir -Recurse
            }
        }
        
        Write-Host ""
        Write-Host "Proximos passos:" -ForegroundColor Cyan
        Write-Host "1. Transfira o APK para um dispositivo Android" -ForegroundColor White
        Write-Host "2. Habilite 'Origens desconhecidas' nas configuracoes" -ForegroundColor White
        Write-Host "3. Instale o APK" -ForegroundColor White
        Write-Host "4. Teste a aplicacao!" -ForegroundColor White
    } else {
        Write-Host "Erro ao gerar APK" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Erro na compilacao" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "BUILD CONCLUIDO COM SUCESSO!" -ForegroundColor Green
