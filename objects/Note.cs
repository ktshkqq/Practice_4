using Godot;

// Записка: читаемая (IReadable) и собираемая (ICollectible).
// После закрытия текста игрок сам подбирает её.
[GlobalClass]
public partial class Note : Area3D, IReadable, ICollectible
{
    // Название для подсказки в центре экрана
    [Export] public string ItemName { get; set; } = "Записка";
    // Текст записки, у каждого экземпляра может быть свой
    [Export] public string Text { get; set; }
    // Узлы интерфейса с текстом 
    [Export] private CanvasLayer canvasLayer;
    [Export] private RichTextLabel richTextLabel;

    // Открыта ли записка сейчас
    private bool isOpen = false;

    public override void _Ready()
    {
        // если в инспекторе не назначено, ищем узлы по пути в сцене Note
        canvasLayer ??= GetNode<CanvasLayer>("CanvasLayer");
        richTextLabel ??= GetNode<RichTextLabel>("CanvasLayer/RichTextLabel");

        // Подставляем текст и прячем интерфейс при старте
        richTextLabel.Text = Text;
        canvasLayer.Visible = false;
    }

    // По нажатию E переключает записку: открыть или закрыть
    public void Interact()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    // Показывает интерфейс с текстом
    public void Open()
    {
        isOpen = true;
        canvasLayer.Visible = true;
    }

    // Скрывает интерфейс с текстом 
    public void Close()
    {
        isOpen = false;
        canvasLayer.Visible = false;
    }

    // Последствия подбора: записка удаляется со сцены
    public void OnCollect()
    {
        QueueFree();
    }
}