=== 0.0.1
- An earlier prototype scene called RotatingCylinder is also here but ignore it. It was deprecated upon learning that the reel should appear flat to the user and not on a rounded surface.
- Symbols fall down in the screen giving the illusion of one big reel
- The reel always stops at the specified index (which is currently 168 in this release)
- The "speed" of the reel is controlled by the value that increments/decrements a ReelStrip object's y position. This is changed within FixedUpdate for consistency.
- The way entire system works:
    1. Upon startup, instantiate a ReelStrip A (which then creates N symbols as its children)
    2. After ReelStrip A stops on its target symbol, its parent (Wheel_v2) instantiates a ReelStrip B
    3. Based on a manually entered integer H , ReelStrip B takes H Symbol_v2 objects that are in-view (in this case indexes 169, 168, 167) and makes them the head of its reel
    4. ReelStrip B then generates the rest of its symbols (count = N - H)
    5. ReelStrip A gets destroyed, ReelStrip B starts moving downwards and the cycle happens all over again