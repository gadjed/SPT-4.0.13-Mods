# Questing Bots Continuous

**SPT 4.1.0 Compatible**

Fork of [DanW's Questing Bots](https://github.com/dwesterwick/SPTQuestingBots) with **continuous scav population** merged from [Scav Population](https://github.com/gadjed/Scav-population-SPT-mod).

All original Questing Bots questing / AI objective logic is preserved. PMC squads default to **2–4 bots** more often than solos.

[Latest release](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod/releases/latest) · [License: CC BY-NC-SA 4.0](LICENSE) · [Attribution](NOTICE)

## Requires

- [BigBrain](https://forge.sp-tarkov.com/mod/902/bigbrain) ≥ 1.4.0
- [Waypoints](https://forge.sp-tarkov.com/mod/827/waypoints-expanded-navmesh) ≥ 1.8.2

**Recommended:** [SAIN](https://forge.sp-tarkov.com/mod/791/sain-solarints-ai-modifications-full-ai-combat-system-replacement), [Looting Bots](https://forge.sp-tarkov.com/mod/812/looting-bots)

## Do not use with

- Original **Questing Bots** (DanW) — replace it with this fork
- **Scav Population** — features are included here (stacking causes overpopulation)
- Phobos, ORBIT, AI Limit (same incompatibilities as upstream)

## Install

1. Remove `user/mods/QuestingBots*` / `DanW-QuestingBots*` and `BepInEx/plugins/QuestingBots*` if present
2. Remove Scav Population (server + client) if present
3. Download `QuestingBotsContinuous-*.zip` from [Releases](https://github.com/gadjed/QuestingBots-Continuous-SPT-mod/releases)
4. Extract into your **SPT game root** (folder with `SPT.Server.exe` / `user/`)
5. Restart SPT server and game client

```text
SPT/user/mods/QuestingBotsContinuous/QuestingBotsContinuous-Server.dll
SPT/user/mods/QuestingBotsContinuous/config.json
SPT/user/mods/QuestingBotsContinuous/eftQuestSettings.json
SPT/user/mods/QuestingBotsContinuous/zoneAndItemQuestPositions.json
BepInEx/plugins/QuestingBotsContinuous/QuestingBotsContinuous-Client.dll
BepInEx/plugins/QuestingBotsContinuous/Quests/...
```

## What this fork adds

| Feature | Where |
|--------|--------|
| Timed scav reinforcement waves across the full raid | Server |
| PMC group weights biased to 2–4 man squads | `config.json` |
| Squad size no longer shrunk mid-PMC-raid by ET factor | Client |

Config block:

```json
"continuous_population": {
  "enabled": true,
  "scav_reinforcements": {
    "enabled": true,
    "start_after_seconds": 180,
    "interval_seconds": 180,
    "slots_min": 2,
    "slots_max": 4,
    "waves_per_interval": 2,
    "difficulty": "normal",
    "extend_bot_stop": true,
    "skip_maps": [ "laboratory", "labyrinth", "hideout" ]
  }
}
```

PMC squad weights:

```json
"bots_per_group_distribution": [
  [ 1, 10 ],
  [ 2, 40 ],
  [ 3, 35 ],
  [ 4, 15 ]
]
```

Everything else (quest types, BigBrain layers, door unlocking, AI limiter, F12 scav limits, etc.) follows upstream Questing Bots — see the [upstream README](https://github.com/dwesterwick/SPTQuestingBots/blob/master/README.md) for the full reference.

## Credits

- **DanW** — original [Questing Bots](https://github.com/dwesterwick/SPTQuestingBots) (CC BY-NC-SA 4.0)
- **gadjed** — continuous population merge; scav wave approach from [Scav Population](https://github.com/gadjed/Scav-population-SPT-mod) (MIT)
- Upstream credits also apply: DrakiaXYZ (BigBrain / Waypoints), Solarint (SAIN), Skwizzy (Looting Bots), Props, nooky, ozen, SPT team

## License

Distributed under **CC BY-NC-SA 4.0** (ShareAlike from upstream Questing Bots). See [LICENSE](LICENSE) and [NOTICE](NOTICE).  
MIT text for Scav Population adaptations: [THIRD_PARTY_MIT_ScavPopulation.txt](THIRD_PARTY_MIT_ScavPopulation.txt).

## Build from source

Requires **.NET 10** SDK for the server (`net10.0`). Client build needs SPT/EFT hollowed references under `Client/References/` (see `Client/References/README.md`).

```bash
dotnet build Server/QuestingBots-Server.csproj -c Release -p:SkipQuestingBotsScripts=true
dotnet build Client/QuestingBots-Client.csproj -c Release -p:SkipQuestingBotsScripts=true
./scripts/package-release.sh
```
