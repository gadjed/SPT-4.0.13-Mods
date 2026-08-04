# SAIN StealthEngage

| | |
|---|---|
| **Version** | v4.4.4 |
| **Type** | Client + server / Клієнт + сервер |
| **Source** | https://github.com/gadjed/SAIN-StealthEngage-SPT-mod |
| **Target** | SPT 4.0.13 |

---

## English

### Description
Fork of SAIN: when a PMC is at peace and hears nearby gunfire, it approaches carefully (no sprint/charge) and engages once it still has the unseen/unheard advantage. Map-relative engage distance (default 70% of longer map axis; Customs ≈ 660 m).

### Settings
**Client:** F6 → Mind → Stealth Engage (map-relative fraction, min/max clamp). Full SAIN preset GUI otherwise.
**Server:** `SPT/user/mods/Solarint-SAIN-ServerMod/` (+ nickname personality data).

### Notes
Install: extract into SPT game root → `BepInEx/plugins/SAIN/` + `SPT/user/mods/Solarint-SAIN-ServerMod/`. Requires **BigBrain** ≥ 1.4.0 and **Waypoints**. **Replace** stock SAIN — do not run both (same GUID).

---

## Українська

### Опис
Форк SAIN: ЧВК у мирі чує постріл поруч → обережний підхід (без спринту/charge) і вступ у бій з переваги «не бачать / не чують». Дальність — від розміру мапи (типово 70% довшої осі; Таможня ≈ 660 м).

### Налаштування
**Клієнт:** F6 → Mind → Stealth Engage. Решта — стандартний GUI SAIN.
**Сервер:** `SPT/user/mods/Solarint-SAIN-ServerMod/`.

### Примітки
Встановлення в корінь SPT. Потрібні **BigBrain** і **Waypoints**. Замінює стоковий SAIN (не ставити обидва).
