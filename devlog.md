# General Stuff

- When making cutscenes, always do a `Cutscene Isn't Playing` signal at the end! (Should've named this Cutscene done playing instead lol, too late for that tho)

- So I setup two kinds of teleporters, one where you just hit the collider and insta-teleport, the other where you have to "knock". The knock is basically where the door is closed under some special condition (which we can specify), and we have a chance to knock on it. And then depending on whether or not we specify a knock dialogue to play or not, we can trigger an additional dialogue. Kinda a bad design on my end, but whatever.

There's also a `Is Door` option which can bypass this whole knocking thing entirely (which I only really see being applicable to the house scene anyways, lol), and just plays whatever dialogue was specified under the closed door condition. And of course, if the door is supposed to be open, then none of the conditions will be met and the user is just insta-teleported.

For example, the scene between the outside house and campus has a teleporter that we want to prompt the user with a dialogue with, of "are you sure you wanna walk and lose some energy?" So this would have isDoor **unchecked** cause it makes no sense to knock when walking to campus. We set a special condition for ALL days 24/7 that has a special "do u wanna walk" dialogue. Then the behavior would be that it would simply play the "are you sure u wanna spend energy" dialogue every time we hit it.

Also the `knock cutscene to play` field is just the name of the cutscene that plays when we knock. Leave blank if not a door.

# Things to do if you want to make another scene
Warning- its kinda complicated. Didn't really design the game with making scenes being an easy process in mind, oopsies.

- Create a new enum in Enums.cs, also create the `hasEntered` followed by the name of your scene variable. **Make sure that the scene name and the enum are the exact same!** Since we rely on parsing strings to enum.

- Add an entry to LevelLoader, both the initial zoom and also the enum of the scene itself. It's the first two lists, pretty obvious where it is. And specify default spawn location if desired.

- (FOR NPC MOVEMENT ONLY) Create a Scriptable Object for tilemap properties, populate the Origin X, Origin Y, Grid width and height fields. Hook that up to the list in GridPropertiesManager, and also to the "Is Map" and "NPCObstacle" tilemaps. as a reminder, the Grid Origin has to be a tile coordinate that is left of and below all the tiles that you are using in your scene. The reason we need Grid Origin is basically this: we represent the tilemap as a 2D array. Thus if we use a tile with negative coordinates (say tile -1, -1 on the grid), then we will get an indexing error. Thus we can first say OK, the origin is at -2, -2, so the point -1, -1 is actually (1,1) RELATIVE to the origin of -2, -2. Because (-1 - (-2) = 1). 

- (FOR NPC MOVEMENT ONLY) Specify the offset of the tilemap in NPCManager's `Tilemap Offsets` list. Important! Without this, NPC movement will not work (or without the stuff above, too, as I spent 45 min trying to debug one time)

- (FOR NPC MOVEMENT ONLY) Make sure to populate the Route scriptable objects with appropriate entries on how to get into the rooms.

- Adjust the bounds confiner, make sure TRIGGER IS SET

- Hopefully it works!?

# Things to do to make new character
- Duplicate existing character
- Change all animation clips to appropriate values, create all animations
- Set their starting location on the map, also set their current scene to whatever scene they will start in
- Create a new schedule Scriptable object, hook that up. And then populate with whatever schedule you want this NPC to have.
- Write a dialogue node called [npc_name]_Dialogue. So if new NPC is called Bob, then it should be called Bob_Dialogue. Then go to the Dialogue Collider on that NPC and set that as the starting dialogue node.
- Make a `headshots` scriptable object with all the head pictures and put into the Headshot object (under Dialogue System, Canvas, Line View, Headshot)
- make a new typewriter sound enum in `SoundItem.cs`, and link that up to the switch statement in `AudioManager.cs`
- Make sure variables work fine, for example `canTalkToAllNPCsAgain` in GameState will need updating so your character can get talked to again.
- Profit

# Time

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

# Dark scene ideas

On iPhone lol

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

# 9/7/22

OutsideHouse tilemap offset: -32.3, 40
Campus tilemap offset: 48 40

So the coordinates 63 -69 are in OutsideHouse, which turn into 111.50 -28.5, that's coming from (campus tilemap + coordinates)

We want (outsidehouse tilemap + coordinates), so all we gotta do is subtract campus tilemap offset and add outsidehouse offset.

OK finally figured out the problem on why cross scene movement wasnt working. Now I'm fairly sure it works, since I got it running.

Two major fixes, one was that in Rob's tutorial, all the tilemaps are centered at 0,0,0, meaning that any coordinate offset into the grid is **the same for ALL scenes!** But I didn't pay attention to this detial, instead I made my tilemaps have different offsets. This is nice cuz I can see two scenes at once without them overlapping, when I'm editing, but downside is that his code doesnt work anymore. So I made a new list of offsets inside NPCManager to account for this. Then inside of the method `GetWorldPosition`, we search for the offset of the referenced scene each time. So **whenever you make a new scene, you gotta put the offset of the tilemap of that scene into this array**, else it wont calculate the real world coordinates correctly and the cross scene movement wont work.

Second fix is, whenever you have an NPC character, you **MUST set their starting scene location via the "NPC Current Scene" field in NPC Movement**, else there's this annoying If statement that wont run in NPCPath, meaning that the stack never gets populated, meaning that nothing ever runs. IDK how to fix this tbh, I checked a ton of times on his original code and still dont understand how this value npcMovement.npcCurrentScene is being set. So I guess we manually set it. Rob had it as a HideInInspector, but I just made it SerializeField so we can change it.

Also, **the lower the number, the higher the priority** for NPC schedules! A "1" is done before a "2".

Did Pause menu lecture 71 until 10 min... UI is way too boring and I have so many issues :cry:

For Saving, the SaveLoadManager will call ISaveableSave() and ISaveableLoad() on all gameobjects that have **registered** with it. So it's a big for loop. So `SaveLoadManager` is also the one that writes it all to file. Those changes are at around 21min lecture 74.

# 9/8/22

Abit lazy today, but OK we worked on save system more. Generate GUID is important btw because you add it on every **gameobject** that you want to save, and it acts as a key into the SceneSave's dict object. And since it is unique, when the game is re-started, the value is guaranteed to not coincide with any other gameobjects' GUID, and you can easily restore all of that gameobjects' specific data (what data is saved is decided completely by our implementation of ISaveable)

# Things I've learned (general things)
- RPG games are hard to make
- Scriptable Objects exist and I should use them more (instead of spamming SerializeField)
- Implementing a Game saving system (surprisingly not too bad, in this course we used an interface called ISaveable which implements a bunch of methods across all GameObjects that we want to save)

# 9/22/22

LMAO 2 week break.

Anyways I'm back. Drew up Lecture Hall scene. Also learned an interesting trick, you can set enum values to say 1, 10, 20, 30. That way if you wanna insert a new enum  between two that you've already defined, you can by say setting value = 5 (if you wanna put between first two). I ran into this issue firsthand and at first didnt know how to solve it, but now I do.

This is cuz in Unity Editor if you change any of the order of the enums, it will freak out and think you're assigning it a new value. 

Oh also I made a CarManager to dynamically spawn cars on the main roads, that play loud ass music. Just gotta add those cars now... Shouldn't be too bad..

Also just all around trying to fix stuff that is bad or ugly, which is a lot of stuff lol
I really should redo the walls for the house scene. There's so much to do and so little willpower to do it. Is this just the indie dev experience? At least I added some sound today using spatial sound (learned about 3d feature) so now it feels a bit more like a game, but still far from where I would like it to be. 

# 10/13/22

Another 2 week break XD
Ok technically I dev'd yesterday as well, fixed some more weird bugs such as that stupid thing where it kept spamming my console with "No AnimationEvent selected", turns out I did have an animation event on the left sprint, I just didn't see it.

Added a lot more to the Campus Downtown scene, got some cool anims + art done, but still have a decent more way to go before it feels like the real downtown scene.

Added a few more cutscenes.

Also learning a bit about Audio Mixer group atm... trying to decide whether I should use it since I already have something that works reasonable well without Audio Mixer Group (Which I made myself, its the SoundManager + MusicManager scripts). It is a bit hacky, though

OK some notes about sound:

- He makes a sound prefab, which works with the pool manager (something that I didn't make, but apparently it just reuses prefab objects) to create instances of sounds.

- Then he passes that prefab to an AudioManager object which has a script on it. We also pass a scriptableobject to this script. This AudioManager has a `PlaySound()` method and also a `DisableSound()` method. So basically its super streamlined, no more `FindObject<SoundManager>().GetComponent<AudioSource>()`, now we can just do `AudioManager.Instance.PlaySound()`, which is much cleaner and feels more official... Hm, I'm kinda sold but I still wanna see lecture 97 first to see if its really worth it to refactor.

- Audio Mixer groups are basically heiarchical, and have their own tab in Unity under `Window->Audio`. The difference Audio Sources can route to an audio mixer group using the "Output" option in the dropdown. Each group has its own settings such as overall gain from -80 to +20dB, which as you can imagine allows for fine tuning. Also, there's something called `snapshots` which are just presets for the different gains of each audio group.

I am having this weird bug where the cars audio is not playing even though it says it is... Apparently I need to turn up max voices in the project settings?

https://www.youtube.com/watch?v=PGP7Dnys4sY&t=28s

Also maybe screwing with prioritiy might help?

OK **never mind** I figured it out. Just adding a good old `WaitForSeconds` worked. I think what was happening was that Instantiate and Drive() were being called at the same time and hence there was some kinda race condition where the object wasnt quite intiialized yet and it was already tryna play the music. Was cool to look at the Audio Profiler tho.

OK also it's just too quiet, after I made some of the clips louder it sounds wayy beter. Like around -20 Loudness Normalization in Audacity for the rap songs, and -30 for the normal songs.

Also I was destroying objects wrong, should be `Destory(gameObject)`, not `Destroy(this)`

OK final update, I revamped the sound system to match that of the course, its just way more flexible and I like it quite a bit. I didn't end up using any of the Audio Mixer Group stuff tho, don't think my game will be that complex. Finished a few more cutscenes, worked out some intricacies with opening door timelines. Very cool stuff.

Also added some cool pausing functionality between scenes, redid the Union stairs drawing so that it looks 10x better now. Good progress all around, tomorrow probably will continue drawing the downtown scene and also maybe get started on the lecture hall cutscene.

Another thing I could do is rework the Cutscenes into a scriptable object, but I'm too lazy.

# 10/14/22

Basically a **Pool Manager** tries to avoid calling Instantiate and Destroy because apparently they are bad for performance? Too much garbage collection. Internally, pool manager is just a queue of gameobjects that are made in advance so that we don't have to instantiate. So like, we say "instantiate 20 of these objects on the game start" and then they never get destroyed, instead they merely activated and de-activated.

I probably can use this for my car driving mechanism thing since I'm creating a bunch of new car objects then destroying them each time- whatever, maybe later. The only difference is performance.

OK and I finished the lecture hall scene and some other sound management stuff. Lookin good, maybe next we add some characters with unique dialogues to really make it feel like a college campus. Finished drawing the professor and making some weird audio for the lecture (and cutscene where player falls asleep). Also we need to find some way to fill the lecture hall so it doesn't look empty as hell.

# 10/15/22

I made it so that in the code, you cannot have two cutscenes playing at once. Like, one cutscene plays, which triggers a dialogue, which has Yarn call playCutscene, this will simply just stop the first cutscene and start the second. This is to avoid confusing bugs where two cutscenes are fighting over the animator (not that it's confusing enough already lmao)

Okay, the lecuter hall scene is in a playable state. Not the best, but oh well. Can't have everything perfect, I guess.

# 10/24/22

One week break XD I have to stop doing this, seriously. No zero days. The sooner I finish this project, the sooner I get to move onto doing something else. I mean, I know I could probably start working on something now, but
1. I wouldn't be able to devote all my energy to it with this project still looming over my head, and
2. It would just feel bad leaving this project unfinished after having spent so many hours on it already. I need to see this to completion.

Strategies: Work at least 3hr a day. My average is around that. I've gone way higher before, but that's how you burnout. And less is just not enough to get any meaningful work done.

Made stacy sprite, animated it up. Had a weird bug which scared me for a second, basically I was trying to define her schedule and I made the target square of one of the paths an obstacle as well. Thing is, obstacles (the way we implemented it) have no penalty values. Instead, they just are completely unwalkable for NPC. So it made it seem like the NPC scripts were broken for the longest time. Then I realized this and removed the obstacle tile, and behold, it works again.

Also added `H` key to remove HUD 

Oh also, TIL about **Yarn Spinner Declares**

Basically its just an organizational thing that gets ran everytime Yarn starts up. I put it in a dialogue node called Declarations in `Misc.yarn` like so:
```
title: Declarations
---
<<declare $minTilNextBus = 1>>
<<declare $acceptedBibleStudy = false>>
===
```

Another cool thing I did was implement **a more robust spacebar skipping dialogue thing**. Basically, before when you press spacebar it skipped the entire dialogue, but now it shows the whole text on the first spacebar press, then skips on the second. This is much closer to modern RPG games (for instance I think OMORI, Oneshot, etc. all have this system- you can't just skip the dialogue in one press, it still shows the whole thing once before continuing). This is more or less to **prevent the player from skipping the whole story**/ skipping all the important bits. Especially since story is often an emphasis in these types of games.

Implementing seemed daunting but I just read `LineView.cs` in detail, and how it interacts with `DialogueAdvanceInput.cs` and I was able to hack together a solution by only adding like 10 lines, which is pretty sick!

Drew another building for Campus scene. Tmrw will work on revamping Room scene, an maybe add the jerk off cutscene xD 

LMAO another like 8hr workday or something. God my working habits are so unhealthy wtf

# 10/25/22

Jerk off cutscene added :sunglasses:

Also added like 2-line fix so that **I can now keep scenes loaded in the editor when pressing play**, this is huge since on startup it will unload all the scenes that have loaded already. Just a nice QOL as a dev. Shoulda had this way earlier.

Also added arriving home cutscene and halfway done with eating cutscene. God, cutscenes take forever to make. Is this really what fuckin all RPG adventure game devs have to go thru? 

Added different talk sounds for Kabowski and Nikolai

Added different typing speeds, starting to play with Yarn Markup to make cool character effects. We'll see tomorrow about this. 

Maybe if my game was more focused on gameplay as opposed to dialogue it would be different... But I'm tryna focus on the dialogue cuz I want it to be story based.

I made a few important fixes tho. The biggest one being, we now pause non-NPC animators when in a cutscene. By "non-NPC" I mean things that don't move around on a time schedule. For instance, the NPCs in our game move based on what time it is, that is, they trigger movement via the timer. So, if we pause time then the animators should be paused too, or we find out some way to keep the NPCs moving without the time. Not sure how though. See DialogueManager.cs:40

# 10/26/22

Polishing the arrive home scene, made eating scene

I'm planning on doing this: for each character we have two versions an NPC version and a non-NPC. For example. There's a NPC Brain which will follow some schedule (sit in room for X hours of day, go outside for a bit, etc.), and then once a cutscene plays, we will disable the NPC version's sprite renderer and refer to the non-NPC version in the Timeline. I can't use the NPC's since idk how the NPC movement scripts will interact with Timeline, and tbh, I don't wanna know. I have a feeling that it'll be ugly

Note to self, do not use rotation with animations, or flipX flipY bullshit. Just make it in Aesprite. IT's so much nicer not having to deal with Unity's random crap of things just not working. Example: I made animation with a 90 degree Y rotation, since the bed for Brain was horizontal instead of vertical. Then when I try to play this animation, the sprite switches, but the rotation just doesn't change. wtf?

So I just rotated the sprite itself in Aesprite. Still have no clue why it didn't work.

Also major progres today with Brain's daily schedule. I also **got rid of door Knocking mechanism** (so like 2 days of work was for nothing lmAO). But my logic is, why tf have knocking in this game? Just make the other roommates' rooms avialable 24/7, that way its more interesting and you can snoop on whatever they are doing at any time. So now, what you do is you enter roomate's rooms, and you talk to the roomate. **Depending on how good your relation is with them, different dialogues are triggered. Which then trigger different cutscenes**. For instance if you were rude to them in the past and its one of the later days, then they will likely just ignore you or give you little attention. 

Oh also, I made each roommate also an NPC copy as well, made progress of defining Brain's daily routine a bit. Will do the same for other two as well, and perhaps all other characters we add to this game. Hopefully if I add enough characters it'll start to feel like a real small town, with each person having their own little routine? Who knows.

Also still have to draw MainQuad scene, and maybe that's all for the scenes. Drawing campus was hard enough man. But I have to make a quad scene probably, and I can add like grass field and disgusting spikeball players and shit.

Da game is coming together AAAAAA just need like 20 more uniquely drawn characters and some more original music, more good art, and hopefully we're good. Way harder to do than it sounds. At least I probably have like 10min of gameplay already, maybe 15. IDK.

# 10/27/22

I feel like I do way more work than I write in the devlog, cuz there's so many bugs and I fix them and then instantly move to the next one and forget about what I did earlier.

But I did a pretty high profile bug today, basically it was line 20 or something in NPCMovement, and it only became evident once I had more than one NPC (finished Brain's schedule btw). The bug was basically that it was trying to get the coordinates using the grid loaded in the scene (of which there's only ever one, cuz only one scene is loaded at a time).

But the problem is we want it to use its own scene's grid, which isn't loaded. SO to solve this I wrote this method `CrossGridCoord`. It's super similar to `GetWorldPosition` but not exactly the same. Super confusing stuff but just printing a ton of debug statements helped I guess. Fingers crossed it completely works now!

Lotta art done today, started on MainQuad scene, which will probably be my last scene. RN the teleporters are acting strange again (wtf?) so yeah gotta fix that... 

Haha ok my freaking bounds confiner had a collider on it. Not the first time I did that.

But yeah, one more scene to go. Added some funny animations (mostly for kabowski), after I verify his schedule works, I will do Nikolai, and then just focus on all the content (cutscene, storyline, more characters, game progression etc.)

# 10/29/22
Fixed pathing bug, did lots of art yesterday, today did lots of debugging and verified the schedules for all 3 NPCs that I made so far. AKA, the transitions between scenes and all look good enough. So now I might start blindly making schedules without rly looking too closely at them.

Todo is Nikolai's schedule next, and then probably start on the Day 2 go to sleep / dream and also maybe work on the bible study cutscene (classroom is completely drawn!)

# 10/30/22

Finished Nikolai schedule and anims

Just for reference, Unity supports a [rich text scheme](http://digitalnativestudios.com/textmeshpro/docs/rich-text/) which I can use to spice up the dialogues that appear inside of Yarn Spinner's boxes.

Go to `Day2_Brain_Exits_Lair` dialogue for an example. The only problem I have with this method is that it's quite verbose. I dunno if I'll do any animating text. Like the ones they had in Night in the Woods. It was super nice but it also seems like a lot of work. Maybe I focus now on the dialogue first then once everythings done, we come back to this problem.

Now that we have all the NPCs for now, we can add `DialogueActivate` scripts to each, and so now the player can theoretically talk to them. I'm afraid they won't stop though so we gotta do some work there as well to make sure their animation stops and they talk to the player? 

Okay, big changes to some movement and dialogue related stuff. First off, **Dialogue nodes pause time now as well!** I couldn't find a good reason not to do this, the only reason I thought of was that we cannot have NPCs moving in the background now during our dialogues, which would've made the game feel more realistic I suppose. But its too much of a hassle. 

I also revamped the `DialogueActive` script onto the NPCs as mentioned above, required some changes but overall worked OK. You can now talk to NPCs **inside of running animations** and they will stop, look at you, and then once the dialogue is done, go back to doing what they were before. This is huge!

Only problem is, right now talking to NPCs on the move is really buggy. For example, Becky is walking on the OutsideHouse scene. I talk to her, she turns to look at me, but after I finish the dialogue, she doesn't keep going on her merry way. She just get stuck there. Right now I'm not sure why this is happening, but I suspect the answer lies somewhere in NPCMovement so I will go look in there tomorrow.

If I can't figure it out its not the end of the world, we can just make it so you can't talk to NPCs that are moving. But I would prefer you could, to make the game feel real.

Also I setup a sort of "friendship points" system, using Nikolai as an example. Basically each character starts off at friendship score of 0, then if you are mean to them it goes to -1, -2 etc. and if you are nice it goes up. Caps at -2 and 2. Then depending on your score, you will get different dialogue from them.

This sounds hard but isn't really with Yarn variables. The hard part is writing all the dialogue, but that shouldn't be too bad after I get the game setup properly.

Didn't do much art today tbh. But still gotta do the Main Quad scene and then we'll be all good. Oh yeah, I also tested the transition between Day 2 sleeping and Day 3, so we're good to go. Made some changes to DarkScene as well. Overall super productive today :D

# 10/31/22

Yay, so now we got it such that NPCs will continue movement after being talked to. And they will stop when you talk to them. Fix was in `DialogueFinishedPlaying`, I just added this line `NPCBeingTalkedTo.npcIsMoving = false;` which basically says "ok, this NPC has no movement, please assign one to it". The problem earlier (NPC wasn't moving after a dialogue) was cuz npcIsMoving was true, so the game thought that the NPC had a movement already. So we explicitly have to say "no it doesnt have a movement".

The function `CancelNPCMovement` also does a similar thing (I noticed this later) but its what we want plus some extra stuff, which is too much (for example it clears the NPC path which is our A* info, thats wrong since we will still need this after the matter).

K I think I know the problem with the "moving after colliding with NPC" problem.

Basically a **rigidbody** in Unity is just a way of controlling multiple colliders. Both the NPC and player has one. My problem is that the Player, after collision with the NPC, still has his rigidbody velocity set to nonzero value. The only way to fix this is to detect collisions between NPC and player, and if NPC has zero rigidbody velocity, then player must as well. Maybe I could say, OnCollisionExit with player, check if our velocity is zero. If it is, set player rigidbody velocity to zero as well?

Anyways [this thread from 2011](https://forum.unity.com/threads/how-to-make-rigid-body-stop-moving-after-collision.88066/) has the exact same question.

And yay, I fixed it. Now it should be fully working! I just needed to add an OnCollisionExit2D on the Player class. I first tried adding it on the DialogueActivate script and it wasn't working which made me confused (not sure why this doesn't work), but now its ok.

Drew Pepe, tried (and failed) to add wojak). I think some sprites are just too hard. Planning to add one more character (like a quintesential christian guy) into the Bible scene, and then we can do that cutscene tmrw.

Idea: Maybe also do a second club? Like some sport club, or some niche hobby club (like anime or whatever? XD)

# 11/1/22

Damn I did so much freakin art today lmao. Added like 5 characters. World will definitely will feel more full once I finish putting all of them in game. I drew them tho cuz I'm tryna figure out the bible study scene. Still gotta add the guitar player guy, then we're good. Then I can work on the scene :P

This shit is so much work actually xD, but very entertaining to try to draw pixel art versions of wojaks and pepe lol

Codewise everything went ok today, dont think I made many changes at all.

Todo: Add radio to boomer anim, rig up all the animations, finish brad sprite then start work on cutscene. Lots of stuff to do :( but I'm sure once I finish this the game will feel more alive. It's already kinda fun seeing my NPCs run around on the map, and it will def be much more fun when we get even more NPCs.

# 11/2/22

Jesus I think I really bit off wayy more than I could chew with this game. Fuck the steam release, fuck all that shit. This game is so bad XD

Anyways, finished up the two characters I was talking about yesterday. At this point I think I have all I need to make a few club cutscenes. TBH IDK I might just ditch the social aspect of this game. Takes wayyy too many NPCs to make this game feel like a real town. At this point I'm just happy that everythings working, but I can't imagine drawing like 20+ more NPCs (at which point I think I could start saying the town is immersive)... And animating all that shit? No way.

I think again I'll just focus on the cutscenes and stuff, to be honest no one will play this anyway or care about it, and if I don't care about it, then I shouldn't put so much effort. After all I don't think there's anything special going on in this game that you can't find elsewhere, lol.

Hopefully I can kinda finish this game before next semester starts (ok tbh there's no way that's happening if I don't want to churn out a shit game) so I can turn my attention to new things? Let's try to use that as extra motivation to bust our butts these next few weeks and really try to make some nontrivial progress on the cutscenes and what not. Most of the characters seem to be done, I just gotta make the dialogue interesting. 

Another todo might be to research a more interesting dialogue window, and also a way to display a face next to the text. That way there's more emotion attached to the dialogues lol

Again, my goal with this game isn't to make perfection, I just want to convey emotion and give the player a glimpse of my college life. 

Tmrw: Sort out the classroom scene, finish the animators for other NPCs, and hopefully start working on some cutscenes!


# 11/3/22
OK, all schedules have been officially added. Hurray. Campus definitely feels a little more lively, surely the player will run into a few people now (unless he holes himself up in the room).

Now I will work on church cutscene and dialogues. Added lots more audio today as well, some really troll ones in there xD

# 11/4/22

Thinking heavily today about headshot, u know every rpg has it, where it shows the character's face on the left side of the screen. That way you get a close up and it feels more realistic ig?

Problem is I need to read in each line of dialogue, and see who is talking. Then also somehow I need to be able to pass in using Yarn, which face I want. I was thinking about using a YarnCommand each time we wanna change the face, but now it seems a better solution is using [Yarn tags](https://docs.yarnspinner.dev/getting-started/writing-in-yarn/tags-metadata)? Then we use the `nameUpdated` function in `Headshots.cs` which really is called by the `OnNameUpdate` function in `DialogueCharacterNameView.cs`, to extract the metadata of the line, and hopefully see whether or not we should use a default headshot or a custom one.

Complicated! Let's see if I can get this working. Because the alternative is just, we talk to people and they just talk and stuff but there's no emotion involved.

Ok for future reference when doing **metadata** we put something like `#e:slight-smile` in the yarn dialogue. I chose "e" but it could be any word or letter, this e stands for expression. And then slight-smile is the name of the string in the scriptable object, which has a corresponding sprite. When the metadata is put into a String[], the elements then look like `e:slight-smile`, so without the `#` symbol. Therefore I can now get rid of the first two letters to get my desired string, which we will go find the sprite with. And if there is no metadata then we can assume we want the default sprite. Cool.

OK I think the headshot system is working. Suprisingly not that terrible, hurray for Yarn. Not hurray for having to put tags in all my Yarn dialogues now xD

At least no tag = default so that's a net positive.

Also just a sidenote, **stop moving scripts around in the file heiarchy**, it keeps causing these weird "Missing Script" bugs to appear in Unity and then I have to re-enter in all the fields. Really annoying.

# 11/5/22

OK I changed the player anims all to the Robot character. We're committing to that. Also I am pretty much done with church cutscene, just gotta find non cancer sounds for each of the players. Harder than expected.

# 11/8/22

Ok finished fixing a few bugs, biggest one was that at end of the scene it would just freeze, the problem was the way I was advancing time. I fixed it using two things: First we now just **instantly set the time to the value we want**, instead of using the for loop to call the `UpdateGameSecond` method a few thousand times. Then you might say, well then the NPCs aren't gonna move properly.

Not anymore. I added the ability for NPCs to move between scenes, even if there is no scene route between them. Adding this was actually pretty easy since its already inside the NPC Move coroutine whenever the time lags behind. The only problem is that it doesn't look realistic since the NPC just teleports into the right place, but thats totally fine for me. So now you can hop around in time and NPCs will still move around just fine. Perfect.

Finished Bible Study scene, finished bloomer dialogue (general), now I was working on the coomer dialogue and a cutscene where you bust him for being a stalker.

Made a really retarded scene where cops chase the coomer around lol. Still doesnt work yet, theres this weird bug where the cutscene doesnt run cuz the animators are supposedly disabled on the non-npcs... wtf?

# 11/10/22
Honestly I have stopped writing daily devlogs cuz sometimes there's not enough stuff to put in it for one day. And I don't wanna just write in it for the sake of writing in it. 

Tday and yesterday was just working on dialogues, here's a design decision that I thought I would elaborate on:

So everytime I start a dialogue node I disable all animators, and everytime I exit dialogue I enable all animators. This is the whole `npcs` and `nonnpcs` objects that I am putting into the LevelLoader and GameState scripts.

Then when a cutscene starts, we disable all **sprite renderers** on the npc versions, and enable all the renderers on the NON-NPC versions. Then when cutscene ends we do the opposite of this. The following system seems to be working fine.

# 11/13/22
So I haven't written a devlog in a while, was busy making zoomer char, and finishing up all the dialogues. I am at a point now where ALL dialogues have been completed, yay!

So I'm going to start turning my attention to making dark scenes and cutscenes again.

I felt like going over the way I implemented dialogue system for this game. It's a bit sloppy, and on a second pass of designing this game I would do a better job. But for now here's how it works.

You start with 0 friend progress. Friend progress is either -1, 0, or 1.

The thing is, it can only be 0 on the first encounter. Every time after that its either -1 or 1.

-1 means unfriendly. YOu will get some hostile dialogue from the NPC, and then you can either choose to keep insulting them or apologize, which will send your progress to 1. 1 means you are on friendly terms, and you will have a host of dialogue options to choose from.

The thing with friendly dialogue options is: There's two kinds. The ones where you can insult them and send progress back to unfriendly terms, or the ones where you just listen to what they say, and gain a passive bonus to your stats.

The ones where you insult them are filtered for first, because they are typically more elaborate (with choices and what not, and therefore more fun to play for the user). I will check if they have all been completed. If not, we prioritize playing those first. Then once all those have been done, we do the ones where you just listen and get a social bonus, next. You can go into any of the Yarn files for the NPCs, and look at the nodes with the ending `_Decide`, to see what I am talking about. For example here's Zoomers's Decide:

```
title: Zoomer_Chat_Decide
---
<<if $didZoomerDance == false>>
	-> Dance
		<<jump Zoomer_Dance>>
	-> Talk about something else
<<endif>>
<<if $didZoomerFashion == false>>
	-> Help
		<<jump Zoomer_Fashion>>
	-> Talk about something else
<<endif>>

<<FadeIn LevelLoader 1.0>>
<<wait 1>>
<<playSoundString AudioManager TimeTicking>>
<<wait 6>>
<<if $didZoomerDialogue1 == false>>
    <<set $didZoomerDialogue1 = true>>
    <<jump Zoomer_Chat_End_1>>
<<else>>
    -> Dance 
        <<jump Zoomer_Dance>>
    -> Help <color=\#FFB6C1> (already talked about this)</color>
        <<jump Zoomer_Fashion>>
    -> Generic Chat
        <<jump Zoomer_Chat_End_Generic>>
<<endif>>
===
```

Here, the first two if blocks check if the longer dialogues have been completed. IF so, we skip them. If not, we will give the user an option to talk about it. The thing is, the player has already seen these options (help and dance) from the first encounter, its just that they picked one of the two, so the other one is presented here as a possible dialogue option. IF they elect to skip it, then they go into the dialogue nodes where the character just talks for a while. 

And finally at the very end (in the else statement), you see I bring back all the nodes again. This is because I want the user to be able to screw with all the dialogue options (insulting vs complementing), and see how the benefits vary for each. Also I think insulting is just fun, and the game would be boring if they just were a yes man the whole time.

So yeah, in summary **I tried to design the system such that any player will see the most unique dialogue as possible**, as opposed to seeing the same dialogue over and over again. This is both in their interests (so they dont get bored), and also in my interests (since I created many dialogue paths, and **it would be a shame if they were never explored by the user due to poor game design**). By adding all the dialogue options back at the end, I hopefully allow the user to insult the NPCs a bunch and see what happens when they say certain things.  Hopefully I succeeded.

(Also as you can  see above, I considered adding a small message next to the dialogue that tells the user they already talked about something. But I figured against it, since I don't want there to be too many hints in the game. And also I think it would just look kinda weird there.)

Also I should talk about the rewards for dialogue. Basically it affects everything but energy (so social, contentedness and entertained). Both social and entertained are always positive, but contentedness can be negative depending on if you insulted them (negative) or was nice (positive). Also, the values are higher/lower depending on the pre-existing status. So for example if you are already friends and being nice that would be a +10 for example, vs if you were not friends and are being nice then its +5 only.

# 11/23/22 
LOL took another 2wk break

I've been spending that time "brainstorming" ideas for the dark scenes.

Maybe Day 1's dream can be childhood things, Day 2 is for teenage age (pre college)
Day 3 is for young adult life (right after graduate), Day 4 is boomer, Day 5 is when we die... I think that's a good plan.

Prolly will try to compose unique soundtrack for all of them?

**Childhood**: Ideas are, digging in the dirt during recess in a park to find cool insects with another friend, being talked about behind your back. 

End scene: Getting beat up once by a bully after trying to stand up to them, and then crying.

Items: Pokemon Cards, Halloween Candy, stuffed dolphin. Parents are nice and normal still.

**Tennage**: Sitting alone during lunch, Parents arguing/yelling at you, hardcore studying in a tight room. 

Friend gets told to stop coming to house.

Maybe also an ugly character to symbolize college admissions, and then having the parents be slightly wackly drawn, and also have them worship this ugly character (since my parents worshipped me getting into a good college as the only objective of my young adult life :D and they still do to some degree)

End scene: ugly character "college" rejects us and parents are devestated (symbolizing how all my "work" for college was for naught since I didn't get anywhere going to IMSA)

Items: Homework packet, pencils, ticking clock in the song? 

**YA**: 

You see friend again at some random place. He's a total druggie?

Small apartment building, wagie job, drinking alcohol, parents are OLD. Have to take care of them. A failed date due to deterioration of social skills. 

Staring mindlessly into a screen that blares random audio 

Items: Paintbrush. Self help books. Empty alcohol bottles. Paperwork (for sick parent), walk around and see married couples taking their kids out to play at the same park that we played in during childhood. 

**Boomer**: Parents die as symbolized by us lowering the casket twice. Start to get pain everywhere. Too tired to think anymore. Too little energy to keep playing guitar. 

End Scene: Attempt at suicide with gun. Gun is unloaded. Conclusion is, we are too pussy to kill ourselves

Items: Pills. 

**Death**: Dying alone in a hospital bed. Beeping sounds. No friends or family surrounding us. Doctor asks if we have any family to send our belongings to. We want to say no, but our voice is gone. Slowly shake head, and then die.


# 11/23/22

OK I added a third category of characters, right now we have NPCs, non-npcs, and I'm also gonna add `AlwaysShow` group under the `Characters` object, which is gonna be for stuff like in the dream scenes that arent NPCs (i.e they dont move around) but also they are animated, so we wanna pause the animators on them when cutscenes stop and play the animators on them when cutscene is playing.  

# 11/30/22

So devlog has been very sparingly updated as I mentioned before, but progress is still being made. Lots of artwork being done. Dreams 1 and 2 are basically complete.

Added a lot of QOL stuff, so for instance in order to wake up from the dream and go back to the bedroom, I added a `WakeUp` signal on LevelLoader. **MAKE SURE THAT THIS SIGNAL IS RAN AFTER "Cutscene Isn't Playing" signal btw!!**

All this signal does is unload whatever scene is currently playing and then load the Bedroom scene.

I also made a lot of changes to `SceneTeleport.cs`, for example, now you can make a sprite appear whenever a teleport condition is met, and also you can choose as to whether or not a dialogue will end with a teleport or not.

I use this in dream 1 where I have a special condition that runs the Nothing dialogue, followed by no teleport, which makes it basically do nothing. And then if a condition is met, I make it do the dialogue I want, followed by a teleport. This way you can conditionally turn on and off a teleporter in the scene.

Also, I created a `PlayerLoad` script that basically conditionally loads stuff depending on what scene it is. I was stupid and didn't separate out the dream tilemaps (mostly because I wanted to reuse the cutscene of the player rowing in) and this makes it really ugly because certain cross-scene sprites from other scenes are rendering when they aren't supposed to. So to fix it I just need to put a `PlayerLoad` script on all the cross-scene objects in order to indicate when they should appear and when they shouldn't. Simple enough. Only downside to this approach is that they are always appearing when I'm editing. But that's ok, I can disable them and enable them all once I'm done.

# 12/4/22
So some days for the past week have been OK, others not so much. But I'm done w/ day 2 dream as well, so just adding some cutscenes for Day 3. One thing I just noticed (that maybe a problem) is that for cutscenes that aren't triggered (that is, are loaded on entrance to a location), we may have a slight problem in that each time the person walks to this location, it will trigger, if the only filter is the day and location. SO I will have to create more yarn variables to filter this, meaning everytime a cutscene finishes we set a variable so it can only ever trigger once.

# 12/5/22

CANCER ALERT

So I finally solved the problem of the audio listener being weird (showing that I'm distance 10 from something when I'm right next to it). Turns out the audio listener was on the main camera the entire time, which is obviously not moving exactly where the player is (since its a follow Cinemachine camera... which stays in the middle of the screen lots of the time). To fix, I changed this to be on the player, and voila, now it is accurate reading (meaning, 0 is right next to the object). This sucks though, cuz now I have to go tweak every single spatial sound object that I made... God damn it.

The default I was using before was Lienar Rolloff, min distance 10 and max distance like 12. I think the new one will be linear rolloff, min distance 0, max distance 4 or 5. More or less the same thing if you think about it. Just actually accurate this time. Grrrr, I shoulda found this much sooner tbh.

Also, I added another form of progression. Basically, three characters (Doomer guy, doomer girl, and pepe) have further dialogues that you can unlock by going on your computer. But you must first get their social medias, which requires you to be good enough friends with them first. The game notifies the player when one of these dialogues have been unlocked via the `NotifyMessages` cutscene, which occurs in the player's room. Once he goes on the computer, he can then choose to "text" his friends.

And once all three friends have been texted at least once, we unlock the last scene in the game, where Robot invites over his three new frens to his house.

# 12/7/22
A tough design decision I had to make was that I was tryna debate whether or not to create two version of each character- a non NPC and an NPC version, the only real difference being that the NPC version of the character is governed by the NPC movement scripts (which is running A*). 

The benefit of using only the NPCs is that in the cutscenes, I could then (theoretically?) move the literal location of the NPC to the end position of the cutscene. And then, I could add dialogue on top of the NPC player characters that wait for certain times, so that the cutscenes could have like "in between" moments where the player can walk around a bit (but the other characters stay put!) Cause, the difficulty in maintaing pairs of each character is that the locations don't sync up. That's why I have so many cutscenes that end with the non NPC characters walking off screen... cuz its easy that way. Because once the cutscene ends, the non NPC characters will dissapear and the real NPCs will once again have their sprite renders enabled...

But it's more complex to "just move" the NPCs to the right place. First of all, they operate on a time and grid basis. And time is paused when cutscenes run (this seems logical). Second of all, player NPCs don't show unless they are in the specified scene (by the way we coded NPC movement). And there's no guarantee that the NPC character that we add to our timeline is in the same scene that this timeline takes place. And so we would somehow have to change not only the position of the NPC, but also its "Current scene" field, then start time up again, to somehow put it into the right place.

All this considered made me decide to just stick with two separate versions. 

That aside, today I got some good work done on the game failing cutscenes, lots of dialogue, also started work on two scenes for "winning" and "losing" the game.

WE'RE GETTIN THERE!

# 12/13/22
Finished Dream 3,4,5,0 (dream 0 is actually the dream for the first night, I decided to make it after all cuz why not? It's the first thing the player sees) I know naming scheme is really shit cuz this dream actually happens on Night 1... But whatever.

Anyways I'm now onto tryna run the game. Gonna play thru it as if I was a real player and try to tweak what I don't like. Do this for a few weeks and hopefully it'll be good enough so that I can just release it.

Also need to work on footstep sounds. That's another big one, maybe I do that tmrw actually (before doin any playtestin)

But yeah huge art progress, made a few piano songs, quite happy with how dreams 3+4 came out. I think I really captured that depressed mood quite well. Need to make songs for a few of those dreams tho. As well as maybe a nice song for dialogues, or for certain characters, etc.

# 12/13/22

Got footstep sounds workin.

Now my strat is to go thru each cutscene and its usecase, see how it runs and tweak whatever is necessary to fix.

Changes: NPCs will NOT appear on day 1. This is to preserve the feeling of empty day 1 I guess.

Alright, so I verified that everythings working OK up to Day 1 dream. Tmrw we can start with Day 2!

# 12/14/22
Almost had anothe heart attack after thinking NPC movement broke again... Turns out I just added something that didnt ned to be there, specifically the grid was null in `AfterSceneLoad`, but I had added something, so after removing it the thing worked again.

God damn I'm just gonna stop touching the codebase because I stg if I break fkin npc movement again im gonna cry

Disabled: Inside Building Tutorial