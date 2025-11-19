# Kingdom Capitals - Mount & Blade II: Bannerlord Mod

**Version:** 1.0.0
**Game Version:** Mount & Blade II: Bannerlord v1.2.12
**Author:** [Your Name]

## Overview

Kingdom Capitals adds strategic importance to capital cities in Calradia. Capturing an enemy kingdom's capital results in the complete conquest of that faction, with all settlements and clans joining the victor.

## Features

### 🏰 Capital Cities
Each kingdom has a designated capital:
- **Battania:** Marunath
- **Vlandia:** Galend
- **Aserai:** Quyaz
- **Sturgia:** Balgard
- **Khuzait:** Makeb
- **Western Empire:** Jalmarys
- **Northern Empire:** Diathma
- **Southern Empire:** Lycaron

### 🏗️ Enhanced Capital Buildings
Capitals can upgrade buildings to levels 4 and 5 (exclusive feature):
- Extended benefits for prosperity, production, and military capacity
- Higher construction costs and requirements
- Linear progression following base game patterns

### 👑 Automatic Capital Transfer
When a ruler dies or abdicates:
- Capital ownership automatically transfers to the new ruler
- No voting or political maneuvering required
- Maintains capital status across succession

### 🛡️ Daily Garrison Reinforcement
Capital garrisons are automatically reinforced:
- **Default:** +3 troops per day (configurable via MCM)
- **Troop Tier:** Based on city prosperity (every 2500 prosperity = +1 tier)
- **Branching Paths:** Randomly selects upgrade path when troop trees split
- **Food Requirement:** Reinforcement only occurs if city has food
- **Reduced Consumption:** Capital garrisons consume 50% less food (configurable)
- **Free Upkeep:** Capital garrison troops cost nothing to maintain

### ⚔️ Total Conquest Mechanics
Capturing an enemy capital triggers complete kingdom destruction:
1. **Direct Transfer:** Capital goes to ruling clan WITHOUT voting
2. **Settlement Seizure:** All enemy settlements transfer to victor
3. **Vassalization:** Enemy clans become vassals (configurable)
4. **Kingdom Destruction:** Defeated kingdom is eliminated
5. **No Distribution Vote:** Capital bypasses normal settlement distribution

