# Define the directory variable
$directory = "BuildOutput"

# Check if the provided directory exists
if (-not (Test-Path $directory)) {
    Write-Host "Error: Directory '$directory' does not exist."
    exit 1
}

# Navigate to the directory
Set-Location $directory -ErrorAction Stop | Out-Null

# Get the base name of the directory
$dir_name = Split-Path -Leaf $directory

# Define the zip file name using a variable for the current project name
$zip_file = "TestAccount666-$env:CURRENT_PROJECT.zip"

# Add all files in the directory to the zip file
Compress-Archive -Path.\* -DestinationPath $zip_file
