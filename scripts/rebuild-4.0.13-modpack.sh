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
extract_zip_norm "${ROOT}/mods_files/MedRebalance/MedRebalance-1.3.0.zip" "$STAGE/overlay/med"
merge_tree "$STAGE/overlay/med" "$STAGE/pack"
if [[ -d "$STAGE/pack/SPT/user/mods/MedRebalance" ]]; then
  mkdir -p "$STAGE/pack/user/mods"
  merge_tree "$STAGE/pack/SPT/user/mods/MedRebalance" "$STAGE/pack/user/mods/MedRebalance"
fi

echo "==> Overlay YellowFlareCurse 1.3.0"
rm -rf "$STAGE/overlay/yfc"
extract_zip_norm "${ROOT}/mods_files/YellowFlareCurse/YellowFlareCurse-1.3.0.zip" "$STAGE/overlay/yfc"
merge_tree "$STAGE/overlay/yfc" "$STAGE/pack"

echo "==> Overlay AutoMedHotkeys 1.0.3"
rm -rf "$STAGE/overlay/amh"
extract_zip_norm "${ROOT}/mods_files/AutoMedHotkeys/AutoMedHotkeys-1.0.3.zip" "$STAGE/overlay/amh"
merge_tree "$STAGE/overlay/amh" "$STAGE/pack"

echo "==> Overlay InsuranceControl 1.0.1"
rm -rf "$STAGE/overlay/ins"
extract_zip_norm "${ROOT}/mods_files/InsuranceControl/InsuranceControl-1.0.1.zip" "$STAGE/overlay/ins"
merge_tree "$STAGE/overlay/ins" "$STAGE/pack"

echo "==> Overlay SAIN StealthEngage 4.4.4"
rm -rf \
  "$STAGE/pack/BepInEx/plugins/SAIN" \
  "$STAGE/pack/user/mods/Solarint-SAIN-ServerMod" \
  "$STAGE/pack/SPT/user/mods/Solarint-SAIN-ServerMod" \
  "$STAGE/overlay/sain"
extract_zip_norm "${ROOT}/mods_files/SAIN/SAIN-StealthEngage-4.4.4.zip" "$STAGE/overlay/sain"
merge_tree "$STAGE/overlay/sain" "$STAGE/pack"

echo "==> Overlay FastSellInFlea 1.2.0"
rm -rf "$STAGE/overlay/fsf"
extract_zip_norm "${ROOT}/mods_files/FastSellInFlea/Kat-FastSellInFlea-1.2.0.zip" "$STAGE/overlay/fsf"
merge_tree "$STAGE/overlay/fsf" "$STAGE/pack"

echo "==> Mirror server mods to user/mods and SPT/user/mods"
mirror_server_mods "$STAGE/pack"

BUILT_AT="$(date -u +"%Y-%m-%dT%H:%M:%SZ")"
cat > "$STAGE/pack/MANIFEST.txt" <<EOF
# Mod pack manifest
# Built: ${BUILT_AT}
# Target: SPT 4.0.13
# Server mods: user/mods + SPT/user/mods (mirrored)

MOD                      RELEASE_TAG
---                      -----------
AutoMedHotkeys           v1.0.3
DynamicMaps              1.1.3
EnableLabyrinth          1.0.2
FastSellInFlea           1.2.0
FastTaxi                 v1.0.0
Fika-Plugin              v2.3.9
Fika-Server              v2.3.5
GildedKeyStorage         2.0.4
InsuranceControl         v1.0.1
LiveFleaPrices           2.0.1
LootingBots              v1.7.0-spt-4.0
MedRebalance             v1.3.0
MoreBotsAPI              2.0.1
MoreCheckmarks           v2.2.0
QuickSearch              v1.0.0
SAIN                     v4.4.4
SPT-BigBrain             1.4.0
SPT-Waypoints            1.8.2
SPTQuestingBots          v0.12.1
Saria-4.x.x              v2.0.1
UIFixes                  v5.3.11
UnbreakableKeys          2.0.0
YellowFlareCurse         v1.3.0

EXCLUDED: QuestingBots-DanW, ScavPopulation (folded into SPTQuestingBots Continuous), ContinuousHealing / FastSurgery (folded into MedRebalance), SVM (no redistribution; install from upstream via manage-modpack)
EOF

cat > "$STAGE/pack/INSTALL.txt" <<'EOF'
SPT 4.0.13 — Mod Pack
=====================

Встановлення
------------
1. Встанови чистий SPT 4.0.13 і один раз запусти SPT.Server / гру.
2. Розпакуй вміст цієї теки (або архіву) ПРЯМО в корінь SPT
   (туда, де лежать EscapeFromTarkov.exe, BepInEx/, і SPT.Server.exe або SPT/SPT.Server.exe).
3. Підтвердь злиття тек BepInEx і user / SPT/user.
4. Запусти SPT.Server і перевір лог завантаження модів.

Що в пакеті
-----------
Серверні моди в user/mods/ і дзеркально в SPT/user/mods/.
Клієнтські в BepInEx/plugins/ (+ patchers/).
QuestingBots Continuous включає continuous population (колишній Scav Population).
Med Rebalance 1.3.0 (колишній Fast Surgery + Continuous Healing).
YellowFlareCurse 1.3.0, AutoMedHotkeys 1.0.3, FastSellInFlea 1.2.0,
SAIN StealthEngage 4.4.4, Saria 2.0.1, Gilded Key Storage 2.0.4, Live Flea Prices 2.0.1.

Виключено навмисно
------------------
- QuestingBots (DanW) — замінено на QuestingBots Continuous
- Scav Population — функціонал увійшов у QuestingBots Continuous
- ContinuousHealing / FastSurgery — замінено на MedRebalance
- BigBrain Debug.dll
- SVM / Greed.exe — не входить у збірку (ліцензія забороняє редистрибуцію).
  За бажанням постав окремо через manage-modpack (пункт «Встановити SVM»).

Примітки
--------
- Fika: клієнт 2.3.9 + сервер 2.3.5 з GitHub releases.
- Автоінсталер при оновленні прибирає старі FastSurgery / ContinuousHealing.
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
for need in ["MedRebalance.Client.dll","AutoMedHotkeys.dll","YellowFlareCurse.Client.dll","Fika.Core.dll","SAIN/SAIN.dll","Kat.FastSellInFlea.dll"]:
    ok=any(need in n for n in names)
    print(f"  client {need}: {'OK' if ok else 'MISSING'}")
PY

echo "Done: $OUT_ZIP"
