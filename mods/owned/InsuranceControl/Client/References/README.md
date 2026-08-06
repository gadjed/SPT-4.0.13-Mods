# References

Copy these DLLs from your **SPT 4.0.13** install (or from another client mod such as SAIN / QuickSearch / QuestingBots) before building if HintPaths break:

| File | Source |
|------|--------|
| `0Harmony.dll` | `BepInEx/core/` |
| `BepInEx.dll` | `BepInEx/core/` |
| `hollowed.dll` | publicized `Assembly-CSharp` (e.g. SAIN `References/`) |
| `Comfort.dll` | `EscapeFromTarkov_Data/Managed/` |
| `spt-reflection.dll` | `BepInEx/plugins/spt/` |
| `Sirenix.Serialization.dll` | `EscapeFromTarkov_Data/Managed/` |
| `UnityEngine.dll` / `UnityEngine.CoreModule.dll` | `EscapeFromTarkov_Data/Managed/` |
| `UnityEngine.UI.dll` | `EscapeFromTarkov_Data/Managed/` |

The `.csproj` currently references shared copies under `mods/owned/QuickSearch/References` and `mods/forks/SAIN` / `SPTQuestingBots`.
