# ModInventory

**SPT 4.0.13 Compatible**

Server mod that exposes the host's installed mod inventory so clients can **delta-sync** only changed files (instead of downloading a full mod pack).

Developed and tested against **SPT 4.0.13**.

[Latest release](https://github.com/gadjed/ModInventory-SPT-mod/releases/latest) · [License: MIT](LICENSE)

## Features

- Live inventory of installed client/server mods on the host
- Per-file `sha256` + size in a JSON manifest
- Secure file download for allowlisted paths only
- No client plugin required (any launcher / tool can call the HTTP API)

## Endpoints

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/modinventory/api/manifest` | JSON list of mods + per-file `sha256` / size |
| GET | `/modinventory/api/file?path=` | Download one allowlisted file (game-root relative) |

Example `path`: `BepInEx/plugins/SomeMod/SomeMod.dll`

## Install

1. Download `ModInventory-*.zip` from [Releases](https://github.com/gadjed/ModInventory-SPT-mod/releases)
2. Extract the archive into your **SPT game root** (the folder that contains `EscapeFromTarkov.exe` / `SPT.Server.exe`)
3. Restart the SPT server

The zip already contains the correct paths:

```text
SPT/user/mods/ModInventory/ModInventory.dll
SPT/user/mods/ModInventory/config.json
```

On startup the server log should show:

```text
[ModInventory] Ready. Game root: ...
```

## Config

Edit `SPT/user/mods/ModInventory/config.json`:

```json
{
  "scanRoots": [
    "BepInEx/plugins",
    "BepInEx/patchers",
    "SPT/user/mods",
    "user/mods"
  ],
  "excludeModFolders": [
    "ModInventory"
  ]
}
```

| Key | Description |
|-----|-------------|
| `scanRoots` | Folders relative to the game root that are inventoried and may be downloaded |
| `excludeModFolders` | Top-level mod folder names skipped from the manifest (default includes `ModInventory`) |

Profiles, logs, and `.pdb` files are never listed or served.

## Build from source

Requires **.NET 9** SDK.

```bash
dotnet build ModInventory.csproj -c Release
```

Output is copied to `Build/SPT/user/mods/ModInventory/`.

## License

MIT — see [LICENSE](LICENSE).
