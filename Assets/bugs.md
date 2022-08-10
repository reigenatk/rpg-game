Bug: Scene tilemap not rendering
Fix: Make sure the z-value of the tilemap grid is 0, our cam is at -10, so it needs to be below

Bug: Tilemap seemed to keep yeeting my character out of position.
Fix: The bounds confiner wasn't set to trigger and hence kept colliding with my player- fml I spent an hour on this.