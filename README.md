# SPT Mods

Monorepo with source trees, release archives, and an installable mod pack for SPT **4.0.13**.

Target: **SPT 4.0.13** only. Do not treat 4.1.0+ as the baseline.

## Layout

```
SPT mods/
├── README.md
├── README.uk.md
├── mods/          # source per mod
├── mods_files/    # latest GitHub release zips per mod (+ DESCRIPTION.md EN/UK)
└── mods_patch/    # ModPack zip + install/update manager scripts
```

## Automatic install / update

Preferred way to install or refresh the full loadout on a clean **SPT 4.0.13** install.

1. Download the **Mods Pack Installer** release asset: [`mods_pack_installer`](https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer) (`SPT-4.0.13-ModsPack-Installer.zip`).
2. Unpack **both** `manage-modpack.cmd` and `manage-modpack.ps1` into the SPT root (same folder as `EscapeFromTarkov.exe` / `SPT.Server.exe`).
3. Run `manage-modpack.cmd` and pick a menu item:
   - **1** — Auto-update: remove current mods, download the latest ModPack from GitHub, install
   - **2** — Clean mods only (`BepInEx/plugins`, `BepInEx/patchers`, `user/mods`, pack extras)
   - **3** — Install mods (local zip next to the script, or download if missing)
   - **4** — Install SVM from the official [GhostFenixx/svm-csharp](https://github.com/GhostFenixx/svm-csharp) release (not bundled; PUSL forbids redistribution)
   - **5** — Exit

Internet is required for options **1** / **4**, and for **3** when `SPT-4.0.13-ModPack.zip` is not beside the scripts. The ModPack itself is published on [GitHub Releases](https://github.com/gadjed/SPT-4.0.13-Mods/releases).

macOS / Linux: use `mods_patch/manage-modpack.sh` the same way (`./manage-modpack.sh "/path/to/SPT"`).

## Mods

### Own / maintained here

| Folder | Notes | Source |
|--------|--------|--------|
| `FastSurgery` | Faster surgery / splint use times | [gadjed/FastSurgery-SPT-mod](https://github.com/gadjed/FastSurgery-SPT-mod) |
| `FastTaxi` | Shorter car/taxi extract wait | [gadjed/FastTaxi-SPT-mod](https://github.com/gadjed/FastTaxi-SPT-mod) |
| `InsuranceControl` | Insurance refund control | [gadjed/Insurance-refund-SPT-mod](https://github.com/gadjed/Insurance-refund-SPT-mod) |
| `QuickSearch` | Faster container search (client) | [gadjed/Quick-search-SPT-mod](https://github.com/gadjed/Quick-search-SPT-mod) |
| `Saria-4.x.x` | Saria Trader 2.0 | [gadjed/SariaTrader2.0-SPT-mod](https://github.com/gadjed/SariaTrader2.0-SPT-mod) |
| `SPTQuestingBots` | QuestingBots Continuous (includes former Scav Population; replaces DanW) | [gadjed/QuestingBots-Continuous-SPT-mod](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod) |
| `YellowFlareCurse` | Yellow flare curse | [gadjed/Yellow-flare-curse-SPT-mod](https://github.com/gadjed/Yellow-flare-curse-SPT-mod) |

### Third-party (server loadout + deps)

| Folder | Mod | Source |
|--------|-----|--------|
| `EnableLabyrinth` | Enable Labyrinth | [acidphantasm/enablelabyrinth-csharp](https://github.com/acidphantasm/enablelabyrinth-csharp) |
| `GildedKeyStorage` | Gilded Key Storage 2.0.4 (C#) | [DrakiaXYZ/SPT-GildedKeyStorage-CSharp](https://github.com/DrakiaXYZ/SPT-GildedKeyStorage-CSharp) |
| `LiveFleaPrices` | Live Flea Prices 2.0.1 (C#) | [DrakiaXYZ/SPT-LiveFleaPrices-CSharp](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices-CSharp) |
| `Fika-Server` | Fika server | [project-fika/Fika-Server-CSharp](https://github.com/project-fika/Fika-Server-CSharp) |
| `Fika-Plugin` | Fika client | [project-fika/Fika-Plugin](https://github.com/project-fika/Fika-Plugin) |
| `MoreBotsAPI` | MoreBotsAPI | [TacticalToaster/MoreBotsAPI](https://github.com/TacticalToaster/MoreBotsAPI) |
| `MoreCheckmarks` | MoreCheckmarks (+ backend) | [TommySoucy/MoreCheckmarks](https://github.com/TommySoucy/MoreCheckmarks) |
| `DynamicMaps` | Dynamic Maps | [acidphantasm/SPT-DynamicMaps](https://github.com/acidphantasm/SPT-DynamicMaps) |
| `LootingBots` | Looting Bots | [Skwizzy/SPT-LootingBots](https://github.com/Skwizzy/SPT-LootingBots) |
| `SAIN` | SAIN | [ArchangelWTF/SAIN](https://github.com/ArchangelWTF/SAIN) |
| `UIFixes` | UI Fixes | [tyfon7/UIFixes](https://github.com/tyfon7/UIFixes) |
| `UnbreakableKeys` | Unbreakable Keys | [Toha3673/unbreakableKeys](https://github.com/Toha3673/unbreakableKeys) |
| `SPT-BigBrain` | BigBrain (SAIN / bot deps) | [DrakiaXYZ/SPT-BigBrain](https://github.com/DrakiaXYZ/SPT-BigBrain) |
| `ContinuousHealing` | Continuous Healing | [Lacyway/ContinuousHealing](https://github.com/Lacyway/ContinuousHealing) |

## Loadout notes

- Use **`SPTQuestingBots`** (Continuous); DanW QuestingBots and standalone **Scav Population** are not included (population features live in Continuous)
- Includes **`YellowFlareCurse`** and **`Saria-4.x.x`** 2.0.1
- **SVM is not redistributed** (upstream PUSL). Optional install via manage-modpack menu item **4** from the official GhostFenixx release
- Install pack: `mods_patch/SPT-4.0.13-ModPack.zip` (GitHub Releases) — or use the installer above
- Manager scripts: `mods_patch/manage-modpack.cmd` + `.ps1` (+ `.sh`)

## Notes

- Prefer the installer / ModPack release for installs; use this tree for reading and patching source.
- Original upstream URLs are listed above (nested per-mod git history is not kept in the monorepo).
