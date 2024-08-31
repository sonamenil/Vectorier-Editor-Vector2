# Vectorier Unity Editor
Vectorier-Unity-Editor is an level editor for the game Vector using the Unity Engine.

A Discord server is open for the project : https://discord.com/invite/pVRuFBVwC2

# Features
* Level Creation

# Installation
 * Download Unity [2020.3.35f1](https://download.unity3d.com/download_unity/18e4db7a9996/Windows64EditorInstaller/UnitySetup64-2020.3.35f1.exe) at the [official Unity Website](https://unity3d.com/get-unity/download/archive)
 * Download the [Vectorier Unity Editor project]((https://github.com/sonamenil/Vectorier-Editor-Vector2.git))
 * Extract the zip file
 * Open the project in Unity Hub

# Rooms Folder
on Project Settings> Editor> Vectorier, you should find a place to put the files where the rooms are located (inside vector 2 is Resources/gamedata/run_data/rooms

* Write the room filename to replace in script manager

Fun fact: you can just hit restart when youre in your level after you build, so you dont have to restart your game.
 
# Tutorials

* First off, i recommend having at least a bit of knowledge about the Vector 1 editor, since this one follows its rules.

  # Basics
  The tag in works as the spawn of the level. Create am empty game object, place it where you want and tag it as in
  
  The tag out is what finishes the level. Same method as in object. IT IS OBLIGATORY

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
   To set the height of a laser, select the first three child objects (the laser beam) and resize it, once youve done that, take the y scale and multiply it by 5.
   So in the object reference component, on LaserReachDistance variable, paste the total scale.

   ![image](https://github.com/user-attachments/assets/a094d6bb-9127-404c-9858-669c97e971ce)     ![image](https://github.com/user-attachments/assets/4b4a0002-0941-44e7-8928-83da1b6e7e93)

   # Dynamic
   Normal dynamic movement works the same as in Vector 1 editor, but the new thing is the size interval.
   On the movement usage you have a new option wich is size interval, that makes the dynamic properties be of size instead of movement.
   Below theres the matrix properties, wich are automatically set to 0.001 (it make the image look tiny), to make the image look normal, set the size interval to 1000, wich will set the matrix to 1.

   For the trigger, you need to make it a separate object (not a child object as usual) and tag it as dynamic trigger, after that it should be like usual.

# Packages
* Used textures from Domnul Inginer
