#!/usr/bin/env bash
# Rebuild SPT-4.0.13-ModPack.zip from last known-good full pack (v1.2.4),
# replacing FastSurgery/ContinuousHealing with MedRebalance 1.3.0 and
# applying safe 4.0.13 overlays. Always writes forward-slash zip paths.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
STAGE="${TMPDIR:-/tmp}/spt-4013-modpack-rebuild"
BASE_ZIP="${BASE_ZIP:-/tmp/spt-pack-124/SPT-4.0.13-ModPack.zip}"
OUT_DIR="${ROOT}/mods_patch"
OUT_ZIP="${OUT_DIR}/SPT-4.0.13-ModPack.zip"
OUT_TREE="${OUT_DIR}/SPT-4.0.13-ModPack"

die() { echo "ERROR: $*" >&2; exit 1; }

extract_zip_norm() {
  local zip="$1" dest="$2"
  python3 - "$zip" "$dest" <<'PY'
import sys, zipfile, pathlib
src, dest = sys.argv[1], pathlib.Path(sys.argv[2])
dest.mkdir(parents=True, exist_ok=True)
with zipfile.ZipFile(src) as z:
    for info in z.infolist():
        name = info.filename.replace("\\", "/")
        if not name or name.endswith("/"):
            (dest / name).mkdir(parents=True, exist_ok=True)
            continue
        target = dest / name
        target.parent.mkdir(parents=True, exist_ok=True)
        with z.open(info) as s, open(target, "wb") as d:
            d.write(s.read())
PY
}

merge_tree() {
  local src="$1" dest="$2"
  mkdir -p "$dest"
  if command -v rsync >/dev/null 2>&1; then
    rsync -a "$src"/ "$dest"/
  else
    cp -R "$src"/. "$dest"/
  fi
}

mirror_server_mods() {
  local root="$1"
  local flat="${root}/user/mods"
  local nested="${root}/SPT/user/mods"
  mkdir -p "$flat" "$nested"
  local flat_n nested_n
  flat_n=$(find "$flat" -mindepth 1 -maxdepth 1 2>/dev/null | wc -l | tr -d ' ')
  nested_n=$(find "$nested" -mindepth 1 -maxdepth 1 2>/dev/null | wc -l | tr -d ' ')
  if [[ "$nested_n" -gt "$flat_n" ]]; then
    merge_tree "$nested" "$flat"
  elif [[ "$flat_n" -gt 0 ]]; then
    merge_tree "$flat" "$nested"
  fi
  merge_tree "$flat" "$nested"
  merge_tree "$nested" "$flat"
}

write_zip_forward() {
  local src_dir="$1" zip_path="$2"
  python3 - "$src_dir" "$zip_path" <<'PY'
import sys, zipfile, pathlib
src = pathlib.Path(sys.argv[1])
out = pathlib.Path(sys.argv[2])
if out.exists():
    out.unlink()
with zipfile.ZipFile(out, "w", compression=zipfile.ZIP_DEFLATED) as z:
    for path in sorted(src.rglob("*")):
        if path.is_dir():
            continue
        rel = path.relative_to(src).as_posix()
        z.write(path, rel)
print(f"Wrote {out} ({out.stat().st_size} bytes)")
PY
}

[[ -f "$BASE_ZIP" ]] || die "Base zip missing: $BASE_ZIP (download v1.2.4 first)"

rm -rf "$STAGE"
mkdir -p "$STAGE/pack" "$STAGE/overlay"

echo "==> Extract base v1.2.4 (normalize paths)"
extract_zip_norm "$BASE_ZIP" "$STAGE/pack"

echo "==> Remove FastSurgery / ContinuousHealing leftovers"
rm -rf \
  "$STAGE/pack/user/mods/FastSurgery" \
  "$STAGE/pack/SPT/user/mods/FastSurgery" \
  "$STAGE/pack/BepInEx/plugins/ContinuousHealing" \
  "$STAGE/pack/BepInEx/plugins/ContinuousHealing.dll" \
  "$STAGE/pack/BepInEx/plugins/FastSurgery.Client.dll" \
  "$STAGE/pack/BepInEx/plugins/FastSurgery.dll"

