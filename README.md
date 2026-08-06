# SPT Mods

Monorepo with source trees, release archives, and an installable mod pack for SPT **4.0.13**.

Target: **SPT 4.0.13**.

## Layout

```
SPT mods/
├── README.md
├── README.uk.md
├── docs/
├── mods/
│   ├── owned/      # original mods maintained here
│   ├── forks/      # maintained forks of upstream mods
│   └── external/   # third-party sources bundled in ModPack
├── mods_files/     # latest release zips (+ DESCRIPTION.md EN/UK), same grouping
└── mods_patch/     # ModPack zip + install/update manager scripts
```

## Automatic install / update

Preferred way to install or refresh the full loadout on a clean **SPT 4.0.13** install.

1. Download the **Mods Pack Installer** release asset: [`mods_pack_installer_4.0.13`](https://github.com/gadjed/SPT-4.0.13-Mods/releases/tag/mods_pack_installer_4.0.13) (`SPT-4.0.13-ModsPack-Installer.zip`).
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

### Owned (`mods/owned/`)

| Folder | Notes | Source |
|--------|--------|--------|
| `MedRebalance` | Medicine rebalance — fast surgery, continuous limbs, scratch heal, cancel on damage | [gadjed/MedRebalance-SPT-mod](https://github.com/gadjed/MedRebalance-SPT-mod) |
| `AutoMedHotkeys` | Auto-bind meds to slots 4/5/6 (stash + raid) | [gadjed/Auto-med-hotkeys-SPT-mod](https://github.com/gadjed/Auto-med-hotkeys-SPT-mod) |
| `FastTaxi` | Shorter car/taxi extract wait | [gadjed/FastTaxi-SPT-mod](https://github.com/gadjed/FastTaxi-SPT-mod) |
| `InsuranceControl` | Insurance return rules (server) + Insure All stash button (client, F12) | [gadjed/Insurance-refund-SPT-mod](https://github.com/gadjed/Insurance-refund-SPT-mod) |
| `ModInventory` | Host mod inventory API for client delta-sync | [gadjed/ModInventory-SPT-mod](https://github.com/gadjed/ModInventory-SPT-mod) |
| `QuickSearch` | Faster container search (client) | [gadjed/Quick-search-SPT-mod](https://github.com/gadjed/Quick-search-SPT-mod) |
| `YellowFlareCurse` | Yellow flare curse | [gadjed/Yellow-flare-curse-SPT-mod](https://github.com/gadjed/Yellow-flare-curse-SPT-mod) |
| `PackItems` | Stash context menu — pack matching loose items into a case | [gadjed/Pack-items-SPT-mod](https://github.com/gadjed/Pack-items-SPT-mod) |
| `DefibAllyRevive` | Revive downed allies with defibrillator on quick slots | [gadjed/Defib-ally-revive-SPT-mod](https://github.com/gadjed/Defib-ally-revive-SPT-mod) |

### Forks (`mods/forks/`)

| Folder | Notes | Source |
|--------|--------|--------|
| `SAIN` | SAIN StealthEngage (PMC careful engage on nearby gunfire) | [gadjed/SAIN-StealthEngage-SPT-mod](https://github.com/gadjed/SAIN-StealthEngage-SPT-mod) |
| `SPTQuestingBots` | QuestingBots Continuous (includes former Scav Population; replaces DanW) | [gadjed/QuestingBots-Continuous-SPT-mod](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod) |
| `Saria-4.x.x` | Saria Trader 2.0 | [gadjed/SariaTrader2.0-SPT-mod](https://github.com/gadjed/SariaTrader2.0-SPT-mod) |
| `SPT-Waypoints` | Waypoints — local fork base (DrakiaXYZ 1.8.2) | [DrakiaXYZ/SPT-Waypoints](https://github.com/DrakiaXYZ/SPT-Waypoints) |

### External (`mods/external/`)

| Folder | Mod | Source |
|--------|-----|--------|
| `AmandsGraphics` | Amands's Graphics (client; 1.7.0 / SPT 4.0) | [Amands2Mello/AmandsGraphics](https://github.com/Amands2Mello/AmandsGraphics) · [Forge](https://forge.sp-tarkov.com/mod/592/amandss-graphics) |
| `FastSellInFlea` | Fast Sell In Flea (client; 1.2.0) | [Katrin0522/SPT-FastInFleaSell](https://github.com/Katrin0522/SPT-FastInFleaSell) |
| `EnableLabyrinth` | Enable Labyrinth | [acidphantasm/enablelabyrinth-csharp](https://github.com/acidphantasm/enablelabyrinth-csharp) |
| `Skipper` | Skipper — skip quests (client; 1.1.4) | [acidphantasm/SPT-Skipper](https://github.com/acidphantasm/SPT-Skipper) · [Forge](https://forge.sp-tarkov.com/mod/1343/skipper) |
| `GildedKeyStorage` | Gilded Key Storage | [DrakiaXYZ/SPT-GildedKeyStorage-CSharp](https://github.com/DrakiaXYZ/SPT-GildedKeyStorage-CSharp) |
| `LiveFleaPrices` | Live Flea Prices | [DrakiaXYZ/SPT-LiveFleaPrices-CSharp](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices-CSharp) |
| `Fika-Server` | Fika server | [project-fika/Fika-Server-CSharp](https://github.com/project-fika/Fika-Server-CSharp) |
| `Fika-Plugin` | Fika client | [project-fika/Fika-Plugin](https://github.com/project-fika/Fika-Plugin) |
| `MoreBotsAPI` | MoreBotsAPI | [TacticalToaster/MoreBotsAPI](https://github.com/TacticalToaster/MoreBotsAPI) |
| `MoreCheckmarks` | MoreCheckmarks (+ backend) | [TommySoucy/MoreCheckmarks](https://github.com/TommySoucy/MoreCheckmarks) |
| `DynamicMaps` | Dynamic Maps | [acidphantasm/SPT-DynamicMaps](https://github.com/acidphantasm/SPT-DynamicMaps) |
| `LootingBots` | Looting Bots | [Skwizzy/SPT-LootingBots](https://github.com/Skwizzy/SPT-LootingBots) |
| `UIFixes` | UI Fixes | [tyfon7/UIFixes](https://github.com/tyfon7/UIFixes) |
| `UnbreakableKeys` | Unbreakable Keys | [Toha3673/unbreakableKeys](https://github.com/Toha3673/unbreakableKeys) |
| `SPT-BigBrain` | BigBrain (SAIN / bot deps) | [DrakiaXYZ/SPT-BigBrain](https://github.com/DrakiaXYZ/SPT-BigBrain) |

## Loadout notes

- **ModPack 4.0.13** (`v1.2.16+`) ships the full server+client loadout (see MANIFEST in the zip), including **Saria 2.0.5** (BlackRock, SPEAR 6.8 ammo/mags), **PackItems**, **DefibAllyRevive 1.0.1**, **InsuranceControl 1.1.0** (return rules + Insure All), **ModInventory**, **Skipper** (quest skip), and **AmandsGraphics 1.7.0** (brightness / post FX)
- Use **`SPTQuestingBots`** (Continuous); DanW QuestingBots and standalone **Scav Population** are not included
- Prefer **`SAIN` StealthEngage** over stock SAIN (same install paths / GUID); manage-modpack strips alternate SAIN folder names
- **SVM is not redistributed** (upstream PUSL). Optional install via manage-modpack menu item **4**
- Install pack: `mods_patch/SPT-4.0.13-ModPack.zip` (GitHub Releases) — or use the installer above
- Manager scripts: `mods_patch/manage-modpack.cmd` + `.ps1` (+ `.sh`)
- Nested SPT layout: server mods must live in `SPT/user/mods/` (installer syncs / packs ship that path). Do not leave DanW `QuestingBots` beside Continuous.
- Do **not** use broken ModPack `v1.2.5`

## Notes

- Prefer the installer / ModPack release for installs; use this tree for reading and patching source.
- Original upstream URLs are listed above (nested per-mod git history is not kept in the monorepo).
- Bot AI stack design notes (UK): [`docs/bot-ai-stack-notes.uk.md`](docs/bot-ai-stack-notes.uk.md)
