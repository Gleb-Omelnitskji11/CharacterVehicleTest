# Unity Moving Mechanics System

## Огляд

Цей модуль (`Assets/Scripts`) реалізує рух персонажа та автомобіля з можливістю перемикання керування, а також камеру від третьої особи та взаємодію (enter/exit vehicle).

Ключові залежності:
- Unity Input System (генерований клас `InputSystem_Actions`)
- Zenject (інʼєкція залежностей та installers)

## Структура папок

- `Camera/`
  - `ThirdPersonCamera` — камера від третьої особи. Перемикає offset/target в залежності від `ControlMode`.
- `Core/`
  - `MainSceneInstaller` — Zenject composition root для сцени (біндить `GameStateManager`, `IInputController`, `InteractManager`, `ThirdPersonCamera`).
  - `EventBusInstaller` — Zenject composition root для прожект контекста (біндить `EventBus`).
  - `GameStateManager` — менеджер для керування зміни (`ControlMode`).
  - `InteractManager` — викликає `CanInteract()` та `Interact()` на активних `InteractiveObject`.
  - `ControlMode` — enum режимів (`Character`, `Vehicle`).
  - `EventBus/` — простий event bus + `ControlModeChangedSignal`.
- `Input/`
  - `InputController` — реалізація `IInputController` через Unity Input System.
  - `IInputController` — контракт для читання дій гравця.
  - `BaseMovementController` — базовий абстрактний контролер руху.
- `Player/`
  - `CharacterMoving` — рух персонажа через `CharacterController` + анімації `Moving`/`Sprint`.
  - `PlayerHolder` — контейнер для посилань на компоненти персонажа (поточна реалізація мінімальна).
- `Vehicles/`
  - `InteractiveObject` — базовий тип для обʼєктів взаємодії.
  - `Car` — `InteractiveObject`, який виконує enter/exit та вмикає/вимикає `CarMoving`.
  - `CarMoving` — фізика авто на `WheelCollider` (torque/steering/brake/anti-roll).

## Як запустити

- Відкрий `Assets/Scenes/MainSceneScene.unity`.
- Переконайся, що в сцені налаштовано Zenject:
  - Є `EventBusInstaller` в префабі `ProjectContext` (біндить `IEventBus -> EventBus`).
  - Є `MainSceneInstaller` з посиланнями на обʼєкти сцени:
    - `InputController`
    - `InteractManager`
    - `ThirdPersonCamera`

## Керування (Input System)

Використовується `InputSystem_Actions` з такими діями:
- **WASD** — рух гравця та машини
- **Mouse** — огляд (камера) лише гравець
- **Shift** — sprint (персонаж) лише гравець
- **E** — interact (вхід/вихід з авто) дія залежить від `ControlMode`
- **Space** — brake (авто) лише авто

## Налаштування компонентів (коротко)

- `CharacterMoving`
  - Потрібні компоненти на персонажі: `CharacterController`, `Animator`.
  - Параметри анімацій: bool `Moving`, bool `Sprint`.
- `CarMoving`
  - Потрібні: `Rigidbody`, `WheelCollider` для кожного колеса + meshes для візуалізації.
  - Бажано щоб enable = false якщо гравець не в цій машині.
- `ThirdPersonCamera`
  - Має offsets для `Character`/`Vehicle`, `sensitivity` та параметри smoothing.

## Події та стан

- `GameStateManager` публікує `ControlModeChangedSignal` через `IEventBus` при зміні режиму.
- `ThirdPersonCamera` підписується на `ControlModeChangedSignal` і оновлює target/offset.

## Вимоги

- Unity 2022.3 або новіше
- Unity Input System (пакет проєкту NewInputSystem)
- Zenject (пакет/плагін у проєкті ExZenject без optional files)
