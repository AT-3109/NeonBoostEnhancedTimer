# Enhanced Timer

A quick and dirty mod that adds an RTA timer in-game as well as on the level end screen.

Tentatively approved for use in IL speedrunning.

## Installation and Usage

1. Download [MelonLoader](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.2) and install it to your 'Neon Boost.exe'.
2. Run the game once. This will create the required folders. You should see a splash screen and a terminal if the modloader was installed correctly.
3. Optionally, download [Melon Preferences Manager](https://github.com/Bluscream/MelonPreferencesManager/releases).
3b. Download the latest version of [UniverseLib](https://github.com/sinai-dev/UniverseLib/releases).
4. Download the ExtendedTimer.dll from [the releases page](https://github.com/Hyonk-Tea/NeonBoostEnhancedTimer/releases)
5. Extract all of the downloaded mods from their .zip files, and deposit them into your Neon Boost/Mods/ folder.
6. Launch the game!

## Building
### Requirements
- Visual Studio 2026 w/ .Net Desktop Development
- .NET 4.7.2
### Process
1. Create a new sub-folder in the project directory called `lib`.
2. Copy all Unity `.dll` files and `Assembly-CSharp.dll` from `<gamedir>/NeonBoost_Data` into `lib/`.
3. Copy `0Harmony.dll` & `MelonLoader.dll` from `<gamedir>\MelonLoader\net472\` into `lib/`.
4. Open `NeonBoostEnhancedTimer.slnx` with VS 2026.
5. Build the project as release.

As of release v0.1.0-beta, you can tell that it's working if the new timer appears right below the old one.

## Additional Notes

Once you've confirmed your MelonLoader install is functional, make sure to add `--melonloader.hideconsole` to your game launch properties (Neon Boost in your Steam library -> properties -> launch options at the bottom of that window). This will help your game launch faster.

