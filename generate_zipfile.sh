#!/bin/sh

directory="BuildOutput"

# Check if the provided directory exists
if [ ! -d "$directory" ]; then
    echo "Error: Directory '$directory' does not exist."
    exit 1
fi

# Navigate to the directory
cd "$directory" || exit 1

# Get the base name of the directory
dir_name=$(basename "$directory")

# Create the zip file
zip_file="TestAccount666-"$CURRENT_PROJECT".zip"

# Add all files in the directory to the zip file
zip -r "$zip_file" . 
