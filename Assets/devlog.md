# General Stuff

Game Time scale:
- 60 game seconds is 1 real second
- 1 min irl = 1 hr in game
- 24 min irl = 1 day in game
I think that's fair enough considering with sleep its more like 2/3 of that

*Energy decay*: we have 100 pts of energy- 86400 sec in a day, 57600 of that is spent awake, so already we can say 576 in game seconds -> 1 energy point. Food will replace energy, but players will more than likely lose more energy than they gain due to doing activites, so I think this number 576 should be slightly **higher** to not drain the energy too hard. Maybe like 600? That way 60000 is the max a user can stay up in one day, which is around 16 2/3 hours (which is assuming they do nothing).

*Social decay*: How about we make this one go from 100 to 0 over course of 2 days? So 57600*2 = 115200 so maybe 1152 is a decent number.

Also setup a DialogueWithTime class so that each object in the scene will return a different dialogue depending on what time it is. Earliest times should go first in the array (for the Unity Editor) and more restrictive (must be at least x late) should be later in the editor.

# Score Penalties
If you sum up the three happiness scores (so not including energy) you can split it into a few categories. In general I want there to be a bit of a snowball effect, i.e, once you get depressed, it gets harder to crawl out and the penalties are harsher

Penalties are for example, sleeping longer (which is a disadvantage in this game. You wanna wakeup early so that you can have the max amount of possible activities, most of which are in the morning. But if you constantly oversleep, you will miss them and therefore make your situation worse)

- <50: Suicidal 
    - Sleeps 10.5 hours (or maybe, we could make him unable to sleep lol)
    - Energy refilled to 60%
    - -5% on Contentedness
- <100: Depressed
    - Sleeps 10 hours
    - Energy refilled to 65%
    - -2.5% on Contentedness
- <150: Unhinged (so each one about 50)
    - Sleeps 9.5 hours
    - Energy refilled to 70%
    - +0% on Contentedness
- <200: Okay
    - Sleeps 9 hours
    - Energy refilled to 80%
    - +2.5% on Contentedness
- <250: Quite good
    - Sleeps 8.5 hours
    - Energy refilled to 85%
    - +5% on Contentedness
- 250-300: Basically Perfect (300 is max anyways)
    - Sleeps 8 hours (maybe we could give user the choice here? Just an idea if I have time)
    - Energy refilled to 90%
    - +7.5% on Contentedness

