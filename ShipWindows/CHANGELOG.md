# Ship Windows

## 2.7.1

\+ Added little easter egg<br>
\* Fixed Shutter Switch again .-.<br>

## 2.7.0

\+ Added support for latest Interactive Terminal API<br>
\* Fixed some shenanigans with Shutter Switch<br>

Note:<br>

- Shutter Switch may still cause issues like clients not triggering Wesley Voice<br>

## 2.6.1

\* Fixed Enemy Fix<br>

## 2.6.0

\* Fixed v73 compatibility<br>

## 2.5.1

\* Fixed layer + tag for window-less ship<br>

## 2.5.0

\# Final push to stable<br>

## 2.4.0

\+ Added `WindowRegistry#Windows` to get a readonly list of all registered windows<br>

## 2.3.3

\* Fixed self-intersecting in roof window<br>
\# Updated readme<br>

## 2.3.2

\* Fixed lobby load playing wesley voice<br>
\* Fixed issues with Diversity<br>
\- Removed debug messages<br>
\# Now forcing `movePosters` to `false`, if other ship mods are present<br>

## 2.3.1

\* Fixed skybox<br>

## 2.3.0

\+ Added Skybox Events<br>
\# Now avoiding `GameObject.Find` calls wherever possible<br>

## 2.2.1

\* Fixed window spawning behavior<br>
\* Fixed holes in ship<br>
\# Tried to optimize window meshes<br>
\# Now shows a message, if no unlockable windows could be found<br>

## 2.2.0

\+ Added right wall window<br>
\+ Added roof window<br>
\* Fixed not being able to place furniture on floor window<br>
\* Fixed unlocked windows not spawning on save load<br>
\* Fixed floor window mesh

## 2.1.0

\+ Added left wall window<br>
\+ Added wesley voice lines<br>
\+ Added enemy attack fix<br>

## 2.0.0

\# Start of the rewrite

## 1.10.2

\* Fixed ship shutter switch sound<br>

## 1.10.1

\* No more fatal debug messages<br>

## 1.10.0

\* Fixed Ship Floor using the Ship Wall material<br>
\# Switched to v61 Unity Project. Stuff may look/react differently now. Please report issues.<br>

## 1.9.0

\+ Added support for [ShipColors](https://thunderstore.io/c/lethal-company/p/darmuh/ShipColors/)<br>

## 1.8.0

\+ Added config option to change enemy + window behavior (Defaults to `false` when vanilla mode is enabled)<br>

## 1.7.1

\* Fixed ship object placement<br>

## 1.7.0

\# Made the sell voice line common by default<br>

## 1.6.1

\+ Actually add the sell counter voice line file...

## 1.6.0

\+ Added rare sell counter voice line (Thank you, Wes!)<br>

## 1.5.3

\* Fixed Ship Shutters Lever texture<br>

## 1.5.2

\* Hmmm, turns out, the code I removed was important. I improved it instead of removing it<br>

## 1.5.1

\* Remove useless optimizations that actually worsened performance<br>

## 1.5.0

\* Fixed a lot of bugs by using proper null checks. No more disappearing light switch<br>

## 1.4.0

\+ Add compatibility with latest Celestial Tint<br>

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
