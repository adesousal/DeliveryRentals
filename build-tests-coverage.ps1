# Caminhos
$testProject = "DeliveryRentals.Tests/DeliveryRentals.Tests.csproj"
$outputFolder = "TestResults/Coverage"
$reportFolder = "TestResults/CoverageReport"

# Limpar resultados anteriores
Remove-Item $outputFolder -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item $reportFolder -Recurse -Force -ErrorAction SilentlyContinue

# Executar testes com cobertura
dotnet test $testProject `
  --collect:"XPlat Code Coverage" `
  --results-directory $outputFolder `
  --settings coverlet.runsettings

# Pegar caminho do .cobertura.xml
$coverageFile = Get-ChildItem "$outputFolder/**/coverage.cobertura.xml" -Recurse | Select-Object -First 1

# Gerar relatório HTML
reportgenerator -reports:$coverageFile.FullName -targetdir:$reportFolder -reporttypes:Html

# Abrir no navegador
Start-Process "$reportFolder/index.html"