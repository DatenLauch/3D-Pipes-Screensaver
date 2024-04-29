# 3D Pipes Screensaver
![image](https://github.com/DatenLauch/3D-Pipes-Screensaver/assets/103434272/a98565a4-ddd5-4823-b407-65202b9ce5bc)

A practice project / coding challenge to recreate the classic Windows 98 3D Pipe Screensaver.

Made with Unity Version 2022.3.25f1

# How it's implemeneted
 
The main scene contains a gameobject called "PipeGenerator". First the user defines a cubic spawn area within the field "Spawn Area Size". Additionally, the user can also set the start delay and a spawn intervall. On launch, the PipeGenerator is placed at a random valid destination within the spawn area, with a random rotation and picks a random color for the pipes.

The main spawn loop works like this: The PipeGenerator attempts to move to a random neighboring position (top, down, left, right, forward). The attempt is validated so that the generator stays within the designated area and no pipe collisions occur. If the PipeGenerator had to turn in any direction to achieve it's destination, a pipejoint prefab is spawneded. A pipe prefab is then placed facing to the original position and the loop repeats. If in any case the generator runs into a dead end, the generator is then assigned a new randomized valid position with the spawn area.

# To do:

* ~~Work on Better Rotation~~ ---> Fixed with branch **"Generator-Script-refactored"**
* ~~Allign some Pipe Ends~~ ---> Fixed with branch **"Generator-Script-refactored"**
* ~~Unifying pipe colors~~ ---> Fixed with branch **"Color-Changer-refactored"**
* ~~Add Background~~ ---> Fixed with branch **"Background"**


