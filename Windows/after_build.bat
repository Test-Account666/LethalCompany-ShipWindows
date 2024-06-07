:: AI-generated script disclaimer
echo "==================== WARNING ===================="
echo "I am using Linux and therefore, this script has not been tested."
echo "It is an AI-generated script."
echo "Please proceed with caution."
echo "If you run this script, I can NOT be held liable for any damages that may arise."
echo "You have been warned!"
echo "==================== WARNING ===================="
PAUSE

:: Remove the existing BuildOutput directory if it exists
if exist "BuildOutput\" (rmdir /s /q "BuildOutput\")

:: Create the BuildOutput directory
mkdir BuildOutput

:: Create the sounds directory
mkdir "BuildOutput\sounds"

:: Copy files to the BuildOutput directory
copy "%CURRENT_PROJECT%\bin\Debug\netstandard2.1\TestAccount666.%CURRENT_PROJECT%.dll" "BuildOutput\%CURRENT_PROJECT%.dll"
copy "%CURRENT_PROJECT%\README.md" "BuildOutput\"
copy "%CURRENT_PROJECT%\CHANGELOG.md" "BuildOutput\"
copy "%CURRENT_PROJECT%\icon.png" "BuildOutput\"
copy "%CURRENT_PROJECT%\sounds\ShutterClose.wav" "BuildOutput\sounds\"
copy "%CURRENT_PROJECT%\sounds\ShutterOpen.wav" "BuildOutput\sounds\"
copy "LICENSE" "BuildOutput\"
copy "ship_window" "BuildOutput\"

:: Generate manifest and zip file
call powershell.exe -ExecutionPolicy Bypass -File generate_manifest.ps1
call powershell.exe -ExecutionPolicy Bypass -File generate_zipfile.ps1

:: Assuming explorer.exe is located in the current directory or added to PATH
call explorer.exe "BuildOutput"

:: Pause to examine the output
PAUSE