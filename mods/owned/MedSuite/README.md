# Med Suite

Unified medical toolkit for **SPT 4.0.13** — merges Auto Med Hotkeys, Defib Ally Revive, and Med Rebalance into one client+server mod with **F12** (ConfigurationManager) controls.

**GUID:** `gadjed.medsuite` · **Version:** 1.0.0 · **Author:** gadjed

## Features

| Module | What it does |
|--------|----------------|
| **Auto Med Hotkeys** | Auto-binds medkits / bleed-stoppers / bandages to quick slots **4 / 5 / 6** |
| **Defib Ally Revive** | Downs GroupId allies instead of killing them; revive with a Portable defibrillator on a quick slot |
| **Continuous Healing** | Multi-limb heal continue, scratch top-ups, cancel-on-damage |
| **Server rebalance** | Shortens surgery/splint `MedUseTime`; sets defibrillator resource to 1/1 for UI |

## Install

Extract into the SPT **game root** (no extra wrapper folder):

```text
BepInEx/plugins/MedSuite.Client.dll
SPT/user/mods/MedSuite/MedSuite.dll
SPT/user/mods/MedSuite/config.json
```

Remove the old standalone mods if present:

- `AutoMedHotkeys`
- `DefibAllyRevive` (client + server)
- `MedRebalance` / `MedRebalance.Client`

Incompatible with Lacyway Continuous Healing (`com.lacyway.ch`).

## Config (F12)

Press **F12** (ConfigurationManager) → **Med Suite**. Each module has an **Enabled** toggle.

### 1. Auto Med Hotkeys

| Key | Default | Description |
|-----|---------|-------------|
| Enabled | true | Bind meds to slots 4/5/6 |
| OverwriteExisting | true | Replace non-matching items on those slots |
| Debug | false | Verbose bind logs |

### 2. Defib Ally Revive

| Key | Default | Description |
|-----|---------|-------------|
| Enabled | true | Ally downed + defibrillator revive |
| ReviveRange | 3.5 | Max revive distance (m) |
| ReviveTime | 5 | Channel time (s) |
| BleedoutTime | 90 | Seconds until permanent death (`0` = no timer) |
| RequireSameGroup | true | Same `GroupId` only |
| AllowSameSide | false | Also same USEC/BEAR |
| ConsumeDefibrillator | true | Spend 1/1 charge and remove defib after success |
| FullHealOnRevive | true | Full heal vs minimal vital restore |
| Debug | false | Verbose logs |

### 3–5. Continuous Healing / Interrupt / Scratch Heal

| Key | Default | Description |
|-----|---------|-------------|
| Continuous Healing → Enabled | true | Continue across limbs |
| Heal Limbs | true | Also surgery/splints |
| Heal Delay | 0 | Seconds between limbs |
| Reset Animations | true | Fresh anim between limbs |
| Cancel On Damage | true | Abort heal + restore weapon |
| Scratch Heal → Enabled | true | Top up lightly damaged limbs |
| Heal Amount | 2.5 | HP per scratch limb |
| Max Missing HP | 8 | Scratch threshold |

Settings are also in `BepInEx/config/gadjed.medsuite.cfg`.

## Server config (`config.json`)

Requires a **server restart** (not F12):

```json
{
  "UseTimeSeconds": 5,
  "FixDefibrillatorResource": true,
  "Items": {
    "Immobilizing splint": true,
    "Aluminum splint": true,
    "CMS surgical kit": true,
    "Surv12 field surgical kit": true
  }
}
```

## Build

```bash
dotnet build Server/MedSuite.csproj -c Release
dotnet build Client/MedSuite.Client.csproj -c Release
bash scripts/package-release.sh
```

Forge-ready zip layout: `BepInEx/...` + `SPT/...` at archive root.

## License

MIT
