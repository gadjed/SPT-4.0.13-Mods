# Insurance Control

**SPT 4.0.13 Compatible**

Combined **server + client** mod: insurance return / content rules on the server, plus an **Insure All** stash button on the client (merged from Insure All Prapor).

Developed and tested against **SPT 4.0.13**.

[Latest release](https://github.com/gadjed/Insurance-refund-SPT-mod/releases/latest) · [License: MIT](LICENSE)

## Features

### Server

- Configurable insurance return delay (fixed seconds or per-trader hours)
- Debug fast return via `DebugReturnSeconds`
- Configurable lost-item chance per trader (Prapor / Therapist)
- Magazines returned with loaded ammo; backpacks / rigs with grid contents
- Pre-raid inventory snapshot (LootingBots-safe)
- Shorter insurance processing interval

### Client

- Stash button above the tactical vest — insure equipped loadout in one click
- Trader selectable in F12 (Prapor or Therapist)
- No confirmation dialog; skips already-insured / non-insurable items
- Button label, size, and position configurable in F12

## Install

1. Download `InsuranceControl-*.zip` from [Releases](https://github.com/gadjed/Insurance-refund-SPT-mod/releases)
2. Extract into your **SPT game root**
3. Restart the SPT server and the game client

```text
SPT/user/mods/InsuranceControl/InsuranceControl.dll
SPT/user/mods/InsuranceControl/config.json
BepInEx/plugins/InsuranceControl.Client.dll
```

Remove older standalone installs if present:

- `user/mods/InsuranceControl/` from **Insurance Refund** alone is fine to overwrite
- Delete `BepInEx/plugins/InsureAllPrapor.dll` (replaced by this client plugin)

## Client config (F12)

`BepInEx/config/gadjed.insurancerefund.cfg` — or **F12** Configuration Manager:

| Section | Key | Default | Description |
|---------|-----|---------|-------------|
| 1. Insure All | Enabled | `true` | Show the button |
| 1. Insure All | Button Label | `Застраховать все` | Button text |
| 1. Insure All | Insurer | `Prapor` | `Prapor` or `Therapist` |
| 2. Button Layout | Offset Right | `190` | Horizontal offset from vest/armor |
| 2. Button Layout | Gap Above Anchor | `12` | Vertical gap above the slot |
| 2. Button Layout | Button Width / Height | `140` / `24` | Size |
| 2. Button Layout | Font Size | `14` | Label font size |
| 3. Debug | Verbose Logging | `false` | Extra BepInEx logs |

Layout changes apply while the stash is open (no restart needed).

## Server config

Edit `SPT/user/mods/InsuranceControl/config.json` (server settings are not in F12):

```json
{
  "DebugReturnSeconds": 0,
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
| `DebugReturnSeconds` | If `> 0`, return after N seconds and shorten the poll. `0` = normal. |
| `ReturnTimeOverrideSeconds` | Fixed delay in seconds. `0` → use `TraderReturnHours`. |
| `RunIntervalSeconds` | How often the server checks ready returns. Vanilla `600`. |
| `StorageTimeOverrideSeconds` | Mail retention (`0` = trader default). |
| `ReturnMagazinesWithAmmo` / `ReturnContainersWithContents` | Content enrichment |
| `SimulateItemsBeingTaken` | SPT scavenger / attachment loot simulation |
| `LostChancePercent` | Permanent loss % (`0` = always returned) |
| `TraderReturnHours` | Hour window when override/debug seconds are `0` |

## Notes

- Insure All covers **equipped PMC gear** only (not stash grids)
- Needs enough rubles in stash
- GUID `gadjed.insurancerefund` (client + server). Remove old `InsureAllPrapor.dll` (`gadjed.insureallprapor`)

## Build from source

```bash
dotnet build Server/InsuranceControl.csproj -c Release
dotnet build Client/InsuranceControl.Client.csproj -c Release
```

Output under `Build/SPT/user/mods/InsuranceControl/` and `Build/BepInEx/plugins/`.

## License

MIT — see [LICENSE](LICENSE).
