# All Ships All Slots

A lightweight, configurable Sunless Sea mod that adds Forward and AFT slots to each ship in an elegant, configurable (and legal) manner.

## Overview

This mod adds both Forward and AFT weapon slots to every ship in Sunless Sea, while adding configuration through a config file. It was born out of necessity, since the original "Aft Slots for ALL" mod is inferior to this mod in several ways.

1. At just 10KB (compared to the original's 70MB) this mod is 7000 times smaller,  incredibly lightweight.
2. The original mod only added AFT slots and couldn't be configured, while this version gives you both Forward and AFT slots with easy customization options.
3. The original mod edits Sunless.Game.dll, meaning there are major incompatibilities between updates (Pre/Post 2023 fix) and versions (Steam, Epic, GOG).
4. The original mod only affected vanilla ships, while this mod includes modded ships.
5. The original mod redistributes Sunless.Game.dll, which is proprietary code, and thus illegal.

## Known limitations

- Ships will not advertise their new slots on the shipyard screen
- Ships will have their Forward and AFT text greyed out on the Hold menu
- Your current ship will not automatically have the new slots. You will have to re-purchase your ship (or trade it in using Ship Storage).

Fixing these issues would require **extremely invasive** patching. These will not be fixed.

## Configuration

Edit `AllShipsAllSlots_config.ini` in your Sunless Sea **install** folder to customize:

- `Unlock Forward = true/false` - Enable/disable Forward slot
- `Unlock AFT = true/false` - Enable/disable AFT slot
