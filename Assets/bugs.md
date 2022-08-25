Bug: Scene tilemap not rendering
Fix: Make sure the z-value of the tilemap grid is 0, our cam is at -10, so it needs to be below

Bug: Tilemap seemed to keep yeeting my character out of position.
Fix: The bounds confiner wasn't set to trigger and hence kept colliding with my player- fml I spent an hour on this.

Bug: Unity wouldn't let me save a scene called Bathroom.unity, kept deleting everything the instant I unloaded it
Fix: I renamed it to BathroomWTF

Bug: Unity Tilemap keeps shifting itself randomly
Fix: IDK why this is happening yet. I think when I make a new scene I'm somehow shifting the other tilemaps' offsets? Not sure.

Bug: Sprite sort around pivot wasn't working! 
Fix: Took me like 3 hrs but I figured it out, basically my Univeral Rendering Pipeline was outdated. Updated that, and then went to the 2D Renderer Data, set the transparency sort mode to custom axis, with values 0,1,0, and it works. Fkin hell. Well also, my sprites had the pivots in the wrong places too, so I guess it makes sense. Have to set all the pivots to Bottom Center, in the sprite editor.

Bug: Characters runs straight thru walls!
Fix: Change the fixedTimeStep to 0.005 in the Project Settings, check out [this thread](https://forum.unity.com/threads/what-are-the-necessary-settings-to-prevent-objects-passing-through-each-other-at-high-speeds.384519/)

Bug: Cinemachine cam keeps snapping randomly
Fix: Make sure Bounds Confiner is a rectangle! Even if its a little bit off, it will fail and have this bug. [Here](https://www.reddit.com/r/Unity3D/comments/9ubpur/cinemachine_camera_jumps_around_how_to_fix/).

Bug: In the Cutscene where player falls asleep, in the last frame he changes directions to left-idle again even though the last animation state is Idle-down.
~~Fix: Make sure the Wrap on Playable Director is set to Hold, rather than None. What this means is that, the Timeline will end on the last state that it was at. It was facing left because that's the last state the animation controller was in when we interacted with the bed (since we walk left and press E).~~
EDIT: Still not fixed, so this introduces a new bug. Now I can't move my player to the dark scene coordinates of 0 -50 0, and it fails. Need to find a way to both end on the right state and also move the player. So wrap mode: hold is probably not it.
Fix: Turns out the bug was because of the collisions! I have a box collider 2d on the player, and also on the bed. So in the gap between cutscene and dialogue, the box collider was slightly pushing my player! To fix this, I turn off box collider everytime a cutscene starts. Then I re-enable when the cutscene ends. Also I re-worked such that GameState now stores the actual PlayableDirector object for whatever cutscene is currently running.

Bug: Tiles not being affected by the 2D Global Light
Fix: Make sure the Material on the tilemap layers is "2D Lighting"

