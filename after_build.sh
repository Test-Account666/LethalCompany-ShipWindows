rm -rf BuildOutput &&
mkdir BuildOutput &&
cp -f "$CURRENT_PROJECT"/bin/Debug/netstandard2.1/TestAccount666."$CURRENT_PROJECT".dll BuildOutput/"$CURRENT_PROJECT".dll &&
cp -f "$CURRENT_PROJECT"/README.md BuildOutput/ &&
cp -f "$CURRENT_PROJECT"/CHANGELOG.md BuildOutput/ &&
cp -f "$CURRENT_PROJECT"/icon.png BuildOutput/ &&
cp -rf "$CURRENT_PROJECT"/sounds BuildOutput/ &&
cp -f LICENSE BuildOutput/ &&
cp -f ship_window BuildOutput/ &&
cp -f ship_windows_shutter BuildOutput/ &&
./generate_manifest.sh &&
./generate_zipfile.sh &&
xdg-open "./BuildOutput"
