using Godot;

// Игрок от первого лица: движение WASD, обзор мышью, взаимодействие лучом через интерфейсы
[GlobalClass]
public partial class Player : CharacterBody3D
{
    private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    // Скорость движения, чувствительность мыши и урон от атаки
    [Export] private float Speed = 5.0f;
    [Export] private float MouseSensitivity = 0.002f;
    [Export] private float AttackDamage = 10f;

    // Названия собранных предметов (строки, а не узлы: узел после QueueFree станет недействительным)
    [Export] private Godot.Collections.Array<string> Inventory = new Godot.Collections.Array<string>();

    // Элементы интерфейса, луч и камера
    private Label label;
    private Label itemsCountLabel;
    private RayCast3D rayCast;
    private Camera3D camera;

    // Объект под лучом и текущая цель взаимодействия
    private Node collider;
    private IInteractable interactable;
    // Заблокирован ли игрок (во время чтения)
    private bool IsLocked = false;

    public override void _Ready()
    {
        // Захватываем курсор мыши
        Input.MouseMode = Input.MouseModeEnum.Captured;

        // Находим нужные узлы один раз
        label = GetNode<Label>("CanvasLayer/CameraLabel");
        itemsCountLabel = GetNode<Label>("CanvasLayer/LabelItemsNumber");
        rayCast = GetNode<RayCast3D>("Camera3D/RayCast3D");
        camera = GetNode<Camera3D>("Camera3D");

        // Показываем начальное значение счётчика
        itemsCountLabel.Text = Inventory.Count.ToString();
    }

    // Физика: гравитация и движение по WASD
    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        velocity.Y -= gravity * (float)delta;

        // Направление движения относительно поворота игрока
        Vector2 input = Input.GetVector("left", "right", "forward", "backward");
        Vector3 movementDirection = Transform.Basis * new Vector3(input.X, 0, input.Y);
        velocity.X = movementDirection.X * Speed;
        velocity.Z = movementDirection.Z * Speed;

        // при чтении записки стоим на месте, но гравитация продолжает действовать
        if (IsLocked)
        {
            velocity.X = 0;
            velocity.Z = 0;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    // Каждый кадр: ищем объект под лучом и показываем его название
    public override void _Process(double delta)
    {
        // пока читаем записку, цель не меняем, чтобы E закрыл именно её
        if (IsLocked)
        {
            label.Text = "";
            return;
        }

        rayCast.ForceRaycastUpdate();
        collider = rayCast.GetCollider() as Node;

        // объект сам, либо его родитель (если луч попал в дочерний коллайдер)
        interactable = collider as IInteractable ?? collider?.GetParentOfType<IInteractable>();

        label.Text = interactable != null ? $"{interactable.ItemName} (E/ЛКМ)" : "";
    }

    // Обработка ввода: обзор мышью, выход, взаимодействие (E) и атака (ЛКМ)
    public override void _Input(InputEvent @event)
    {
        // Поворот игрока и камеры мышью (камера ограничена по вертикали)
        if (@event is InputEventMouseMotion mouseMotion && !IsLocked)
        {
            RotateY(-mouseMotion.Relative.X * MouseSensitivity);

            camera.RotateX(-mouseMotion.Relative.Y * MouseSensitivity);
            Vector3 cameraRotation = camera.Rotation;
            cameraRotation.X = Mathf.Clamp(cameraRotation.X, Mathf.DegToRad(-70), Mathf.DegToRad(70));
            camera.Rotation = cameraRotation;
        }

        // Выход из игры
        if (@event.IsActionPressed("quit"))
        {
            GetTree().Quit();
        }

        // Если цели нет, дальше делать нечего
        if (interactable == null)
            return;

        // Взаимодействие по E
        if (@event.IsActionPressed("interact"))
        {
            interactable.Interact();

            // Читаемый объект: блокируем игрока на время чтения
            if (interactable is IReadable)
            {
                IsLocked = !IsLocked;

                // записка подбирается автоматически после закрытия текста
                if (!IsLocked && interactable is ICollectible collectible)
                    CollectItem(collectible);
            }
            else if (interactable is ICollectible collectibleItem)
            {
                CollectItem(collectibleItem);   // предмет без чтения подбирается сразу
            }
        }

        // Атака по ЛКМ: наносим урон разрушаемым объектам
        if (@event.IsActionPressed("attack") && !IsLocked)
        {
            if (interactable is IDamageable d)
            {
                d.ReceiveDamage(AttackDamage);
            }
        }
    }

    // Добавляет предмет в инвентарь, обновляет счётчик и вызывает последствия подбора у цели.
    private void CollectItem(ICollectible collectible)
    {
        Inventory.Add(interactable.ItemName);               // берём до OnCollect(), потом узел удалится
        itemsCountLabel.Text = Inventory.Count.ToString();
        collectible.OnCollect();
    }
}