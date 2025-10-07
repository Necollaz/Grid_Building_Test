# Grid Building Demo (Unity 2D)
## 📌 О проекте

2D-сцена с размещением и удалением зданий на грид-сетке. Управление — через UI-кнопки и New Input System. Данные о размещённых зданиях сохраняются между сессиями в JSON-файл (не PlayerPrefs).PPU = 32.

**Стек:** Unity 2022.3.56f, Zenject (DI), New Input System, JSON-конфиги (вместо ScriptableObject).

## 📸 GIF
![Demo](Docs/gifs/Recording_01.gif)
![Demo](Docs/gifs/Recording_02.gif)

## ✅ Реализовано
### UI и режимы

- Нижняя панель с:
  - каталогом зданий (иконки из каталога),
  - кнопками Place и Delete,
  - кнопками Save / Load / Export / Import (работа с данными).
- Визуальная подсветка активного режима.
- Защита от ложных кликов по миру если курсор над панелью/элементом UI — размещение/удаление не выполняются.

### Механики размещения

- Выбор здания → появляется курсор-превью, который:
   - следует за мышью по грид-сетке,
   - подсвечивает валидность клетки.
- Подтверждение ЛКМ/Enter/Space — фиксирует объект на сетке.
- Удаление: клик ЛКМ по занятой клетке в режиме Delete.

### Сохранение / загрузка
- `Save` — сериализация всех установленных объектов в placed_buildings.json.
- `Load` — полная пересборка мира из файла.
- `Export` — выгрузка текущего состояния в JSON-строку (попадает в TMP_InputField).
- `Import` — чтение JSON из поля и пересборка мира.

### Архитектура и код

- Слоистая структура:
   - `Domain` — грид, описания зданий.
   - `Application` — логика размещения/удаления/выбора/режимов.
   - `Infrastructure` — ввод, конфиги, файловое хранилище.
   - `Presentation` — UI, курсор, фабрика и вьюхи.

- Zenject: ProjectInstaller, PresentationInstaller.
- Assembly Definition на каждый модуль (корректная модульность и быстрые сборки).

## 🕹️ Управление
| Действие                      | Клавиши / Мышь                              |
|------------------------------|---------------------------------------------|
| Переключить режим            | Кнопки **Place** / **Delete** внизу         |
| Выбор здания                 | Клик по иконке в каталоге                   |
| Перемещение по сетке         | Движение мышью; шаг — 1 клетка              |
| Доп. смещения                | **WASD** или стрелки                        |
| Поворот                      | **Q / E** (шаг 90°)                         |
| Подтвердить                  | **ЛКМ**, **Enter**, **Space**               |
| Отмена/вторичная кнопка      | **ПКМ**, **Esc**                            |
| Сохранить / Загрузить        | Кнопки **Save / Load**                      |
| Экспорт в JSON               | **Export** (строка в TMP InputField)        |
| Импорт из JSON               | Вставить JSON в поле → **Import**           |

Примечание: нажатия не срабатывают, если курсор над панелью/элементом UI или нажатие началось над UI.

## 🧠 Детали реализации
- `GridOccupancyMap` хранит занятые клетки Dictionary<Vector2Int, string> и проверяет свободные области под footprint здания (учитывая поворот).
- `BuildingFactory` создаёт BuildingView по данным каталога: назначает спрайт, масштабирует под размер клетки и footprint, применяет трансформ в мире.
- `PlacementController`:
   - двигает курсор по миру (мышь/клавиатура),
   - считает поворот,
   - обновляет курсор-превью,
   - блокирует подтверждение, если нажатие началось над UI или курсор над UI.
- `SaveLoadCoordinator`:
   - преобразует все живые экземпляры в записи (Id/позиция/поворот),
   - сохраняет/читает JSON через PlacedBuildingsFileRepository,
   - пересобирает мир от чистого состояния.
- `JsonBuildingCatalogProvider` загружает конфиг из StreamingAssets/buildings.json, при отсутствии — генерирует дефолт.

## ➕ Как добавить новое здание
1. Положить спрайт в Resources/Sprites/Buildings/YourSpriteName.png.
2. Добавить запись в StreamingAssets/buildings.json:
```json
{
  "Id": "office", 
  "DisplayName": "Office", 
  "GridWidth": 3, 
  "GridHeight": 6, 
  "SpriteResourcePath": "Sprites/Buildings/YourSpriteName"
}
```
3. Запустить сцену — кнопка с иконкой появится автоматически в каталоге.

## 💾 Где лежат сохранения
- Файл: `placed_buildings.json`
- Путь: `Application.persistentDataPath`.
- Формат:
```json
{
  "Placed":
  [
    {
      "BuildingId":"house_small",
      "GridX":0,
      "GridY":0,
      "RotationDegrees":0
    }
  ]
}
```
## 🔎 Процедура проверки кнопок
1. `Export/Import`
   - Выбери здание → Place → поставь 2–3 объекта.
   - Нажми Export — в поле появится JSON. Скопируй.
   - Удали объекты (режим Delete) → Import (вставь JSON) → объекты восстановились на тех же клетках.
2. `Save/Load`
   - Поставь объекты, Save.
   - Удали всё (Delete), Load → сцена восстановилась.
   - Перезапусти Play Mode → Load ещё раз.

## 📦 Assembly Definitions (модули)
- Project.Domain.Buildings, Project.Domain.Grid
- Project.Application.Placement, Project.Application.Persistence
- Project.Infrastructure.Config, Project.Infrastructure.Input, Project.Infrastructure.Persistence
- Project.Presentation.UI, Project.Presentation.Views
- Project.Installers