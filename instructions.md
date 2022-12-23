Personal notes:

DEBUG: 
P to freeze time ticks
T to advance time
Q to be able to talk to ALL npcs again

**Starting scene name is ActualDarkScene, game day should start at 0** 

Dream is always the value of the next day. For instance Dream 0 plays when day == 1 since we call gotosleep (which increments the day #) before dream world.

Start the NumSecondsAwake at 12.75*3600 aka 45900, found that to be a decent number since Day 1 you cant leave the house and there's not much to do so we don't want player to wait forever. (50400 is the number btw that u need to sleep
)

There are 14 NPCs

Conditions to win game: 
1. Text at least 2 of: Doomer Girl, Pepe, Doomer
OR
2. Total friend progress is at least 10 (total is 14 characters, all of which has max progress = to 1?) Meaning -1 will subtract from the total.

You **lose** the game when all combined scores for all 4 categories are less than 30%

Cutscenes other than dream cutscenes that we still have to do:
Football
BustingTheCoomer
Gamers

DONE dialogues for:
coomer
doomer
zoomer
boomer music