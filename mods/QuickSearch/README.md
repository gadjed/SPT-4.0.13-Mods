# Quick Search

**SPT 4.1.0 Compatible**

Client mod that makes searching **any container or corpse three times faster**.

Developed and tested against **SPT 4.1.0**.

[Latest release](https://github.com/gadjed/Quick-search-SPT-mod/releases/latest) · [License: MIT](LICENSE)

## Features

- Speeds up the initial **Searching...** delay
- Speeds up per-item reveal inside containers, jackets, bags, corpses, etc.
- Configurable multiplier (default **3×**)
- F12 Configuration Manager support
- No server mod required

## Install

1. Download `QuickSearch-*.zip` from [Releases](https://github.com/gadjed/Quick-search-SPT-mod/releases)
2. Extract the archive into your **SPT game root** (the folder that contains `EscapeFromTarkov.exe` / `BepInEx/`)
3. Launch the game

The zip already contains the correct path:

```text
BepInEx/plugins/QuickSearch.dll
```

On load the BepInEx log should show:

```text
[Info   :Quick Search] Quick Search v1.0.0 loaded (x3 search speed).
[Info   :Quick Search] [QuickSearch] Patched initial delay via ...
[Info   :Quick Search] [QuickSearch] Patched item reveal via ...
```

## Config

Edit in-game via **F12**, or `BepInEx/config/gadjed.quicksearch.cfg`:

| Key | Default | Description |
|-----|---------|-------------|
| `SearchSpeedMultiplier` | `3` | How many times faster search is (`1` = vanilla, `3` = 3× faster) |

## Build from source

Requires **.NET SDK** and these DLLs in `References/` (from your SPT install):

- `BepInEx/core/BepInEx.dll`
- `BepInEx/core/0Harmony.dll`
- `EscapeFromTarkov_Data/Managed/UnityEngine.dll`
- `EscapeFromTarkov_Data/Managed/UnityEngine.CoreModule.dll`

```bash
dotnet build QuickSearch.csproj -c Release
```

Output is copied to `Build/SPT/BepInEx/plugins/`.

## License

MIT — see [LICENSE](LICENSE).
