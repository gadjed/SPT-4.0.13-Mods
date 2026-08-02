# ModInventory (SPT 4.0.13)

Server mod that exposes the host's installed mod inventory for the custom launcher.

## Endpoints

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/modinventory/api/manifest` | JSON list of mods + per-file `sha256` / size |
| GET | `/modinventory/api/file?path=` | Download one allowlisted file from the host game root |

Allowlisted roots (configurable in `config.json`):

- `BepInEx/plugins`
- `BepInEx/patchers`
- `SPT/user/mods`
- `user/mods`

Profiles, logs, and `.pdb` are never served.

## Install

```text
SPT/user/mods/ModInventory/
  ModInventory.dll
  config.json
```

Build: `dotnet build` (output also copied to `Build/SPT/user/mods/ModInventory/`).

## SPT

Targets **SPT 4.0.13** (`~4.0.13`).