echo "==> Overlay MedRebalance 1.3.0"
extract_zip_norm "${ROOT}/mods_files/owned/MedRebalance/MedRebalance-1.3.0.zip" "$STAGE/overlay/med"
merge_tree "$STAGE/overlay/med" "$STAGE/pack"
if [[ -d "$STAGE/pack/SPT/user/mods/MedRebalance" ]]; then
  mkdir -p "$STAGE/pack/user/mods"
  merge_tree "$STAGE/pack/SPT/user/mods/MedRebalance" "$STAGE/pack/user/mods/MedRebalance"
fi

echo "==> Overlay YellowFlareCurse 1.4.5 (net9 / SPT 4.0.13)"
rm -rf \
  "$STAGE/pack/user/mods/YellowFlareCurse" \
  "$STAGE/pack/SPT/user/mods/YellowFlareCurse" \
  "$STAGE/pack/BepInEx/plugins/YellowFlareCurse.Client.dll" \
  "$STAGE/overlay/yfc"
extract_zip_norm "${ROOT}/mods_files/owned/YellowFlareCurse/YellowFlareCurse-1.4.5.zip" "$STAGE/overlay/yfc"
merge_tree "$STAGE/overlay/yfc" "$STAGE/pack"

echo "==> Overlay AutoMedHotkeys 1.0.3"
rm -rf "$STAGE/overlay/amh"
extract_zip_norm "${ROOT}/mods_files/owned/AutoMedHotkeys/AutoMedHotkeys-1.0.3.zip" "$STAGE/overlay/amh"
merge_tree "$STAGE/overlay/amh" "$STAGE/pack"

echo "==> Overlay InsuranceControl 1.0.1"
rm -rf "$STAGE/overlay/ins"
extract_zip_norm "${ROOT}/mods_files/owned/InsuranceControl/InsuranceControl-1.0.1.zip" "$STAGE/overlay/ins"
merge_tree "$STAGE/overlay/ins" "$STAGE/pack"

echo "==> Overlay InsureAllPrapor 1.0.3 (stash insure-all with Prapor)"
rm -rf \
  "$STAGE/pack/BepInEx/plugins/InsureAllPrapor.dll" \
  "$STAGE/overlay/iap"
extract_zip_norm "${ROOT}/mods_files/owned/InsureAllPrapor/InsureAllPrapor-1.0.3.zip" "$STAGE/overlay/iap"
merge_tree "$STAGE/overlay/iap" "$STAGE/pack"

echo "==> Overlay PackItems 1.0.0 (stash context menu — pack into cases)"
rm -rf \
  "$STAGE/pack/BepInEx/plugins/PackItems.dll" \
  "$STAGE/overlay/packitems"
extract_zip_norm "${ROOT}/mods_files/owned/PackItems/PackItems-1.0.0.zip" "$STAGE/overlay/packitems"
merge_tree "$STAGE/overlay/packitems" "$STAGE/pack"

echo "==> Overlay DefibAllyRevive 1.0.1 (revive allies + fix 0/0 defibrillator uses)"
rm -rf \
  "$STAGE/pack/BepInEx/plugins/DefibAllyRevive.dll" \
  "$STAGE/pack/user/mods/DefibAllyRevive" \
  "$STAGE/pack/SPT/user/mods/DefibAllyRevive" \
  "$STAGE/overlay/defib"
extract_zip_norm "${ROOT}/mods_files/owned/DefibAllyRevive/DefibAllyRevive-1.0.1.zip" "$STAGE/overlay/defib"
merge_tree "$STAGE/overlay/defib" "$STAGE/pack"

echo "==> Overlay ModInventory 0.1.0 (host delta-sync API; bootstrap for manage-modpack clients)"
rm -rf \
  "$STAGE/pack/user/mods/ModInventory" \
  "$STAGE/pack/SPT/user/mods/ModInventory" \
  "$STAGE/overlay/modinv"
extract_zip_norm "${ROOT}/mods_files/owned/ModInventory/ModInventory-0.1.0.zip" "$STAGE/overlay/modinv"
merge_tree "$STAGE/overlay/modinv" "$STAGE/pack"

echo "==> Overlay SAIN StealthEngage 4.4.4"
rm -rf \
  "$STAGE/pack/BepInEx/plugins/SAIN" \
  "$STAGE/pack/user/mods/Solarint-SAIN-ServerMod" \
  "$STAGE/pack/SPT/user/mods/Solarint-SAIN-ServerMod" \
  "$STAGE/overlay/sain"
