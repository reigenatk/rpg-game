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

- Create a Scriptable Object for tilemap properties, populate the Origin X, Origin Y, Grid width and height fields. Hook that up to the list in GridPropertiesManager, and also to the "Is Map" and "NPCObstacle" tilemaps.

- Specify the offset of the tilemap in NPCManager's `Tilemap Offsets` list. Important! Without this, NPC movement will not work (or without the stuff above, too, as I spent 45 min trying to debug one time)

- Hopefully it works!?

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