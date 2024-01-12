# CS2 HNS Freeze

HNS Freeze plugin written in C# for CounterStrikeSharp.

Intended for HNS but can be configured for Zombie Escape gamemode.

## Features
- Freeze Grenade (Decoy)
- No Flash
- No Smoke


## Progress
- [x] Decoy Freeze (Configurable time,radius and targetted team)
- [X] Freeze Area Effect Similar to CSGO
- [x] No Flash (Configurable targetted team)
- [x] No Smoke
- [x] Config 
- [ ] Knife Attack Cooldown (HNS)
- [ ] Decoy Freeze Player Tint Effect (ie blue player when frozen)



## Installation
Download the zip file from the latest release, and extract the contents into your `counterstrikesharp/plugins` directory.

#### config.json
Plugin must be reloaded for config changes to take effect.

#### CONFIG FOR HNS
```
{
  "HELP": "Team 2 is Terrorist, Team 3 is CT, Time is in seconds, Radius in units",
  "FreezeTeam": 3,
  "FreezeTime": 4,
  "FreezeRadius": 200,
  "DisableFreeze": 0,
  "NoFlashTeam": 2,
  "DisableNoFlash": 0,
  "DisableSmoke":  0
}
```

#### CONFIG FOR ZOMBIES
```
{
  "HELP": "Team 2 is Terrorist, Team 3 is CT, Time is in seconds, Radius in units",
  "FreezeTeam": 2,
  "FreezeTime": 4,
  "FreezeRadius": 200,
  "DisableFreeze": 0,
  "NoFlashTeam": 3,
  "DisableNoFlash": 0,
  "DisableSmoke":  0
}
```


## Setup for local development
Update the CounterStrikeSharp Nuget package if needed. 

## Issues 
Create issues with any issues you encounter!

## Contribution
I am unaware of any other HNS related plugins in the works. For now I am simply focused on delivering the essential barebones features. Please feel free to create a pull request or suggest some features I am overlooking. 
