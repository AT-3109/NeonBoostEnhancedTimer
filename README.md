# Enhanced Timer

A MelonLoader mod for **Neon Boost** that adds a speedrun.com RTA timer alongside the game's IGT.

## Features

### RTA Timer
- Displays a real-time RTA timer below the in-game IGT timer
- Starts on first movement input (WASD) or rocket fire (Fire1)
- Stops on the frame the results screen appears
- Styled to match the game's existing timer (same font, color, outline)

### IGT Enhancement
- Shows milliseconds when under 10 seconds, deciseconds when 10+
- Adapts precision to the current time

### Best RTA Tracking
- Saves best RTA times per level to `rta_best_times.json`
- Updates automatically when you set a new best

### End Screen
- Displays RTA time on the results screen (configurable)
- Shows best RTA time for the level

### Level Select
- Shows both IGT and RTA times in the level select menu
- Format: `12.3  RTA: 0:34.567`

## Settings (MelonLoader Preferences)

| Setting | Default | Description |
|---------|---------|-------------|
| RTA on End Screen | true | Show RTA instead of IGT on the results screen |
| RTA in Level Select | true | Show RTA times in the level select menu |
| Show IGT Labels | true | Add "IGT:" prefix when showing IGT times |

## Installation

1. Install [MelonLoader](https://github.com/LavaGang/MelonLoader) for Neon Boost
2. Place `EnhancedTimer.dll` in the `Mods/` folder
3. Launch the game

## Building

Requires .NET Framework 3.5 compiler and references from the game's `Neon Boost_Data\Managed/` and `MelonLoader/net35/` directories.

## RTA Rules

- **Start**: First frame of movement input (Horizontal/Vertical axis) OR firing the rocket (Fire1). Crouch/slide is excluded.
- **Stop**: The frame the results screen appears (GameOver Canvas becomes active).
