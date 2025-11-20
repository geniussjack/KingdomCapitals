# Crown Icon Sprites

This folder contains the crown icon textures for capital settlement nameplates.

## Required Files

You need to create two PNG image files:

### 1. `capital_crown_icon.png`
- **Size**: 20x20 pixels
- **Format**: PNG with transparency (RGBA)
- **Content**: Gold/yellow crown icon
- **Background**: Transparent

### 2. `capital_crown_icon@2x.png`
- **Size**: 40x40 pixels
- **Format**: PNG with transparency (RGBA)
- **Content**: Same crown icon, but 2x larger for high-DPI displays
- **Background**: Transparent

## Design Recommendations

### Color Scheme
- **Primary color**: Gold (#FFD700 or similar)
- **Shadow/outline**: Dark brown/black for contrast (#3D2817)
- **Style**: Match Bannerlord's medieval aesthetic

### Icon Style
- Simple, recognizable crown silhouette
- Clear even at 20x20 size
- Good contrast for visibility on map
- Consider a subtle glow/shadow for depth

## Alternative: Using Game's Built-in Crown

Instead of creating custom sprites, you can use Bannerlord's built-in crown icon:
- **Sprite path**: `SPScoreboard\leader_crown_icon`
- This is already available in the game files
- Located in: `Modules\SandBox\GUI\...`

If you want to use the built-in sprite, you don't need to create these PNG files. Just update the patch code to reference `SPScoreboard\leader_crown_icon` instead.

## How to Create the Icons

### Option 1: Use Photo Editor
1. Open your favorite image editor (Photoshop, GIMP, Paint.NET, etc.)
2. Create new image: 20x20 px with transparent background
3. Draw a simple crown shape in gold color
4. Add a dark outline for contrast
5. Save as PNG with transparency
6. Repeat for 40x40 px version

### Option 2: Use Online Tools
- [Flaticon](https://www.flaticon.com/) - Free crown icons (check license)
- [Noun Project](https://thenounproject.com/) - Crown symbols
- [Pixlr](https://pixlr.com/) - Online image editor

### Option 3: Extract from Game Files
Bannerlord's own crown icon can be found in game files. You can extract and modify it if needed.

## Installation

After creating the PNG files:
1. Place them in this folder (`GUI/SpriteParts/ui_kingdomcapitals/`)
2. Rebuild the project (Debug or Release)
3. The build process will automatically copy GUI folder to the mod directory
4. Launch the game and check settlement nameplates

## Troubleshooting

**Icons not showing?**
- Check that PNG files are exactly 20x20 and 40x40 pixels
- Verify files are in correct folder
- Check SpriteData.xml for correct sprite names
- Review game logs for sprite loading errors

**Icons look bad?**
- Increase contrast (darker outline)
- Try simpler design
- Use game's built-in sprite instead
