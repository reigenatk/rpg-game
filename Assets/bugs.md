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

Bug: Thinking that a smaller `Max Distance` for Unity Audio Sources means that the sound will decay faster as the target moves away.

Fix: It actually makes it stay at a certain level forever, regardless of the sound! If you hover over the tooltip it says "the max distance that the sound **will stop attenuating at!**", which is significant since attenuation = sound goes lower! SO by setting max distance to 5, I'm making it so that past 5 unity units, the sound will stop going lower! What a disaster!

Instead I should **set max distance to a higher value** if I want the sound to decay faster. That also explains why the default is 300, its not that extraordinary after all.

Actually I just used a custom rolloff, which zeros out at around 15. Thing is, Unity default makes it so that it decays over a long period of time, meaning if I set max distance to 200, it will slowly decay from like 0.1 to 0.0 from like distance = 20 to 200, which is annoying, cuz I want it so that once it hits a certain distance (like 20) it instantly hits 0 and stays at zero.

Bug (unsolved): Why doesn't this work in LevelLoader?
`TimeManager.Instance.gameClockPaused = true;`
If I do this, the time doesn't pause whenever a cutscene plays. Huh?

But if I check if cutscenePlaying != null in the TimeManager, it works. Da fuk

Bug: Is Path wasn't doing anything at ALL!

Fix: Holy shit this took me like 2hrs to fix. Essentially it came down to me printing out which tiles the program was assigning the path penalty to (which is 0), and which ones it was assigning the default tile penalty to (which is a large value like 100). Turns out, **it wasn't assigning any default tile penalties!** This meant that all tiles had a penalty of 0 and as a result my path thing wasn't doing anything at all.

The problem? An else statement was one level too deep. Specifically line 287, in `Astar.cs` the function called `PopulateGridNodesFromGridPropertiesDictionary`. The line where it says 

```
else
{
    Debug.Log("coords " + x + ", " + y + " is default");
    Node node = gridNodes.GetGridNode(x, y);
    node.movementPenalty = defaultMovementPenalty;
}
```
Was one level too deep. When I was copy pasta-ing from the RPG course I probably fuckin didn't pay attention or something when doing it.

But ok, now it works! Only downside is, I set all the width/heights to 300 so that means it has to loop 90,000 times in one frame, which as you can tell just bricks the game for a second. So I probably need to either make my tilemaps smaller for each scene (uh oh, Campus scene might have a problem here), or I can somehow find a way to do all this in the Editor code when we set the scriptable object for each tilemap grid properties object.

EDIT: Huh, ok so I checked his code and it has the else statement in the same level as the others. Which is strange because that makes no sense- there is no grid property entry for a default tile. Here's the code

```
    // populate obstacle and path info for grid
    for (int x = 0; x < gridDimensions.x; x++)
    {
        for (int y = 0; y < gridDimensions.y; y++)
        {
            GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(x + gridOrigin.x, y + gridOrigin.y, sceneSave.gridPropertyDetailsDictionary);

            if (gridPropertyDetails != null)
            {
                // If NPC obstacle
                if (gridPropertyDetails.isNPCObstacle == true)
                {
                    Node node = gridNodes.GetGridNode(x, y);
                    node.isObstacle = true;
                }
                else if (gridPropertyDetails.isPath == true)
                {
                    Node node = gridNodes.GetGridNode(x, y);
                    node.movementPenalty = pathMovementPenalty;
                }
                else
                {
                    Node node = gridNodes.GetGridNode(x, y);
                    node.movementPenalty = defaultMovementPenalty;
                }
            }
        }
    }
}
```

Basically, I'm arguing that the last else statement should be one level outwards (so the same level as the if statement) because gridPropertyDetails will be null for a tile that we didn't paint the `Is Path` or `IsNPCObstacle` tile on. Because the scriptable object is just a list of coordinates that have these special designations. Otherwise, its not in the list, aka, its null. SO the if checks whether or not its null (that is, whether its painted), and if its not painted, we do an else. 

