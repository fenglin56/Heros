TILE BASED MAP AND NAV
=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

by Leslie Young
Contact: http://www.plyoung.com/ or http://plyoung.wordpress.com/

More Info on the Unity Forum at http://forum.unity3d.com/threads/138355-Tile-Based-Map-amp-Nav

Map & Nav will help you get started with a game where you need a tile based grid system. This is especially useful for strategy games and board games.

* Full source code (C#)
* Hexagon and Square tile layouts
* Tools to quickly create new layouts/maps
* Custom inspector and Editor windows
* Easy setup of 'tile-type masks' (ex. land, water, wall, etc)
* Variable Height Nodes & Tools to easu setup of node heights
* Unit movement options (hug/align to floor/terrain mesh and jump)
* iOS/Android supported
* Various sample scenes

Updates

=== Verison 2.8
* Added a sample showing how Projectors can be used for tile node markers.
* Updated to allow TileNode gizmo size to adapt to tile spacing scale
	
=== Version 2.7
* Fixed a bug related to functions that show/hide node markers.

=== Version 2.6
* Added new option to NaviUnit.cs, “usingSameTimeMovement”. Fixed a bug in unit movement. 
* Optimised the TileNode.ShowNeighbours() functions which would become slow when a radius bigger than 3 tiles where used.

=== Version 2.5
* Moved the TMNController and unit layer related code to samples since it is more related to sample code than the core of Map&Nav.
* Added requested sample (6) which shows how to spawn a unit when you click on a tile.
* Added a new tool to help setup the links between nodes by breaking links between nodes where the one node (tile) might be much higher than another and you do not want units to move from the one node to the other in these cases.
* Added a way to links up the nodes between different maps (MapNavs).

=== Version 2.4
* Added the options to set if more than one unit may occupy a tile. 
* Small bug fixes.
* Some changes to the movement code to allow units to move at the same time and wait if another unit is in the way.
* Added option to GameController to turn auto-random movement of units on (set it before pressing play, else it won't have an effect).

=== Version 2.3
* Changed and optimised the way TNEMovementModifier works.
* Added ability to turn on or off the link between two nodes (TNELinksOnOffSwitch). Useful for doors or even cases where you permanently want the link between two nodes disabled.
* Updates sample4 (dungeon) to show how a door could be implemented.

=== Version 2.2
* The TileNode prefabs are no longer hard-coded in the editors, you can now specify the Prefab to be used.
* Improved Auto Mask-Setup Tool. Movement modifier options added, this will allow you to set certain tiles to cost more movement points to move onto.

=== Version 2.1
* Variable height tile-nodes was added and new movement options for units.

=== Version 2.0
* Release of version 2, which is a total rewrite and clean-up from version 1, while adding square tile layout support and the Radius Marker object.

-end-


