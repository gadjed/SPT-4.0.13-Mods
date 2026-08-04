# Gilded Key Storage

| | |
|---|---|
| **Version** | 2.0.4 |
| **Type** | Client + server / Клієнт + сервер |
| **Source** | https://github.com/DrakiaXYZ/SPT-GildedKeyStorage-CSharp |
| **Target** | SPT 4.0.13 |

---

## English

### Description
Adds progression key cases, pouches, and keychains for consolidated key storage, with barters and key/case rules.

### Settings
(`user/mods/DrakiaXYZ-GildedKeyStorage/config/`, from `config.default.json`)
- **key_insurance_enabled** / **cases_insurance_enabled** (default: false)
- **cases_flea_banned** (true)
- **weightless_keys** (true)
- **no_key_use_limit** (false)
- **keys_are_discardable** (true)
- **all_keys_in_secure** (true)
- Plus `cases.json` / `barters.json` for case & trader data

### Notes
Install: client DLL in `BepInEx/plugins/` + `user/mods/DrakiaXYZ-GildedKeyStorage/`. May overlap with UnbreakableKeys if both change key durability/use limits.

---

## Українська

### Опис
Додає кейси/чохли/ланцюжки для ключів із прогресією, бартерами та правилами для ключів і кейсів.

### Налаштування
(`user/mods/DrakiaXYZ-GildedKeyStorage/config/`, з `config.default.json`)
- **key_insurance_enabled** / **cases_insurance_enabled** (типово: false)
- **cases_flea_banned** (true)
- **weightless_keys** (true)
- **no_key_use_limit** (false)
- **keys_are_discardable** (true)
- **all_keys_in_secure** (true)
- Також `cases.json` / `barters.json`

### Примітки
Встановлення: клієнтський DLL у `BepInEx/plugins/` + `user/mods/DrakiaXYZ-GildedKeyStorage/`. Може перетинатися з UnbreakableKeys щодо міцності/лімітів ключів.
