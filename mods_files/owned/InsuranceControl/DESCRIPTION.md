# Insurance Control

| | |
|---|---|
| **Version** | v1.1.0 |
| **Type** | Client + Server / Клієнт + Сервер |
| **Source** | https://github.com/gadjed/Insurance-refund-SPT-mod |
| **Target** | SPT 4.0.13 |

---

## English

### Description
Insurance return timing, lost chance, and magazine/container contents on the **server**, plus an **Insure All** stash button on the **client** (Prapor or Therapist via F12).

### Settings
**Client (F12 / `gadjed.insurancerefund.cfg`):** button enable, label, insurer, layout, debug.
**Server (`config.json`):** return delay, lost chance, content enrichment.

### Notes
Install:
```
SPT/user/mods/InsuranceControl/
BepInEx/plugins/InsuranceControl.Client.dll
```
Remove old `InsureAllPrapor.dll` if present.

---

## Українська

### Опис
Час повернення страховки / шанс втрати / вміст магазинів і сумок на **сервері**, плюс кнопка **«Застраховать все»** на **клієнті** (Прапор або Терапевт у F12).

### Налаштування
**Клієнт (F12):** кнопка, текст, страховщик, позиція/розмір, debug.
**Сервер (`config.json`):** затримка, шанс втрати, збагачення вмісту.

### Примітки
```
SPT/user/mods/InsuranceControl/
BepInEx/plugins/InsuranceControl.Client.dll
```
Видаліть старий `InsureAllPrapor.dll`.
