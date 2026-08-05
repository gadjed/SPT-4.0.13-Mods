# Defib Ally Revive

| | |
|---|---|
| **Version** | v1.0.1 |
| **Type** | Client + Server / Клієнт + Сервер |
| **Target** | SPT 4.0.13 only |

---

## English

### Description
Allows you to **revive downed allies** using a **Portable defibrillator** bound to the **quick-slot (hotkey) bar**.

1. Put a defibrillator in pockets / tactical rig and bind it to a quick slot (the mod unlocks binding for this barter item).
2. When a GroupId teammate (e.g. Fika squad) would die, they enter a **downed** state instead.
3. Stand next to them and press the defibrillator hotkey — channel the revive (default 5s). On success the defibrillator is consumed.

Soft Fika support: if a teammate is already in Fika’s native downed/revive state, the same hotkey starts Fika’s revive when in range.

The vanilla defibrillator is barter loot with **0 resource**, which the hotkey UI would show as **0/0**.  
**v1.0.1** fixes that: UI shows a single charge, and the server sets the template to **1/1**.

### Settings (F12 / `BepInEx/config/gadjed.defiballyrevive.cfg`)
| Setting | Default | Meaning |
|---------|---------|---------|
| Enabled | true | Master toggle |
| ReviveRange | 3.5 | Max distance (m) |
| ReviveTime | 5 | Channel duration (s) |
| BleedoutTime | 90 | Seconds until permanent death (0 = no timer) |
| RequireSameGroup | true | Only same `GroupId` (squad) |
| AllowSameSide | false | Also same USEC/BEAR side (when group rule allows) |
| ConsumeDefibrillator | true | Destroy defib after success |
| FullHealOnRevive | true | Full heal vs vital parts only |
| Debug | false | Verbose logs |

### Install
```text
BepInEx/plugins/DefibAllyRevive.dll
SPT/user/mods/DefibAllyRevive/DefibAllyRevive.dll
```
(On flat layouts also `user/mods/DefibAllyRevive/`.)

### Notes
- Defibrillator template id: `5c052e6986f7746b207bc3c9`
- Solo without a shared GroupId: disable **RequireSameGroup** and enable **AllowSameSide** if you want same-faction AI to be revivable
- Fully dead corpses (already despawned) cannot be revived — only the downed window

---

## Українська

### Опис
Дозволяє **піднімати / оживлювати союзників**, використовуючи **портативний дефібрилятор** з панелі **хоткеїв**.

1. Поклади дефібрилятор у кишені / розвантаження і прив’яжи до слота швидкого використання (мод дозволяє прив’язку цього бартерного предмета).
2. Коли союзник з тим самим `GroupId` (наприклад, відділення Fika) мав би померти — він падає в стан **downed**.
3. Підійди і натисни хоткей з дефібрилятором — канал оживлення (за замовч. 5 с). Після успіху дефібрилятор витрачається.

М’яка підтримка Fika: якщо напарник уже в рідному downed Fika, той самий хоткей запускає їхнє оживлення в радіусі.

У ванілі дефібрилятор — бартер з **0 ресурсу**, тому хоткей показував **0/0**.  
**v1.0.1** це виправляє: у UI одне використання, на сервері шаблон **1/1**.

### Встановлення
```text
BepInEx/plugins/DefibAllyRevive.dll
SPT/user/mods/DefibAllyRevive/DefibAllyRevive.dll
```
