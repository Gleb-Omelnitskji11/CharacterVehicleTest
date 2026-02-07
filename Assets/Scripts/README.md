# Unity Moving Mechanics System

## Огляд

Ця система реалізує механіку руху персонажа та автомобіля з можливістю переключення між ними. Система використовує архітектурні патерни та Unity Input System для чистого та розширюваного коду.

## Архітектура

### Core Components
- **ServiceLocator** - DI контейнер для управління сервісами
- **GameManager** - Управління станами гри та переключенням між персонажем/автомобілем
- **InputController** - Централізована обробка вводу через Unity Input System
- **GameInitializer** - Ініціалізація всіх сервісів на старті гри

### Player System
- **PlayerMoving** - Рух персонажа з анімаціями (idle, run, sprint)
- **Player** - Основний компонент персонажа

### Car System
- **CarMoving** - Реалістична фізика автомобіля з WheelCollider
- **Car** - Основний компонент автомобіля

## Налаштування

### 1. Створення Player GameObject

1. Створіть пустий GameObject та назвіть його "Player"
2. Додайте компоненти:
   - `CharacterController`
   - `Animator` (з анімаціями idle, run, sprint)
   - `Player` (основний скрипт)
   - `PlayerMoving` (логіка руху)
   - `Rigidbody` (опційно, для фізики)

### 2. Створення Car GameObject

1. Створіть пустий GameObject та назвіть його "Car"
2. Додайте дочірні об'єкти для коліс:
   - `Wheel_FL` (переднє ліве)
   - `Wheel_FR` (переднє праве)
   - `Wheel_RL` (заднє ліве)
   - `Wheel_RR` (заднє праве)
3. Додайте компоненти до головного об'єкта:
   - `Rigidbody`
   - `Car` (основний скрипт)
   - `CarMoving` (логіка руху)
4. Додайте `WheelCollider` до кожного колеса
5. Встановіть тег "Car" для об'єкта

### 3. Налаштування Input System

Використовується існуючий `InputSystem_Actions.inputactions` з такими прив'язками:
- **WASD** - Рух
- **Миша** - Обертання камери
- **Shift** - Спринт
- **E** - Взаємодія (вхід/вихід з авто)
- **Пробіл** - Гальмо (в автомобілі)

### 4. Налаштування GameManager

1. Створіть GameObject "GameManager"
2. Додайте компонент `GameManager`
3. Налаштуйте параметри:
   - `Player Prefab` (опційно)
   - `Player Spawn Point`
   - `Player Camera` та `Car Camera`
   - `Interaction Prompt` (UI елемент)

### 5. Ініціалізація системи

1. Створіть GameObject "GameInitializer"
2. Додайте компонент `GameInitializer`
3. Переконайтеся що `_autoInitialize = true`

## Параметри PlayerMoving

- **Walk Speed**: 5f (швидкість ходьби)
- **Sprint Speed**: 8f (швидкість спринту)
- **Rotation Speed**: 10f (швидкість обертання)
- **Interaction Radius**: 3f (радіус взаємодії з авто)
- **Ground Mask**: LayerMask для землі
- **Car Layer**: LayerMask для автомобілів

## Параметри CarMoving

- **Max Speed**: 50f (максимальна швидкість)
- **Acceleration**: 15f (прискорення)
- **Brake Force**: 30f (сила гальмування)
- **Turn Sensitivity**: 2f (чутливість керма)
- **Suspension**: Налаштування підвіски для реалістичної фізики

## Анімації

Для персонажа потрібні такі параметри в Animator:
- `Speed` (float) - інтенсивність руху (0 = idle, 1 = run)
- `IsGrounded` (bool) - чи на землі персонаж

## Камера

Рекомендується налаштувати дві камери:
- **Player Camera** - слідує за персонажем від третьої особи
- **Car Camera** - слідує за автомобілем

## Приклади використання

```csharp
// Отримати доступ до сервісів
var gameManager = ServiceLocator.GetService<GameManager>();
var inputController = ServiceLocator.GetService<InputController>();

// Переключити режим керування
gameManager.SetControlMode(ControlMode.Car);

// Перевірити чи може персонаж взаємодіяти
if (gameManager.PlayerMoving.CanInteract)
{
    // Логіка взаємодії
}
```

## Розширення системи

Система легко розширюється:
- Додавання нових типів транспорту через створення нових Moving компонентів
- Додавання нових типів взаємодії через розширення InputController
- Інтеграція з іншими системами через Service Locator

## Вимоги

- Unity 2022.3 або вище
- Input System package 1.11.2
- Standard Unity physics та animation системи