### 🎨 Visual Identification
- Capital names display with **golden background (#cbae79)** on campaign map tooltips
- Easily identify strategic targets at a glance

## Installation

### Requirements
- Mount & Blade II: Bannerlord **v1.2.12**
- [Harmony](https://www.nexusmods.com/mountandblade2bannerlord/mods/2006) **v2.3.6**
- [MCM (Mod Configuration Menu)](https://www.nexusmods.com/mountandblade2bannerlord/mods/612) **v5.x** (Optional but recommended)

### Installation Steps

1. **Download** the mod archive
2. **Extract** the `KingdomCapitals` folder to:
   ```
   [Game Directory]\Modules\
   ```
   Example: `D:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\KingdomCapitals\`

3. **Enable** the mod in the Bannerlord launcher:
   - Launch the game
   - Go to **Mods** menu
   - Check **Kingdom Capitals**
   - Ensure load order: `Native` → `SandBoxCore` → `Sandbox` → `Kingdom Capitals`

4. **Start/Load** your campaign

## Configuration (MCM)

If MCM is installed, access mod settings via **Options → Mod Options → Kingdom Capitals**:

### Garrison Settings
- **Daily Garrison Reinforcement** (0-10): Number of troops added daily [Default: 3]
- **Garrison Food Consumption Multiplier** (0.1-2.0): Food consumption rate [Default: 0.5]
- **Prosperity Per Troop Tier** (1000-5000): Prosperity required per tier [Default: 2500]

### Conquest Settings
- **Enable Capital Conquest Mechanics**: Toggle total conquest feature [Default: ON]
- **Transfer Capital to Ruling Clan**: Bypass voting for capitals [Default: ON]
- **Vassalize Defeated Kingdom Clans**: Force vassalization vs independence [Default: ON]

### UI Settings
- **Show Golden Capital Names**: Display capitals with golden background [Default: ON]
- **Enable Capital Conquest Notifications**: Show conquest messages [Default: ON]

### Advanced Settings
- **Enable Debug Logging**: Write detailed logs to file [Default: OFF]
- **Allow Capital Level 4-5 Buildings**: Enable/disable building extensions [Default: ON] ⚠️ Requires restart

## Building from Source

### Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.7.2 SDK
- Mount & Blade II: Bannerlord installed

### Build Instructions

1. **Clone** the repository:
   ```bash
   git clone https://github.com/yourusername/KingdomCapitals.git
   cd KingdomCapitals
   ```

2. **Configure** game path in `KingdomCapitals.csproj`:
   ```xml
   <GameFolder>D:\SteamLibrary\steamapps\common\Mount &amp; Blade II Bannerlord</GameFolder>
   ```

3. **Build** the project:
   - **Debug**: Outputs to `bin\Win64_Shipping_Client\`
   - **Release**: Outputs directly to game's Modules folder

4. **Build Commands**:
   ```bash
   # Debug build
   msbuild KingdomCapitals.csproj /p:Configuration=Debug /p:Platform=x64

   # Release build (auto-installs to game)
   msbuild KingdomCapitals.csproj /p:Configuration=Release /p:Platform=x64
   ```

## Compatibility

### ✅ Compatible
- Clean vanilla Bannerlord v1.2.12
- Most gameplay mods that don't modify capital mechanics
- Diplomacy mods (may require load order adjustment)

### ⚠️ Potential Conflicts
- Mods that change kingdom destruction mechanics
- Mods that modify settlement ownership transfer
- Mods that add custom capital systems

### 📋 Load Order
```
Native
SandBoxCore
Sandbox
StoryMode (optional)
Bannerlord.Harmony
Bannerlord.MBOptionScreen (MCM)
[Other gameplay mods]
KingdomCapitals
```

## Known Issues

1. **UI Tooltip Color**: The golden background may not appear on all UI elements due to Bannerlord's complex UI system. The Harmony patch targets common tooltip locations but may need refinement.

2. **Player Kingdom Conquest**: If the player captures a capital before founding their own kingdom, they will receive a notification to found a kingdom. The conquest mechanics will activate after kingdom creation.

3. **Mid-Game Installation**: Installing on existing saves is supported, but capitals may not have extended buildings until new game or manual console commands.

## Troubleshooting

### Mod doesn't load
- Verify load order in launcher
- Check `[Game]\logs\` for error messages
- Ensure Harmony 2.3.6 is installed

### Capital conquest not working
- Check MCM settings (if installed)
- Verify `EnableCapitalConquest` is enabled
- Review logs at `C:\ProgramData\Mount and Blade II Bannerlord\logs\KingdomCapitals.log`

### Buildings don't show level 4-5
- Ensure `settlements.xml` is in `ModuleData\` folder
- Verify mod load order is correct
- Check MCM setting "Allow Capital Level 4-5 Buildings"

## Credits

- **TaleWorlds Entertainment** - Mount & Blade II: Bannerlord
- **Harmony Team** - Harmony patching library
- **MCM Team** - Mod Configuration Menu

## License

This mod is provided as-is for personal use. Modification and redistribution are allowed with proper attribution.

## Changelog

### v1.0.0 (2025-01-XX)
- Initial release
- Capital city designation system
- Building extensions to level 5
- Automatic capital transfer on ruler death
- Daily garrison reinforcement with prosperity-based tiers
- Total conquest mechanics on capital capture
- Golden capital name highlighting
- MCM integration for full customization
- Harmony patches for voting prevention and UI modification

## Support

For bug reports, suggestions, or questions:
- **GitHub Issues**: [Create an issue](https://github.com/yourusername/KingdomCapitals/issues)
- **Nexus Mods**: [Mod page](https://www.nexusmods.com/mountandblade2bannerlord/mods/XXXXX)

---

**Enjoy conquering Calradia through strategic capital warfare! 🏰⚔️**
