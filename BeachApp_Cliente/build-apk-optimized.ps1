# Script otimizado para gerar APK do BeachApp Cliente
param(
    [string]$Configuration = "Release"
)

Write-Host "=== Gerando APK do Quiosque da Praia - Cliente (Otimizado) ===" -ForegroundColor Green
Write-Host "Configuração: $Configuration" -ForegroundColor Yellow

# Verificar se JDK está configurado
if (-not $env:JAVA_HOME) {
    Write-Host "Configurando JDK portátil..." -ForegroundColor Yellow
    .\setup-jdk-portable.ps1
}

# Criar versão temporária sem CSS problemático
Write-Host "Preparando arquivos para build..." -ForegroundColor Yellow

# Backup dos arquivos originais
$backupDir = "temp_backup"
if (Test-Path $backupDir) {
    Remove-Item -Path $backupDir -Recurse -Force
}
New-Item -Path $backupDir -ItemType Directory

# Lista de arquivos para modificar
$filesToModify = @(
    "Components\Auth\ClientAuthGuard.razor",
    "Components\Pages\LoginCliente.razor",
    "Components\Layout\MainLayout.razor"
)

foreach ($file in $filesToModify) {
    if (Test-Path $file) {
        # Backup
        Copy-Item -Path $file -Destination "$backupDir\$(Split-Path $file -Leaf).bak"
        
        # Remover seções CSS problemáticas
        $content = Get-Content -Path $file -Raw
        
        # Remover @keyframes e @media (substitui por comentários)
        $content = $content -replace '@keyframes\s+[\w-]+\s*\{[^}]*\}', '/* @keyframes removed for build */'
        $content = $content -replace '@media\s*\([^)]*\)\s*\{[^}]*\}', '/* @media removed for build */'
        
        # Salvar versão modificada
        Set-Content -Path $file -Value $content -Encoding UTF8
        Write-Host "  Modificado: $file" -ForegroundColor Gray
    }
}

try {
    # Limpar builds anteriores
    Write-Host "Limpando builds anteriores..." -ForegroundColor Yellow
    dotnet clean -c $Configuration

    # Restaurar dependências
    Write-Host "Restaurando dependências..." -ForegroundColor Yellow
    dotnet restore

    # Build do projeto
    Write-Host "Compilando projeto..." -ForegroundColor Yellow
    $buildResult = dotnet build -c $Configuration -f net9.0-android --verbosity minimal

    if ($LASTEXITCODE -eq 0) {
        # Publicar APK
        Write-Host "Gerando APK..." -ForegroundColor Yellow
        dotnet publish -c $Configuration -f net9.0-android --verbosity minimal

        if ($LASTEXITCODE -eq 0) {
            Write-Host "=== APK Gerado com Sucesso! ===" -ForegroundColor Green
            
            # Encontrar o APK gerado
            $publishDir = "bin\$Configuration\net9.0-android\publish"
            if (Test-Path $publishDir) {
                Write-Host "Diretório de publicação: $publishDir" -ForegroundColor Green
                
                $apkFiles = Get-ChildItem -Path $publishDir -Recurse -Name "*.apk" -ErrorAction SilentlyContinue
                if ($apkFiles) {
                    foreach ($apk in $apkFiles) {
                        $fullPath = Join-Path $publishDir $apk
                        $size = [math]::Round((Get-Item $fullPath).Length / 1MB, 2)
                        Write-Host "📱 APK: $apk ($size MB)" -ForegroundColor Green
                        Write-Host "   📍 Local: $fullPath" -ForegroundColor Cyan
                    }
                } else {
                    Write-Host "⚠️  APK não encontrado no diretório de publicação" -ForegroundColor Yellow
                    Write-Host "Listando conteúdo do diretório publish:" -ForegroundColor Gray
                    Get-ChildItem -Path $publishDir -Recurse | Format-Table Name, Length, LastWriteTime
                }
                
                Write-Host "`n🚀 Próximos passos:" -ForegroundColor Cyan
                Write-Host "1. Transfira o APK para um dispositivo Android" -ForegroundColor White
                Write-Host "2. Habilite 'Origens desconhecidas' nas configurações" -ForegroundColor White
                Write-Host "3. Instale o APK" -ForegroundColor White
                Write-Host "4. Teste a aplicação!" -ForegroundColor White
            }
        } else {
            Write-Host "❌ Erro ao gerar APK" -ForegroundColor Red
        }
    } else {
        Write-Host "❌ Erro na compilação" -ForegroundColor Red
    }
}
finally {
    # Restaurar arquivos originais
    Write-Host "`nRestaurando arquivos originais..." -ForegroundColor Yellow
    foreach ($file in $filesToModify) {
        $backupFile = "$backupDir\$(Split-Path $file -Leaf).bak"
        if (Test-Path $backupFile) {
            Copy-Item -Path $backupFile -Destination $file -Force
            Write-Host "  Restaurado: $file" -ForegroundColor Gray
        }
    }
    
    # Remover backup
    if (Test-Path $backupDir) {
        Remove-Item -Path $backupDir -Recurse -Force
    }
    
    Write-Host "✅ Arquivos restaurados" -ForegroundColor Green
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n🎉 BUILD CONCLUÍDO COM SUCESSO!" -ForegroundColor Green
} else {
    Write-Host "`n💥 BUILD FALHOU - Verifique os logs acima" -ForegroundColor Red
    exit 1
}
