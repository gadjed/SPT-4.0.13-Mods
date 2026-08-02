# Med Rebalance / Ребаланс медицини

| | |
|---|---|
| **Version** | v1.3.0 |
| **Type** | Server + Client / Сервер + клієнт |
| **Source** | https://github.com/gadjed/MedRebalance-SPT-mod |
| **Target** | SPT 4.0.13 |

---

## English

### Description
**Medicine rebalance:** shorter surgical kit / splint use time (default 5s), continuous healing across limbs, scratch top-ups (2–3 HP from medkit resource), and cancel-on-damage (restores last weapon). Replaces standalone Continuous Healing and the old Fast Surgery install paths.

### Settings
Server (`SPT/user/mods/MedRebalance/config.json`):
- **UseTimeSeconds** (default: 5)
- Per-item toggles: Immobilizing splint, Aluminum splint, CMS, Surv12

Client (F12): Continuous Healing, Heal Limbs, Heal Delay, Reset Animations, Cancel On Damage, Scratch Heal amount / max missing HP.

### Notes
Install:
```text
SPT/user/mods/MedRebalance/
BepInEx/plugins/MedRebalance.Client.dll
```
Remove old `FastSurgery` / `ContinuousHealing` folders if present.

---

## Українська

### Опис
**Ребаланс медицини:** швидші CMS / шини (типово 5 с), безперервне лікування по кінцівках, підліковування царапок (2–3 HP з ресурсу аптечки) і переривання лікування при уроні (повертає зброю). Замінює окремий Continuous Healing і старі шляхи Fast Surgery.

### Налаштування
Сервер (`SPT/user/mods/MedRebalance/config.json`):
- **UseTimeSeconds** (типово: 5)
- Перемикачі предметів: Immobilizing splint, Aluminum splint, CMS, Surv12

Клієнт (F12): Continuous Healing, Heal Limbs, Heal Delay, Reset Animations, Cancel On Damage, Scratch Heal.

### Примітки
Встановлення:
```text
SPT/user/mods/MedRebalance/
BepInEx/plugins/MedRebalance.Client.dll
```
Видаліть старі `FastSurgery` / `ContinuousHealing`, якщо вони лишились.
