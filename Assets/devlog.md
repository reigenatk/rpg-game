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
