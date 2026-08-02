# Fast Taxi

**SPT 4.0.13 Compatible**

Server mod that reduces paid car / taxi extract wait time from **60 seconds** to **8 seconds**.

Developed and tested against **SPT 4.0.13**.

[Latest release](https://github.com/gadjed/FastTaxi-SPT-mod/releases/latest) · [License: MIT](LICENSE)

## Features

- Configurable wait time for all paid V-Ex / taxi extracts (default **8s**)
- Applies to both PvE and standard `ExfiltrationTime` fields
- No client-side plugin required

## Affected extracts

| Map | Extract |
|-----|---------|
| Customs | Dorms V-Ex |
| Woods | South V-Ex |
| Interchange | PP Exfil |
| Lighthouse | V-Ex_light |
| Streets of Tarkov | Primorsky Ave Taxi (`E7_car`) |
| Shoreline | Shorl_V-Ex |
| Ground Zero | Sandbox_VExit |

## Install

1. Download `FastTaxi-*.zip` from [Releases](https://github.com/gadjed/FastTaxi-SPT-mod/releases)
2. Extract the archive into your **SPT game root** (the folder that contains `SPT.Server.exe` / `user/`)
3. Restart the SPT server

The zip already contains the correct paths:

```text
user/mods/FastTaxi/FastTaxi.dll
user/mods/FastTaxi/config.json
```

On startup the server log should show lines like:

```text
[FastTaxi] tarkovstreets/E7_car: ExfiltrationTime 60 -> 8s
[FastTaxi] Updated wait time on N car/taxi extract(s) to 8s.
```

## Config

Edit `user/mods/FastTaxi/config.json`:

```json
{
  "WaitTimeSeconds": 8
}
```

| Key | Description |
|-----|-------------|
| `WaitTimeSeconds` | Taxi / car extract countdown in seconds (default `8`, vanilla is `60`) |

## Build from source

Requires **.NET 9** SDK.

```bash
dotnet build FastTaxi.csproj -c Release
```

Output is copied to `Build/SPT/user/mods/FastTaxi/`.

## License

MIT — see [LICENSE](LICENSE).
