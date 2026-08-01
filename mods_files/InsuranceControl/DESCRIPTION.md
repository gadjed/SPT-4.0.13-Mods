# Insurance Control (Insurance Refund)

| | |
|---|---|
| **Version** | v1.0.0 |
| **Type** | Server / Сервер |
| **Source** | https://github.com/gadjed/Insurance-refund-SPT-mod |
| **Target** | SPT 4.1.0 |

---

## English

### Description
Controls insurance return timing, lost chance, and whether magazines/containers return with contents.

### Settings
(`user/mods/InsuranceControl/config.json`)
- **ReturnTimeOverrideSeconds** (3600; 0 = use TraderReturnHours)
- **RunIntervalSeconds** (60)
- **StorageTimeOverrideSeconds** (0)
- **ReturnMagazinesWithAmmo** / **ReturnContainersWithContents** (true)
- **SimulateItemsBeingTaken** (true)
- **LostChancePercent** — Prapor/Therapist (default 0)
- **TraderReturnHours** — per-trader hour ranges

### Notes
Install: `user/mods/InsuranceControl/`. Avoid stacking with other insurance-return mods.

---

## Українська

### Опис
Керує часом повернення страховки, шансом втрати та поверненням магазинів/контейнерів із вмістом.

### Налаштування
(`user/mods/InsuranceControl/config.json`)
- **ReturnTimeOverrideSeconds** (3600; 0 = брати TraderReturnHours)
- **RunIntervalSeconds** (60)
- **StorageTimeOverrideSeconds** (0)
- **ReturnMagazinesWithAmmo** / **ReturnContainersWithContents** (true)
- **SimulateItemsBeingTaken** (true)
- **LostChancePercent** — Прапор/Терапевт (типово 0)
- **TraderReturnHours** — діапазони годин по торговцях

### Примітки
Встановлення: `user/mods/InsuranceControl/`. Не ставте разом з іншими модами повернення страховки.
