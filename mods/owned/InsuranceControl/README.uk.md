# Insurance Control

**Сумісність: SPT 4.0.13**

Об’єднаний **server + client** мод: правила повернення страховки на сервері + кнопка **«Застраховать все»** на клієнті (колишній Insure All Prapor).

## Встановлення

```text
SPT/user/mods/InsuranceControl/InsuranceControl.dll
SPT/user/mods/InsuranceControl/config.json
BepInEx/plugins/InsuranceControl.Client.dll
```

Видаліть старий `BepInEx/plugins/InsureAllPrapor.dll`, якщо був.

## F12 (клієнт)

Увімкнення кнопки, текст, страховщик (Прапор / Терапевт), розмір і позиція кнопки, debug-лог.

## Сервер

`config.json` біля DLL — час повернення, шанс втрати, вміст магазинів/сумок (не в F12).

Деталі англійською: [README.md](README.md).
