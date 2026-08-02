#!/usr/bin/env bash
# Rebuild owned-mod release zips with Forge-correct SPT 4 layout:
#   SPT/user/mods/<Mod>/...
#   BepInEx/plugins/...
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
OUT="${TMPDIR:-/tmp}/spt-forge-repack"
SRC="${TMPDIR:-/tmp}/spt-zip-inspect"
rm -rf "$OUT"
mkdir -p "$OUT"

repack_server() {
  local oldzip="$1" name="$2" zipname="$3"
  local stage="$OUT/stage-$name-$$"
  local extract="$OUT/extract-$name-$$"
  rm -rf "$stage" "$extract"
  mkdir -p "$extract" "$stage/SPT/user/mods/$name"
  unzip -q "$oldzip" -d "$extract"
  if [[ -d "$extract/SPT/user/mods/$name" ]]; then
    cp -R "$extract/SPT/user/mods/$name/." "$stage/SPT/user/mods/$name/"
  elif [[ -d "$extract/user/mods/$name" ]]; then
    cp -R "$extract/user/mods/$name/." "$stage/SPT/user/mods/$name/"
  else
    echo "Cannot find $name in $oldzip" >&2
    return 1
  fi
  (cd "$stage" && zip -r -X -q "$OUT/$zipname" SPT)
  echo "OK $zipname"
  unzip -l "$OUT/$zipname" | sed -n '1,20p'
}

repack_client() {
  local oldzip="$1" zipname="$2"
  local stage="$OUT/stage-client-$$"
  local extract="$OUT/extract-client-$$"
  rm -rf "$stage" "$extract"
  mkdir -p "$extract" "$stage/BepInEx/plugins"
  unzip -q "$oldzip" -d "$extract"
  if [[ -d "$extract/BepInEx/plugins" ]]; then
    cp -R "$extract/BepInEx/plugins/." "$stage/BepInEx/plugins/"
  elif [[ -d "$extract/SPT/BepInEx/plugins" ]]; then
    cp -R "$extract/SPT/BepInEx/plugins/." "$stage/BepInEx/plugins/"
  else
    echo "Cannot find plugins in $oldzip" >&2
    return 1
  fi
  (cd "$stage" && zip -r -X -q "$OUT/$zipname" BepInEx)
  echo "OK $zipname"
  unzip -l "$OUT/$zipname" | sed -n '1,20p'
}

repack_combo() {
  local oldzip="$1" name="$2" zipname="$3"
  local stage="$OUT/stage-$name-$$"
  local extract="$OUT/extract-$name-$$"
  rm -rf "$stage" "$extract"
  mkdir -p "$extract" "$stage/SPT/user/mods/$name" "$stage/BepInEx/plugins"
  unzip -q "$oldzip" -d "$extract"
  if [[ -d "$extract/SPT/user/mods/$name" ]]; then
    cp -R "$extract/SPT/user/mods/$name/." "$stage/SPT/user/mods/$name/"
  else
    cp -R "$extract/user/mods/$name/." "$stage/SPT/user/mods/$name/"
  fi
  if [[ -d "$extract/BepInEx/plugins" ]]; then
    cp -R "$extract/BepInEx/plugins/." "$stage/BepInEx/plugins/"
  else
    cp -R "$extract/SPT/BepInEx/plugins/." "$stage/BepInEx/plugins/"
  fi
  (cd "$stage" && zip -r -X -q "$OUT/$zipname" SPT BepInEx)
  echo "OK $zipname"
  unzip -l "$OUT/$zipname" | sed -n '1,25p'
}

# Prefer local Build trees for 4.1.0 when available (same DLL content).
pack_from_build_server() {
  local name="$1" zipname="$2"
  local src="$ROOT/mods/$name/Build/SPT/user/mods/$name"
  local stage="$OUT/build-$name"
  rm -rf "$stage"
  mkdir -p "$stage/SPT/user/mods/$name"
  cp -R "$src/." "$stage/SPT/user/mods/$name/"
  (cd "$stage" && zip -r -X -q "$OUT/$zipname" SPT)
  echo "OK $zipname (from Build)"
  unzip -l "$OUT/$zipname" | sed -n '1,20p'
}

