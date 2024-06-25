# Ship Windows

## 1.3.7

\* Fixed door texture (Sorry it took so long)<br>

## 1.3.6

\* Hopefully finally completely fixed overriding celestial tint skybox<br>
\* Hopefully fixed all errors with shutter switch<br>
\- Reverted change regarding the `Window` layer<br>

## 1.3.5

\+ Added config option to fix enemies killing you through windows (If you're running into issues with enemy behavior,
disable this)<br>
\# Windows are now on their own layer, called `Windows` (id should be 30)<br>

## 1.3.4

And again...<br>

## 1.3.3

\* Fixed skybox override not working in certain scenarios<br>

## 1.3.2

\* Fixed Celestial Tint overriding the skybox override<br>
\* Fixed log spam, sorry about that<br>

## 1.3.1

\* Fixed voice lines playing without windows<br>

## 1.3.0

\+ Added option to override Celestial Tint's skybox<br>
\+ Added configurable rotation speed (Replaces boolean)<br>
\+ Added config option to disable hiding moon transitions<br>
\+ Added new screenshots to readme<br>
\* Fixed shutter switch spawning without windows<br>
\* Fixed NullReferenceExceptions when shutter is spawned<br>
\* Fixed mod refusing to load, if all except door window are deactivated<br>
\* Fixed 4k skybox never getting loaded<br>
\# Increased moon transition time for shutters by 1 second<br>
\# SpaceOutside config option now uses a self-explanatory enum instead of an integer<br>

## 1.2.3

\* Reduced the intensity of iridescence in glass materials

## 1.2.2

\* Fixed door windows shutter being invisible<br>
\* Fixed shutter ScanNode being not deactivate-able<br>

## 1.2.1

\+ Added option to disable the Shutter Switch ScanNode<br>
\+ Added option to disable playing Wesley's voice lines on transitions<br>
\+ Added iridescence glass materials<br>
\* Fixed speaker sound being played with inactive VoiceLines<br>
\# Buying a new window will now honor the current shutter state<br>
\# If you change the glass material via LethalConfig, the setting will be applied without restart<br>
\# The enable refraction boolean has been replaced with an enum to allow for iridescence material to co-exist<br>

## 1.2.0

\+ Added ship door window!<br>
\+ Added ability for light sources to actually shine through all windows<br>
\+ Added voice lines for opening/closing the window shutters (
Thanks [Wesley](https://thunderstore.io/c/lethal-company/p/Magic_Wesley/) <3)<br>
\+ Added Scan Node for Shutter Switch<br>
\+ Added option to change the Light Switch tool tip<br>
\* Fixed being able to place furniture inside windows<br>
\* Fixed skybox 2<br>

## 1.1.2

\* Fix stretched ship hull texture<br>

## 1.1.1

\* Fix all window collisions (Hopefully...)<br>

## 1.1.0

\+ Shutter switch is now non-interactable while the windows are locked<br>
\+ Window state is now sync to the state switch, instead of being relative<br>
\* Fix posters<br>

## 1.0.1

\* Fix locked window state<br>
\# Improved window shutters closing/opening on level transitions<br>

## 1.0.0

\+ Initial Release<br>
