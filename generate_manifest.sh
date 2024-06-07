#!/bin/sh

# Extract information from .csproj file
name=$(xmlstarlet sel -t -v "//PropertyGroup/Product" "$CURRENT_PROJECT"/"$CURRENT_PROJECT".csproj)
version=$(xmlstarlet sel -t -v "//PropertyGroup/Version" "$CURRENT_PROJECT"/"$CURRENT_PROJECT".csproj)
description=$(xmlstarlet sel -t -v "//PropertyGroup/Description" "$CURRENT_PROJECT"/"$CURRENT_PROJECT".csproj)
website=$(xmlstarlet sel -t -v "//PropertyGroup/Source" "$CURRENT_PROJECT"/"$CURRENT_PROJECT".csproj)

# Generate JSON content
manifest=$(cat <<EOF
{
    "name": "$name",
    "version_number": "$version",
    "website_url": "$website",
    "description": "$description",
    "dependencies": [
        "BepInEx-BepInExPack-5.4.2100"
    ]
}
EOF
)

# Write JSON content to manifest file
echo "$manifest" > BuildOutput/manifest.json