extract_zip_norm "${ROOT}/mods_files/forks/SAIN/SAIN-StealthEngage-4.4.4.zip" "$STAGE/overlay/sain"
merge_tree "$STAGE/overlay/sain" "$STAGE/pack"

echo "==> Overlay FastSellInFlea 1.2.0"
rm -rf "$STAGE/overlay/fsf"
extract_zip_norm "${ROOT}/mods_files/external/FastSellInFlea/Kat-FastSellInFlea-1.2.0.zip" "$STAGE/overlay/fsf"
merge_tree "$STAGE/overlay/fsf" "$STAGE/pack"

echo "==> Overlay Skipper 1.1.4 (quest skip)"
rm -rf \
  "$STAGE/pack/BepInEx/plugins/Terkoiz.Skipper.dll" \
  "$STAGE/overlay/skipper"
extract_zip_norm "${ROOT}/mods_files/external/Skipper/Terkoiz-Skipper-1.1.4-overlay.zip" "$STAGE/overlay/skipper"
merge_tree "$STAGE/overlay/skipper" "$STAGE/pack"

echo "==> Overlay AmandsGraphics 1.7.0 (brightness / post FX; SPT 4.0 only)"
rm -rf \
  "$STAGE/pack/BepInEx/plugins/AmandsGraphics" \
  "$STAGE/overlay/amands"
extract_zip_norm "${ROOT}/mods_files/external/AmandsGraphics/AmandsGraphics-1.7.0-overlay.zip" "$STAGE/overlay/amands"
merge_tree "$STAGE/overlay/amands" "$STAGE/pack"

echo "==> Overlay Saria Trader 2.0.5 (BlackRock + SPEAR 6.8 ammo/mags)"
rm -rf \
  "$STAGE/pack/user/mods/Saria" \
  "$STAGE/pack/SPT/user/mods/Saria" \
  "$STAGE/overlay/saria"
extract_zip_norm "${ROOT}/mods_files/forks/Saria-4.x.x/SariaTrader2.0-2.0.5.zip" "$STAGE/overlay/saria"
merge_tree "$STAGE/overlay/saria" "$STAGE/pack"

echo "==> Mirror server mods to user/mods and SPT/user/mods"
mirror_server_mods "$STAGE/pack"

BUILT_AT="$(date -u +"%Y-%m-%dT%H:%M:%SZ")"
cat > "$STAGE/pack/MANIFEST.txt" <<EOF
# Mod pack manifest
# Built: ${BUILT_AT}
# Target: SPT 4.0.13
# Pack: v1.2.15
# Server mods: user/mods + SPT/user/mods (mirrored)

MOD                      RELEASE_TAG
---                      -----------
AmandsGraphics           1.7.0
AutoMedHotkeys           v1.0.3
DefibAllyRevive          v1.0.1
DynamicMaps              1.1.3
EnableLabyrinth          1.0.2
FastSellInFlea           1.2.0
FastTaxi                 v1.0.0
Fika-Plugin              v2.3.9
Fika-Server              v2.3.5
GildedKeyStorage         2.0.4
InsureAllPrapor          v1.0.3
InsuranceControl         v1.0.1
LiveFleaPrices           2.0.1
LootingBots              v1.7.0-spt-4.0
MedRebalance             v1.3.0
ModInventory             v0.1.0
MoreBotsAPI              2.0.1
MoreCheckmarks           v2.2.0
PackItems                v1.0.0
QuickSearch              v1.0.0
SAIN                     v4.4.4
SPT-BigBrain             1.4.0
SPT-Waypoints            1.8.2
SPTQuestingBots          v0.12.1
Saria-4.x.x              v2.0.5
Skipper                  1.1.4
UIFixes                  v5.3.11
UnbreakableKeys          2.0.0
YellowFlareCurse         v1.4.5

EXCLUDED: QuestingBots-DanW, ScavPopulation (folded into SPTQuestingBots Continuous), ContinuousHealing / FastSurgery (folded into MedRebalance), SVM (no redistribution; install from upstream via manage-modpack)
EOF

cat > "$STAGE/pack/INSTALL.txt" <<'EOF'
SPT 4.0.13 вЂ” Mod Pack
=====================

