# CS2 HNS Freeze
HNS Freeze plugin written in C# for CounterStrikeSharp.

## Progress
- [x] CT Decoy Freeze (5 second witin 200 unit radius) (no circle vfx or sfx yet)
- [x] T NoFlash 
- [x] Config so plugin be used in Zombies aswell
- [x] Decoy Freeze Effect
- [ ] CT Attack Cooldown



## Installation
Download the zip file from the latest release, and extract the contents into your `counterstrikesharp/plugins` directory.

config.json
Plugin must be reloaded for changes to take effect. Default is configured for HNS, but values can be modified for Zombies.
```
{
  "HELP": "Team 2 is Terrorist, Team 3 is CT, Time is in seconds, Radius in units",
  "FreezeTeam": 3,
  "FreezeTime": 4,
  "FreezeRadius": 200,
  "DisableFreeze": 0,
  "NoFlashTeam": 2,
  "DisableNoFlash": 0
}
```


## Setup for local development
Update the CounterStrikeSharp Nuget package if needed. 

## Issues 
Create issues with any issues you encounter!

## Contribution
I am unaware of any other HNS related plugins in the works. For now I am simply focused on delivering the essential barebones features. Please feel free to create a pull request or suggest some features I am overlooking. 
