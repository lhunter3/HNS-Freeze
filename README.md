# CS2 HNS Freeze

HNS Freeze plugin written in C# for CounterStrikeSharp.

Works with HNS & Zombie Escape

## Progress
- [x] Decoy Freeze (Configurable time,radius and targetted team)
- [X] Decoy Freeze Radius Effect Similar to CSGO
- [x] NoFlash (Configurable targetted team)
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
  "DisableNoFlash": 0
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
  "DisableNoFlash": 0
}
```


## Setup for local development
Update the CounterStrikeSharp Nuget package if needed. 

## Issues 
Create issues with any issues you encounter!

## Contribution
I am unaware of any other HNS related plugins in the works. For now I am simply focused on delivering the essential barebones features. Please feel free to create a pull request or suggest some features I am overlooking. 