Р’СЃС‚Р°РЅРѕРІР»РµРЅРЅСЏ
------------
1. Р’СЃС‚Р°РЅРѕРІРё С‡РёСЃС‚РёР№ SPT 4.0.13 С– РѕРґРёРЅ СЂР°Р· Р·Р°РїСѓСЃС‚Рё SPT.Server / РіСЂСѓ.
2. Р РѕР·РїР°РєСѓР№ РІРјС–СЃС‚ С†С–С”С— С‚РµРєРё (Р°Р±Рѕ Р°СЂС…С–РІСѓ) РџР РЇРњРћ РІ РєРѕСЂС–РЅСЊ SPT
   (С‚СѓРґР°, РґРµ Р»РµР¶Р°С‚СЊ EscapeFromTarkov.exe, BepInEx/, С– SPT.Server.exe Р°Р±Рѕ SPT/SPT.Server.exe).
3. РџС–РґС‚РІРµСЂРґСЊ Р·Р»РёС‚С‚СЏ С‚РµРє BepInEx С– user / SPT/user.
4. Р—Р°РїСѓСЃС‚Рё SPT.Server С– РїРµСЂРµРІС–СЂ Р»РѕРі Р·Р°РІР°РЅС‚Р°Р¶РµРЅРЅСЏ РјРѕРґС–РІ.

Р©Рѕ РІ РїР°РєРµС‚С–
-----------
РЎРµСЂРІРµСЂРЅС– РјРѕРґРё РІ user/mods/ С– РґР·РµСЂРєР°Р»СЊРЅРѕ РІ SPT/user/mods/.
РљР»С–С”РЅС‚СЃСЊРєС– РІ BepInEx/plugins/ (+ patchers/).
QuestingBots Continuous РІРєР»СЋС‡Р°С” continuous population (РєРѕР»РёС€РЅС–Р№ Scav Population).
Med Rebalance 1.3.0 (РєРѕР»РёС€РЅС–Р№ Fast Surgery + Continuous Healing).
YellowFlareCurse 1.4.5 (4.0.13/net9), AutoMedHotkeys 1.0.3, FastSellInFlea 1.2.0,
InsureAllPrapor 1.0.3 (РєРЅРѕРїРєР° В«Р—Р°СЃС‚СЂР°С…РѕРІР°С‚СЊ РІСЃРµВ» Сѓ РџСЂР°РїРѕСЂР° РЅР° СЃС…СЂРѕРЅС–),
PackItems 1.0.0 (stash context menu — pack matching items into cases),
DefibAllyRevive 1.0.1 (revive downed allies with defibrillator on quick slots; fix 0/0 uses),
SAIN StealthEngage 4.4.4, Saria 2.0.5, Gilded Key Storage 2.0.4, Live Flea Prices 2.0.1,
ModInventory 0.1.0 (host API for later client delta-sync),
Skipper 1.1.4 (quest skip), AmandsGraphics 1.7.0 (brightness / post FX; SPT 4.0 only).

Р’РёРєР»СЋС‡РµРЅРѕ РЅР°РІРјРёСЃРЅРѕ
------------------
- QuestingBots (DanW) вЂ” Р·Р°РјС–РЅРµРЅРѕ РЅР° QuestingBots Continuous
- Scav Population вЂ” С„СѓРЅРєС†С–РѕРЅР°Р» СѓРІС–Р№С€РѕРІ Сѓ QuestingBots Continuous
- ContinuousHealing / FastSurgery вЂ” Р·Р°РјС–РЅРµРЅРѕ РЅР° MedRebalance
- BigBrain Debug.dll
- SVM / Greed.exe вЂ” РЅРµ РІС…РѕРґРёС‚СЊ Сѓ Р·Р±С–СЂРєСѓ (Р»С–С†РµРЅР·С–СЏ Р·Р°Р±РѕСЂРѕРЅСЏС” СЂРµРґРёСЃС‚СЂРёР±СѓС†С–СЋ).
  Р—Р° Р±Р°Р¶Р°РЅРЅСЏРј РїРѕСЃС‚Р°РІ РѕРєСЂРµРјРѕ С‡РµСЂРµР· manage-modpack (РїСѓРЅРєС‚ В«Р’СЃС‚Р°РЅРѕРІРёС‚Рё SVMВ»).

