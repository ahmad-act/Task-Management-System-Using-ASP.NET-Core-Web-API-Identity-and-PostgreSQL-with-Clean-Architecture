# How to run this ps1 file: PS D:\My Study\Software Engineering Topics\Clean-Architecture-Template> ./ExecCodeCoverage.ps1

# PURPOSE: Automates the running of Unit Tests and Code Coverage
# Ensure the dotnet-reportgenerator-globaltool is installed (install only once globally)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Get the parent directory of the current directory
$parentDir = (Get-Item -Path ".").FullName

# Clean up old TestResults folder in the parent directory
if (Test-Path "$parentDir/TestResults/") {
    Remove-Item -Recurse -Force "$parentDir/TestResults/"
}

# Clean up old coverage reports in the parent directory
if (Test-Path "$parentDir/coveragereport/") {
    Remove-Item -Recurse -Force "$parentDir/coveragereport/"
}

# Ensure a history directory exists for code coverage reports in the parent directory
if (!(Test-Path -path "$parentDir/CoverageHistory")) {
    New-Item -ItemType directory -Path "$parentDir/CoverageHistory"
}

# Replace TaskManagement.Template.sln with your solution file name if necessary
# Run unit tests and collect code coverage
$output = [string] (& dotnet test "$parentDir/TaskManagement.Template.sln" `
                    --collect:"XPlat Code Coverage" `
                    --results-directory:"$parentDir/TestResults" `
                    2>&1)

# Display the last exit code and test output
Write-Host "Last Exit Code: $lastexitcode"
Write-Host $output

# Generate the code coverage HTML report in the parent directory
reportgenerator -reports:"$parentDir/TestResults/**/coverage.cobertura.xml" `
                -targetdir:"$parentDir/coveragereport" `
                -reporttypes:Html `
                -historydir:"$parentDir/CoverageHistory"

# Open the code coverage HTML report (only if running on a workstation)
$osInfo = Get-CimInstance -ClassName Win32_OperatingSystem
if ($osInfo.ProductType -eq 1) {
    Start-Process "$parentDir/coveragereport/index.html"
}
