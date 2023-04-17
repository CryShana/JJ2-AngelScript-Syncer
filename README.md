# Jazz Jackrabbit 2 TSF+ AngelScript Syncer

This is a tool that takes a global script file named `global.as` and copies it to every world in the JJ2 directory. 

## Why
I wanted a global script that is active in every world as I replay the game; and I haven't found any existing solution that would offer this besides manually copying my script to like 100+ worlds.

## Usage
- Put the executable in the JJ2 directory next to `Jazz2.exe`
- Create a `global.as` file in the same directory
- Run the executable

## Exclusions
Because sometimes you have custom worlds with custom scripts, you don't want the syncer to override those.

You can exclude worlds by adding a comment to the `global.as` file in the following format:
```as
// EXCLUDE Castle*
// EXCLUDE *Cry
```
Which will exclude any worlds which filename starts with `Castle` or ends with `Cry`.

Don't worry, the tool asks for confirmation before overwriting any files.

## Example global script
Example of `global.as` file:
```as
// EXCLUDE Share*
// EXCLUDE *Cry

bool initialized = false;

void init(jjPLAYER@ play)
{
    if (initialized) return;
    initialized = true;

    play.showText("Global script working");
}

// run on player death
void onRoast(jjPLAYER@ victim, jjPLAYER@ killer)
{

}

// run before 'onPlayer'
void onPlayerInput(jjPLAYER@ play)
{

}

// run for every player every tick
void onPlayer(jjPLAYER@ play)
{
    init(play);
}
```

## Alternative approach
ALternatively, to avoid running this tool every time, you can simply use the following `global.as` content:
```as
// EXCLUDE Share*
// EXCLUDE *Cry
#include "global-logic.as";
```
And then create a `global-logic.as` file in the same directory. This way you can edit the `global-logic.as` file without having to run the tool every time.