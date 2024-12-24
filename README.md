# Vectorier Unity Editor
Vectorier-Unity-Editor is an level editor for the game Vector 2 using the Unity Engine.

A Discord server is open for the project : https://discord.com/invite/pVRuFBVwC2

# Vector 2 Unity project
(Fully playable!!!!) [Download it here](https://drive.google.com/file/d/17Hc4J0gTjg9LgAnzJT_BC1q9L01kdUs0/view?usp=sharing) Unity version 5.6.7f1 required. ([here](https://download.unity3d.com/download_unity/e80cc3114ac1/Windows64EditorInstaller/UnitySetup64-5.6.7f1.exe))

# Features
* Level Creation

# Installation
 * Download Unity [2020.3.35f1](https://download.unity3d.com/download_unity/18e4db7a9996/Windows64EditorInstaller/UnitySetup64-2020.3.35f1.exe) at the [official Unity Website](https://unity3d.com/get-unity/download/archive)
 * Download the [Vectorier Unity Editor project](https://github.com/sonamenil/Vectorier-Editor-Vector2/archive/refs/heads/main.zip)
 * Extract the zip file
 * Open the project in Unity Hub

# Rooms Folder
on Edit> Project Settings, you should find a place to put the files where the rooms are located (inside vector 2 is Resources/gamedata/run_data/rooms

* Write the room filename to replace in script manager

Fun fact: you can just hit restart when youre in your level after you build, so you dont have to restart your game.

# Warning about textures
Please take them from the textures folder on resources, you can later add z2 textures
 
# Tutorials

* First off, i recommend having at least a bit of knowledge about the Vector 1 editor, since this one follows its rules.

  # Basics
  The tag "in" works as the spawn of the level. Create am empty game object, place it where you want and tag it as in
  
  The tag "out" is what finishes the level. Same method as in object. IT IS OBLIGATORY

  you have an example level wich shows basic stuff.

  # Object References
  To start, you need to learn about object references.
  When you spawn an object, and want to make it an object reference, tag it as one and add the object reference component. Once youve done that, you need to specify the filename from where the object comes from, the component has a list of them.

 * Custom variables:
   It modifies the object by changing specific things, you can find them where the object is in its xml.

   ![image](https://github.com/user-attachments/assets/734930ae-eddf-47ab-9a68-be70b7225ba2)

   # Sorting Layers
   On the sprite renderer component of an image, there should be a sorting layer part, you should select one for the level to look good.

   * Black: for stuff like v_black, and black decoration
  
   * Wall: for stuff like walls, decoration, etc.
  
   * Shadows: for stuff like gradient

   # Lasers
   Extremely simplified the laser system, now just resize the image called laser_complete to set the height of the laser. You can also move the laser activator trigger so it activates sooner or later (only works when global timer is on)

   # Dynamic
   Normal dynamic movement works the same as in Vector 1 editor.

# Packages
* Used textures from Domnul Inginer
