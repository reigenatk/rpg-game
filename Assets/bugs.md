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


