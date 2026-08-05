# Pack Items

**SPT 4.0.13 Compatible**

Client mod that adds **Скласти предмети** to the right-click context menu on stash containers (grenade box, junk box, meds case, etc.). Compatible loose items from the stash grid are moved into the selected container until the first item that no longer fits.

Developed and tested against **SPT 4.0.13**.

## Features

- Context-menu action on containers in stash / trader player inventory
- Uses each container’s grid filters (grenades → grenade box, loot → junk, meds → meds case, …)
- Only moves **top-level** stash items (not contents of other cases)
- Stops at the first compatible item that does not fit (no rearrange / optimize pass)
- Skips pinned and locked items
- Not shown in raid
- F12 Configuration Manager support
- No server mod required

## Install

1. Build or download `PackItems.dll`
2. Place into `BepInEx/plugins/PackItems.dll`
3. Launch the game

On load the BepInEx log should show:

```text
[Info   :Pack Items] Pack Items v1.0.0 loaded (SPT 4.0.13).
```

## Config

Edit in-game via **F12**, or `BepInEx/config/gadjed.packitems.cfg`:

| Key | Default | Description |
|-----|---------|-------------|
| `Enabled` | `true` | Show the menu action |
| `MenuLabel` | `Скласти предмети` | Context menu text |
| `Debug` | `false` | Extra logging |

## Build from source

Requires **.NET SDK** and the DLLs listed in `References/README.md`.

```bash
dotnet build PackItems.csproj -c Release
```

Output: `Build/BepInEx/plugins/PackItems.dll`

## License

MIT — see [LICENSE](LICENSE).