pack_from_build_client() {
  local zipname="$1"
  local dll="$ROOT/mods/QuickSearch/Build/SPT/BepInEx/plugins/QuickSearch.dll"
  local stage="$OUT/build-qs"
  rm -rf "$stage"
  mkdir -p "$stage/BepInEx/plugins"
  cp "$dll" "$stage/BepInEx/plugins/"
  (cd "$stage" && zip -r -X -q "$OUT/$zipname" BepInEx)
  echo "OK $zipname (from Build)"
  unzip -l "$OUT/$zipname" | sed -n '1,20p'
}

pack_from_build_combo() {
  local zipname="$1"
  local stage="$OUT/build-yfc"
  rm -rf "$stage"
  mkdir -p "$stage/SPT/user/mods/YellowFlareCurse" "$stage/BepInEx/plugins"
  cp -R "$ROOT/mods/YellowFlareCurse/Build/SPT/user/mods/YellowFlareCurse/." \
    "$stage/SPT/user/mods/YellowFlareCurse/"
  cp "$ROOT/mods/YellowFlareCurse/Build/SPT/BepInEx/plugins/YellowFlareCurse.Client.dll" \
    "$stage/BepInEx/plugins/"
  (cd "$stage" && zip -r -X -q "$OUT/$zipname" SPT BepInEx)
  echo "OK $zipname (from Build)"
  unzip -l "$OUT/$zipname" | sed -n '1,25p'
}

pack_from_build_sain() {
  local zipname="$1"
  local stage="$OUT/build-sain"
  local src_client="$ROOT/mods/SAIN/Build/BepInEx/plugins/SAIN"
  local src_server="$ROOT/mods/SAIN/Build/SPT/user/mods/Solarint-SAIN-ServerMod"
  rm -rf "$stage"
  mkdir -p "$stage/BepInEx/plugins/SAIN" "$stage/SPT/user/mods/Solarint-SAIN-ServerMod"
  cp -R "$src_client/." "$stage/BepInEx/plugins/SAIN/"
  cp -R "$src_server/." "$stage/SPT/user/mods/Solarint-SAIN-ServerMod/"
  if [[ -f "$ROOT/mods/SAIN/LICENSE" ]]; then
    cp "$ROOT/mods/SAIN/LICENSE" "$ROOT/mods/SAIN/NOTICE" "$ROOT/mods/SAIN/README.md" \
      "$stage/SPT/user/mods/Solarint-SAIN-ServerMod/" 2>/dev/null || true
  fi
  (cd "$stage" && zip -r -X -q "$OUT/$zipname" BepInEx SPT)
  echo "OK $zipname (from Build)"
  unzip -l "$OUT/$zipname" | sed -n '1,25p'
}

echo "=== 4.1.0 from Build ==="
pack_from_build_server MedRebalance "MedRebalance-1.3.0.zip"
pack_from_build_server FastTaxi "FastTaxi-1.1.0.zip"
pack_from_build_server InsuranceControl "InsuranceControl-1.1.0.zip"
pack_from_build_client "QuickSearch-1.1.0.zip"
pack_from_build_combo "YellowFlareCurse-1.1.0.zip"
pack_from_build_sain "SAIN-StealthEngage-4.4.4.zip"

echo "=== 4.0.13 from previous release zips ==="
repack_server "$SRC/old-MedRebalance-1.3.0.zip" MedRebalance "MedRebalance-1.3.0.zip"
repack_server "$SRC/old-FastTaxi-1.0.0.zip" FastTaxi "FastTaxi-1.0.0.zip"
repack_server "$SRC/old-InsuranceControl-1.0.0.zip" InsuranceControl "InsuranceControl-1.0.0.zip"
repack_client "$SRC/old-QuickSearch-1.0.0.zip" "QuickSearch-1.0.0.zip"
repack_combo "$SRC/old-YellowFlareCurse-1.0.0.zip" YellowFlareCurse "YellowFlareCurse-1.0.0.zip"

echo
echo "All zips:"
ls -la "$OUT"/*.zip
shasum -a 256 "$OUT"/*.zip | tee "$OUT/SHA256SUMS.txt"
