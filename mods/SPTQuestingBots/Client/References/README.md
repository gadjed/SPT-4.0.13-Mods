# Client references

Place SPT / EFT hollowed assemblies here for local builds (do **not** commit DLLs).

Minimum set (names as used by the `.csproj`):

- `hollowed.dll` (used as `Assembly-CSharp`)
- `0Harmony.dll`, `BepInEx.dll`
- `Comfort.dll`, `Comfort.Unity.dll`, `CommonExtensions.dll`
- `DissonanceVoip.dll`, `ItemComponent.Types.dll`, `Newtonsoft.Json.dll`, `Sirenix.Serialization.dll`
- `DrakiaXYZ-BigBrain.dll`
- `spt-common.dll`, `spt-custom.dll`, `spt-reflection.dll`, `spt-singleplayer.dll`
- `UnityEngine.dll`, `UnityEngine.CoreModule.dll`, `UnityEngine.AIModule.dll`, `UnityEngine.IMGUIModule.dll`, `UnityEngine.PhysicsModule.dll`, `UnityEngine.TextRenderingModule.dll`, `UnityEngine.UI.dll`
- `Unity.Postprocessing.Runtime.dll`, `Unity.TextMeshPro.dll`

You can copy most of these from an SPT 4.0.13 install or from another mod's `References` folder (e.g. SAIN).