Also when determining earliest time that player can sleep, each player **must be awake at least for 14? (haven't decided, maybe 15 or 16) hours** before they can sleep. 
So for example if you are doing well, 8am+14hrs = 10pm is earliest you can sleep. Or if you are depressed, you wake up at say noon, then 12+14 = 2am is earliest you can sleep (which compounds punishments).

# Bedtime Penalties
Every hour past 1am is +10 onto previous, starting at 10. So if you sleep between 1am-2am that's -10 pts, then -20 for 2am-3am, then -30 (3am-4am), -40 (4am-5am), -50 for (5am-6am), -60 for 6am-7am, and at 7am you will have pulled the all-nighter so we will advance game day (for fun, maybe we could do like from 7am-noon you lose energy at 2x the rate. We want the player to crash in middle of day as penalty for doing allnighter)

# 8/14/22
Added a "Is Triggered Cutscene" option to the LevelLoader, so we can distinguish between cutscenes that get triggered via code, vs cutscenes that are ran through each time a new scene is loaded. 

The ones marked with "Is triggered cutscene" will not be checked against whenever a new scene is loaded- those are only triggered using Yarn. So like, if some dialogue runs in Yarn and we want to trigger a cutscene, then we can do so for cutscenes with this checked.


# 8/15/22
Drew the three characters
Added a music system

# 8/16/22
Added a mechanism to consider adding Players, each time a scene is loaded
Updated project to 2021.3
Fixed a lot of broken crap like pivots on the players
Added multiple Interactables functionality on the Player

# 8/17/22
- Redid all the dialogue canvas related stuff (canvas now looks good, basically I deleted all the vertical layout group shit since it wasn't working and caused a huge headache), so now the game should look roughly OK on all resolutions/ all screen sizes, thanks to Canvas Scaler. 

- Fixed bounds confiner bug

- Added a "default spawn location" for certain scenes like DarkScene and Commons, so that when we call loadScene without any arguments, it will by default use certain starting points for both the player and the camera. For example, whenever we switch to Darkscene, both player and camera should switch to 0 -50 0, and so I set default position for Darkscene to that location.

# 8/18/22
- Finished the rough draft of the Night 1 Dream, also fixed some strange bugs. Still has a weird bug where its not running a Yarn node despite the name being correct, and the fix (for now) was to put it in a different .yarn file. But oh well.

Probably work on composing a theme song for day 1 tmrw, as well as maybe testing out of energy mechanics (right now not working)

# 8/20/22
Made a music fade out option so that it isn't just an instant cut. So now when you walk between scenes there's a slight fade out that it waits to finish before continuing (using coroutines)

Started to mess with global light 2D to emulate passing days, so far so good. In TimeManager there is now a field which takes list of colors, which is the colors throughout the day, and the global light will Color.Lerp between them

# 8/21/22
Added a sleep scene before bed (with some dialogues and two cutscenes depending on whether or not the lamp is on), a lamp (using spotlight), added intensities to the Global Light Lerp thing, also fixed a ton of bugs, added sounds. Tiring stuff.

# 8/22/22
TODO: Different typing noises for Yarn depending on who is talking
Fixed all the bugs from going to sleep.

The way I did it was kinda convoluted, basically we keep track of whether or not a cutscene is Playing. If Dialogue is started while there is a cutscene, we PAUSE the cutscene, and once the dialogues finishes, we immediately send a signal to fade the screen out. Then after the cutscene (which is just a PlayableDirector) finishes running, there's going to be a .stopped event that we can subscribe to. I add a function in LevelLoader which simply just loads the DarkScene. Check out LieInBed.playabledirector for more info

The reason why we can't fade and load the DarkScene in the timeline is because the Timeline screws with the position of the player, so when we want player to go to 0 -50 0, it won't since its still in the timeline.

Oh, I also started on adding conditional teleporters (so for example during Day 2, we want to incentivise the user to exit the house, so we lock all the rooms once they enter the common area)

# 8/23/22
Tried to make a leaving the house animation, also made a wakeup anim and some ways for easier debugging (skipping over dreams to start days instantly)

TODO: make house scene

# 8/24/22
Made even more bugfixes, such as no moving between scene transitions, added knocking functionality and animations (if you go to door and it is locked, you can knock on it), made some animation triggers for playing simple animation + sound effects (dont use timeline for this, just instead create animation trigger and trigger it, then call playSoundString). We should reserve timeline usage for the most complicated scenes.

Also made decent progress on drawing the outside scene. No music yet.

# 8/28/22
Finished up drawing outside scene for now, also made another transition to the next scene (bus picks the player up), title "Campus". Some important changes- inside of playCutscene I made it so that it will also search for the cutscene passed in as a string, so make sure you don't name two cutscenes the same. Also, make sure that **two cutscenes never play at the same time** (because we have cutscene is/isnt playing and all, it would get confusing fast).

Also fixed some strange bug with the manual timeline thing I tried to do... Where character
is knocking at the door while facing up, and it keeps doing some weird stuff

I ended up just using timeline instead, but now there's still two problems:

1. Unity is randomly crashing, dunno how but I seemed to have triggered this
2. It's still playing some walking noises during the cutscene (after you select an option
in the dialogue, it plays walking noises which is super annoying)

# 8/29/22
Started drawing Campus scene, added 2D Tilemap extras package for coordinate brush

Starting to watch the pathfinding + tilemap properties lectures so that I can implement
some NPC movement, since the outdoor scenes right now look really dumb without cars
or NPCs moving around. It's a college campus for crying out loud

A bit of a note to self, watch lecture 44 (@33:30) to see how to use tilemap grid properties (around 35 min in). But if you're lazy basically the GridProperties start as a disabled object, with all the data being stored in the scriptable object. To make something happen, you want to enable the object, draw the squares in using your tilemap editor (can just use any tile, collision tile for example), then re-disable the GridProperties. This will populate the scriptable object.

Aside from populating the scriptable objects, we should make sure to keep GridProperties disabled during game runtime

A* is actually not too complex to understand (lecture 86 does a good job), but I started working on implementing A* since we will use that pathfinding in our game.

# 8/30/22 
Not much tbh, spent the whole day doing 385 shit lmao, still isnt even working. Also I probably shouldnt have skimped out on a bunch of stuff.

A* is in the works

TODO: 
- Figure out where to save the gridPropertiesDetailsDictionary object (probably in gameState?)

# 8/31/22
Ok I was thinking about how to make the camera pan to the bus when the bus cutscene occurs, I conclude that I have to put the bus in the persistant scene. Same with the lamp. We should put all that crap in the same scene since Cinemachine Track cannot see the virtual cameras if they are in different scenes.

Ok update, after 3 hours of tinkering I got virtual cameras in the timeline too! Now bus riding scene looks pretty solid. Only thing I really had to do was move the Bus out of the OutsideHouse scene, into the Main scene. The key is the use an **activation track** to easily dictate when the bus should be rendered!

# 9/3/22
So in A*, we call BuildPath which calls PopulateGridNodesFromGridPropertiesDictionary which looks for a `Dictionary<string, GridPropertyDetails>`, where the key (of type string) is something of the form "X3Y10" for example, symbolizing the location 3,10 on the tilemap, and the value is a GrdiPropertyDetails object which is just some info from the scriptable object.

It gets this dictionary from the SceneSave object, which comes from the GameObjectSave field on the GridPropertiesManager. This GameObjectSave is instantiated on Awake, and stores a dict of string (scene name) to SceneSave objects. TBH its hard to explain, just look at code.

Just taking some notes as well- to make NPCs follow a specific path, paint on the Path tilemap and then set the Default Movement Penalty in NPCManager script (which has AStar on it) to a high value, that way Astar is forced to use our path to avoid high costs.

# 9/4/22

Ok tbh I feel like they complicated A* a bit, but IDK yet, just finished the main lecture and my character moves (finally). There's a bunch of pitfalls that I think lecturer could've made more clear, basically in the Scriptable Objects for GridProperties, the OriginX and OriginY fields are **relative to the tilemap!** You can find what this value is by using the Tile Pallete's inspector mode and then just clicking on the tile. Same with the ~Finish Position~ field in the AStarTest script as well, needs to be relative to the tilemap. This took me a while to figure out. When we say "relative to the tilemap" this means taking the world position and **subtracting** whatever the tilemap's transform is in the editor.

Also the code function call tracing is just confusing, for example there are like 3 buildpaths, NPCPath's calls MPCManager's which calls aStar's. Why can't we just call aStar's directly? IDK. But it would make things simpler. 

~~Also, if I'm not mistaken in this implementation, we are running A* every frame? This seems really time consuming... I think we're running every frame because if you look we can agree that AStar class' FindShortestPath method runs the whole A* algo, and FindShortestPath is being called by BuildPath, which again is ~~ 

Never mind, so basically what's happening is actually pretty smart. We are running A* once (hence why in AStarTest, we immediatelly toggle off the MoveNPC boolean, because we only wanna run AStar once), and while AStar is running, another while loop (in FixedUpdate, class is NPCMovement) is sitting and staring at the stack that AStar is populating. It is constnatly checking how many entries are in the stack. The instant there is more than one entry in there, we will go to that next location. It's quite smart.

Anyways A* is working now. My character is moving and animating, but looks absolutely horrendous, lol. Next step is to probably redraw it or we can move on for now with this character animation and go back later once we got all the infrastructure setup and then redraw things. Dunno. Probably will moveon to cross scene movement for now.

Ok also I just realized I've been drawing my Pixels per unit all randomly. Probably shoulda stuck with one number (likely 16 is the best option, since its way less work to draw)... but I've been doing 16, 24 and 32 like a total dumbass. And I definitely have resolution issues...

I added the Pixel Perfect Camera back, catch is you can't zoom since by definition the camera is at a single resolution... So maybe I can edit it programatically? And from now on, I will draw all my sprites at 16 probably and hopefully it works OK enough...

Ok important other change, **if I make any more cinemachine Virtual cams, be sure to add under "Extensions", the Pixel Perfect extension! Because otherwise it also won't zoom properly, like the ortho size. If you add it though it will work.

# 9/6/22

Implemented a way to distinguish between the obscure item fader collider and the dialogue activate collider, by having the dialogue activate sit in a child object on the layer called "Dialogue Colliders". TBH I'm not sure if this is the best sollution but whatever. 

The reason why we needed this is because if we put two colliders, both with trigger, on the same gameobject, with a dialogueActivate script, then when I enter the dialogue activate collider (which often sits a little bit in front of the object), the sprite will fade. Similarly, if I press "E" inside the fader collider (which often sits behind), the object will activate dialogue. We don't want this, obviously. It's not the end of the world either, but still, its nice to have separation.

For **Routes**, simply change the so_SceneRouteList scriptable object. Or if you're confused watch lecture 90 (@20min). A weird thing is how we sometimes set positions to "999999", or six nines. What this means is to use either the start position or end position. 

Also, it seems like cross-scene movement (the way that they implemented it) has the NPCs **moving** in the other scenes, **even if we are not in that scene**.

TODO: There's still something really wrong with npcMovement.npcCurrentScene... And IDK what it is. But I bet that if I can solve this problem (which I earlier masked by removing an if-statement), then I can probably get this whole thing to work... Problem is right now its not even getting the right scene route because it thinks that npcCurrentScene is Bedroom, when it should be something else... I did try setting it manually to Campus, maybe I should do that again tomorrow and see how the execution of the code changes.

