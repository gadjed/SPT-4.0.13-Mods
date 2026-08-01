# Insurance Refund

**SPT 4.0.13 Compatible**

Server mod that controls insurance return time, lost-item chance, and whether magazines / backpacks / rigs come back with their contents.

Developed and tested against **SPT 4.0.13**.

[Latest release](https://github.com/gadjed/Insurance-refund-SPT-mod/releases/latest) · [License: MIT](LICENSE)

## Features

- Configurable insurance return delay (fixed seconds or per-trader hours)
- Configurable lost-item chance per trader (Prapor / Therapist)
- Magazines returned with loaded ammo
- Backpacks and chest rigs returned with grid contents
- Shorter insurance processing interval so short delays feel accurate
- No client-side plugin required

## Install

1. Download `InsuranceControl-*.zip` from [Releases](https://github.com/gadjed/Insurance-refund-SPT-mod/releases)
2. Extract the archive into your **SPT game root** (the folder that contains `SPT.Server.exe` / `user/`)
3. Restart the SPT server

The zip already contains the correct paths:

```text
user/mods/InsuranceControl/InsuranceControl.dll
user/mods/InsuranceControl/config.json
```

On startup the server log should show lines like:

```text
[InsuranceControl] Prapor: lost chance 0% (return chance 100%)
[InsuranceControl] Content enrichment patch enabled.
[InsuranceControl] Loaded. ReturnTimeOverride=3600s, MagsWithAmmo=True, ContainersWithContents=True.
```

## Config

Edit `user/mods/InsuranceControl/config.json`:

```json
{
  "ReturnTimeOverrideSeconds": 3600,
  "RunIntervalSeconds": 60,
  "StorageTimeOverrideSeconds": 0,
  "ReturnMagazinesWithAmmo": true,
  "ReturnContainersWithContents": true,
  "SimulateItemsBeingTaken": true,
  "LostChancePercent": {
    "Prapor": 0,
    "Therapist": 0
  },
  "TraderReturnHours": {
    "Prapor": { "Min": 1, "Max": 2 },
    "Therapist": { "Min": 1, "Max": 1 }
  }
}
```

| Key | Description |
|-----|-------------|
| `ReturnTimeOverrideSeconds` | Fixed delay before insurance mail (seconds). `3600` = 1 hour. Set `0` to use `TraderReturnHours` instead. |
| `RunIntervalSeconds` | How often the server checks for ready returns. Vanilla is `600`. Use something ≤ your return delay. |
| `StorageTimeOverrideSeconds` | How long returned items stay in mail (`0` = trader default). |
| `ReturnMagazinesWithAmmo` | Keep cartridges inside returned magazines (and chambered rounds on weapons). |
| `ReturnContainersWithContents` | Keep items inside returned backpacks and chest rigs. |
| `SimulateItemsBeingTaken` | Allow SPT's scavenger / attachment loot simulation. |
| `LostChancePercent` | Chance an insured item is permanently lost (`0` = always returned). Vanilla ≈ Prapor `15`, Therapist `5`. |
| `TraderReturnHours` | Min/max return window in hours. Used only when `ReturnTimeOverrideSeconds` is `0`. |

Trader keys accept names (`Prapor`, `Therapist`) or their Mongo IDs.

## Notes

- Content enrichment runs when insurance packages are created after a raid. Ammo / container items still need to exist on the lost item tree or in inventory at that moment (normal on death; dropped kits depend on what the client sends).
- Compatible with most insurance price mods. Avoid stacking with other mods that also rewrite `HandleInsuredItemLostEvent` / insurance return chance unless you know they cooperate.

## Build from source

Requires **.NET 9** SDK.

```bash
dotnet build InsuranceControl.csproj -c Release
```

Output is copied to `Build/SPT/user/mods/InsuranceControl/`.

## License

MIT — see [LICENSE](LICENSE).
