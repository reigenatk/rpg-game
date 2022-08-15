

# 8/14/22
Added a "Is Triggered Cutscene" option to the LevelLoader, so we can distinguish between cutscenes that get triggered via code, vs cutscenes that are ran through each time a new scene is loaded. 

The ones marked with "Is triggered cutscene" will not be checked against whenever a new scene is loaded- those are only triggered using Yarn. So like, if some dialogue runs in Yarn and we want to trigger a cutscene, then we can do so for cutscenes with this checked.


Game Time scale:
- 50 game seconds is 1 real second
- 1 min irl = 50 min in game
- 5 min irl = 250 min in game = 4 hrs 10 min
- 10 min irl = 500 min in game = 8.333 hrs 

Also setup a DialogueWithTime class so that each object in the scene will return a different dialogue depending on what time it is. Earliest times should go first in the array (for the Unity Editor) and more restrictive (must be at least x late) should be later in the editor.