РџСЂРёРјС–С‚РєРё
--------
- Fika: РєР»С–С”РЅС‚ 2.3.9 + СЃРµСЂРІРµСЂ 2.3.5 Р· GitHub releases.
- ModInventory РїРѕС‚СЂС–Р±РµРЅ РЅР° С…РѕСЃС‚С– Р· РїРµСЂС€РѕРіРѕ РІСЃС‚Р°РЅРѕРІР»РµРЅРЅСЏ; РґР°Р»С– РєР»С–С”РЅС‚Рё РјРѕР¶СѓС‚СЊ
  РѕРЅРѕРІР»СЋРІР°С‚Рё Р»РёС€Рµ Р·РјС–РЅРµРЅС– С„Р°Р№Р»Рё С‡РµСЂРµР· /modinventory/api/*.
- РђРІС‚РѕС–РЅСЃС‚Р°Р»РµСЂ РїСЂРё РѕРЅРѕРІР»РµРЅРЅС– РїСЂРёР±РёСЂР°С” СЃС‚Р°СЂС– FastSurgery / ContinuousHealing.
EOF

echo "==> Validate required server mods"
REQUIRED=(
  fika-server
  Solarint-SAIN-ServerMod
  QuestingBotsContinuous
  YellowFlareCurse
  MedRebalance
  InsuranceControl
  FastTaxi
  ModInventory
  Saria
  DrakiaXYZ-GildedKeyStorage
  DrakiaXYZ-LiveFleaPrices
  MoreBotsServer
  MoreCheckmarksBackend
  Skwizzy-LootingBots-ServerMod
  Tyfon.UIFixes.Server
  acidphantasm-enablelabyrinth
  mpstark-dynamicmaps
  unbreakableKeys
)
missing=0
for m in "${REQUIRED[@]}"; do
  if [[ ! -d "$STAGE/pack/user/mods/$m" && ! -d "$STAGE/pack/SPT/user/mods/$m" ]]; then
    echo "MISSING server mod: $m" >&2
    missing=1
  fi
done
[[ "$missing" -eq 0 ]] || die "server mod validation failed"

echo "==> Reject any net10 / SPT 4.1 server DLLs (this pack is SPT 4.0.13 / net9 only)"
python3 - "$STAGE/pack" <<'PY' || die "net10 server DLL validation failed"
import sys
from pathlib import Path
root = Path(sys.argv[1])
bad = []
for dll in root.rglob("*.dll"):
    rel = dll.relative_to(root).as_posix()
    if "user/mods/" not in rel:
        continue
    data = dll.read_bytes()
    if b".NETCoreApp,Version=v10" in data:
        bad.append(rel)
if bad:
    print("ERROR: net10 server DLLs found (SPT 4.1 builds leaked into 4.0.13 pack):", file=sys.stderr)
    for p in bad:
        print(f"  {p}", file=sys.stderr)
    sys.exit(1)
print("OK: no net10 server mods")
PY

if find "$STAGE/pack" \( -iname '*FastSurgery*' -o -iname '*ContinuousHealing*' \) | grep -q .; then
  echo "Leftover FastSurgery/ContinuousHealing:" >&2
  find "$STAGE/pack" \( -iname '*FastSurgery*' -o -iname '*ContinuousHealing*' \) >&2
  die "legacy med leftovers present"
fi

echo "==> Publish tree + zip"
rm -rf "$OUT_TREE"
mkdir -p "$OUT_DIR"
cp -R "$STAGE/pack" "$OUT_TREE"
write_zip_forward "$OUT_TREE" "$OUT_ZIP"

echo "==> Summary"
python3 - "$OUT_ZIP" <<'PY'
import zipfile, sys
z=zipfile.ZipFile(sys.argv[1])
names=[n.replace('\\','/') for n in z.namelist()]
bs=sum('\\' in n for n in z.namelist())
mods=sorted({n.split('user/mods/',1)[1].split('/',1)[0] for n in names if 'user/mods/' in n and n.split('user/mods/',1)[1]})
print(f"entries={len(names)} backslash={bs} server_mods={len(mods)}")
print("mods:", ", ".join(mods))
for need in ["MedRebalance.Client.dll","AutoMedHotkeys.dll","InsureAllPrapor.dll","PackItems.dll","DefibAllyRevive.dll","user/mods/DefibAllyRevive/DefibAllyRevive.dll","YellowFlareCurse.Client.dll","Fika.Core.dll","SAIN/SAIN.dll","Kat.FastSellInFlea.dll","Terkoiz.Skipper.dll","AmandsGraphics/AmandsGraphics.dll"]:
    ok=any(need in n for n in names)
    print(f"  client {need}: {'OK' if ok else 'MISSING'}")
PY

echo "Done: $OUT_ZIP"
