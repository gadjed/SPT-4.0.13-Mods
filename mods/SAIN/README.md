# SAIN StealthEngage

**SPT 4.1.0** fork of [ArchangelWTF/SAIN](https://github.com/ArchangelWTF/SAIN) (Solarint's AI Modifications).

When a PMC is at peace and hears **nearby gunfire**, it starts a **careful search** (no sprint / no charge) and only fully engages once it has the advantage of still being unseen/unheard.

[Latest release](https://github.com/gadjed/SAIN-StealthEngage-SPT-mod/releases/latest) · [License](LICENSE) · [Attribution](NOTICE)

## Requires

- [BigBrain](https://forge.sp-tarkov.com/mod/902/bigbrain) ≥ 1.4.0
- [Waypoints](https://forge.sp-tarkov.com/mod/827/waypoints-expanded-navmesh) ≥ 1.8.2

**Recommended:** [Questing Bots Continuous](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod), [Looting Bots](https://forge.sp-tarkov.com/mod/812/looting-bots)

## Do not use with

- Stock **SAIN** (Solarint / ArchangelWTF) — **replace** it with this fork (same GUID `me.sol.sain`)

## Install

1. Remove existing `BepInEx/plugins/SAIN/` and `SPT/user/mods/Solarint-SAIN-ServerMod/` (or `user/mods/Solarint-SAIN-ServerMod/`)
2. Download `SAIN-StealthEngage-*.zip` from [Releases](https://github.com/gadjed/SAIN-StealthEngage-SPT-mod/releases)
3. Extract into your **SPT game root**
4. Restart SPT server and client

```text
BepInEx/plugins/SAIN/SAIN.dll
SPT/user/mods/Solarint-SAIN-ServerMod/SAINServerMod.dll
SPT/user/mods/Solarint-SAIN-ServerMod/Data/NicknamePersonalities.json
```

## What this fork changes

| Behavior | Detail |
|----------|--------|
| Heard-from-peace (PMC) | `Freeze` / `SearchNow` → **`StealthEngage`** |
| Approach | No sprint, slower move, lights off, until enemy is seen |
| Engage distance | **Map-relative** default: 70% of longer map axis (Customs ≈ 940 m → ~660 m), clamped 120–700 m |
| Wreckless `Charge` | Unchanged |

F6 → **Mind → Stealth Engage**:

- Use Map-Relative Distance
- Map Size Fraction (default `0.7`)
- Absolute Min / Max Distance

After install, open F6 once and **Save / Export** presets (preset version `4.4.4`).

## Build

```bash
dotnet build SAIN/SAIN.csproj -c Release
dotnet build SAINServerMod/SAINServerMod.csproj -c Release
./scripts/package-release.sh
```

Output: `Dist/SAIN-StealthEngage-4.4.4.zip` (Forge SPT 4.x roots: `BepInEx/` + `SPT/`).

## Credits

- **Solarint** — original SAIN
- **ArchangelWTF** — upstream maintenance fork
- **DrakiaXYZ** — BigBrain / Waypoints
- Fork changes (StealthEngage) — gadjed

See [NOTICE](NOTICE).
