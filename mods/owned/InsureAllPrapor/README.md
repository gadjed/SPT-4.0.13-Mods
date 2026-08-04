# Insure All (Prapor)

**SPT 4.0.13 Compatible**

Client mod that adds a **Застраховать все** button next to the helmet slot on the stash / inventory equipment panel. One click insures your equipped loadout with **Prapor** for rubles using the vanilla insurance API — no confirmation dialog.

Developed and tested against **SPT 4.0.13**.

[Latest release](https://github.com/gadjed/Insure-all-prapor-SPT-mod/releases/latest) · [License: MIT](LICENSE)

## Features

- Button on the stash equipment panel, near the helmet slot
- Insures all eligible equipped gear (weapons, armor, vest/backpack/pocket contents, attachments)
- Uses Prapor’s normal paid insurance flow (`InsuranceCompanyClass`)
- Skips the insure confirmation window
- Skips already-insured and non-insurable items (secure container rules, etc.)
- F12 Configuration Manager support
- No server mod required

## Install

1. Download `InsureAllPrapor-*.zip` from [Releases](https://github.com/gadjed/Insure-all-prapor-SPT-mod/releases)
2. Extract the archive into your **SPT game root** (the folder that contains `EscapeFromTarkov.exe` / `BepInEx/`)
3. Launch the game

The zip already contains the correct path:

```text
BepInEx/plugins/InsureAllPrapor.dll
```

On load the BepInEx log should show:

```text
[Info   :gadjed-InsureAllPrapor] gadjed-InsureAllPrapor v1.0.0 loaded (SPT 4.0.13).
```

## Config

Edit in-game via **F12**, or `BepInEx/config/gadjed.insureallprapor.cfg`:

| Key | Default | Description |
|-----|---------|-------------|
| `Enabled` | `true` | Show the button |
| `ButtonLabel` | `Застраховать все` | Button text |
| `Debug` | `false` | Extra logging |

## Notes

- Scope is **equipped PMC gear**, not the stash grids
- Requires enough rubles in stash; otherwise you get a warning and nothing is purchased
- Compatible with server insurance-return mods (e.g. Insurance Control) — this only purchases insurance

## Build from source

Requires **.NET SDK** and the DLLs listed in `References/README.md` (from your SPT 4.0.13 install / SAIN hollowed refs).

```bash
dotnet build InsureAllPrapor.csproj -c Release
```

Output: `Build/BepInEx/plugins/InsureAllPrapor.dll`

## License

MIT — see [LICENSE](LICENSE).
