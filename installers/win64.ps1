param (
    [string]$InstallDir = "$env:ProgramFiles\\Chronky"
)

# Get the directory of the script
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Ensure the installation directory exists
if (!(Test-Path -Path $InstallDir)) {
    New-Item -ItemType Directory -Path $InstallDir -Force
}

# Copy all files to the installation directory
Write-Host "Copying application files to $InstallDir..."
Copy-Item -Path "$ScriptDir\\*" -Destination $InstallDir -Recurse -Force

# Add the application directory to the PATH variable
$EnvPath = [System.Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::Machine)
if ($EnvPath -notlike "*$InstallDir*") {
    [System.Environment]::SetEnvironmentVariable("Path", "$EnvPath;$InstallDir", [System.EnvironmentVariableTarget]::Machine)
    Write-Host "Added $InstallDir to PATH. Please restart your shell for the changes to take effect."
}

Write-Host "Installation completed successfully!"