I swear to god his code is just wrong. Maybe I should tell the Udemy course dude. Cuz this wasted me forever. And tbh I don't watch his vids closely so IDK where he typed this in but it seems incorrect. Cuz my game works now, after doing the fix I just said. wtf?

~~So I'm thinking, I add some code into `TilemapGridProperties` which is the editor script responsible for populating tilemap info.~~

Actually scratch above idea, kinda dumb. Instead I'll just set proper height and width. Don't think there's any way I can save from looping over the size of the grid anyways (so what if I store it in the scriptable object? It's still gonna have to read every entry, right?) Like, its still gonna have to assign a movement penalty to each grid coordinate (note I said GRID coordinate, relative to the grid ORIGIN which isn't the tilemap transform offset, btw (which again I shoulda just set to 0 for all the maps, but I was dumb)).

Again, it's that (2, 2) is at point (1,1) if the grid origin is at (1,1) kinda thing.

Bug: The pattern `GameState.Instance.something` doesn't work anymore, and keeps returning NullReferenceException, even though GameInstance inherits from Singleton. So I'm just doing FindObjectOfType again, but it'd be nice to know why it broke when it was working OK before.

Fix: ??

Bug: When not in the scene, NPC audio isn't switching correctly. For instance, when Brain wakes up to go get breakfast, his audio is still playing the snoring sounds instead of the eating breakfast sounds..

Fix: ??

Bug: If I collide with a walking NPC, I start to slide along with them despite not moving. The only way I can break this movement is if I collide with something else. Dafuq?

Fix: Fixed on 10/31/22

Tip: If NPC is going to the edge of one scene then stopping, its probably cause there is a "No shortest path found" message in the console. This is usually caused (at least it has like 3x for me already) by a NPCObstacle square on your target destination. Or A*  legit just can't find a path found to that tile (it is surrounded by NPCObstacles, for instance).

Bug: NPC Character doesn't turn around immediately when you talk to them.
Fix: There was an exit time on some of the animations. For example when becky walks down street in Outside House scene and I talk to her from above, she does one cycle on IdleDown before going IdleUp. So yeah we fix this by just saying Any State -> Idle Up has **No exit time**

Bug (kinda): Right now, let's say I wanna do some dialogue, then an animation, then some more dialogue. Pretty common pattern. Well I have to make **two** dialogues for this, one before the animation and one after. Cuz, if I try doing some animation trigger stuff inside of Yarn, the cutscene is still active so I think the timeline is still overriding the animation controller and therefore not letting us trigger our animation.

So my goal is to find a way to (inside of yarn) trigger a short animation state before reverting back to the original state. This would be so much more efficient than having to make a new cutscene whenever the characters in the scene feature even the smallest animation change. A prime example was Kabowski turning around in the eating scene. Such a small change, but I need to make a dialogue before and a dialogue after for it.

Dev Tip: Just like how you don't wanna put an enum inside a list of enums since it will screw up what you currently have in Unity editor (since unity depends on the order in which they appear to determine which one you want), in the same way, when you make new animation frames for a character, **do not put them in between the extisting frames**. Like for instance say I have frames 1-10, then I want to put this new frame at position 11 ALWAYS. Otherwise the animations that I currently have, that use frames 1-10, might use this new frame that I insert in (say at position 9), which will screw up what I already have.

Also Unity kinda sucks in that if you add more animation frames you have to re-apply the slice operation. I wish there was some functionality to just auto do it for you. But oh well.

Bug: Had one where cutscenes would trigger twice when talking to someone, despite their being only one collider and one dialogue activate script... I'm lost on this 

Fix: So I just hacked together a solution by storing the name of the cutscene that is running in GameState and then checking if the next one we want to run is of the same name, and if it is, to do nothing... But this doesn't solve the fact that its running twice, or tell me why that's even occuring...

