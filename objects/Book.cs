using Godot;

//Читаемая книга. Реализует только IReadable: её нельзя подобрать, только прочитать.
[GlobalClass]
public partial class Book : Area3D, IReadable
{
    //Текст книги. У каждого экземпляра на сцене свой.
    [Export(PropertyHint.MultilineText)] public string Text { get; set; }
    // Название для подсказки в центре экрана
    [Export] public string ItemName { get; set; } = "Книга";
    // Узлы интерфейса с текстом 
    [Export] private CanvasLayer canvasLayer;
    [Export] private RichTextLabel richTextLabel;

    // Открыта ли книга сейчас
    private bool isOpen = false;

    public override void _Ready()
    {
        // если в инспекторе не назначено, ищем узлы по пути в сцене Book
        canvasLayer ??= GetNode<CanvasLayer>("CanvasLayer");
        richTextLabel ??= GetNode<RichTextLabel>("CanvasLayer/RichTextLabel");

        // Подставляем текст и прячем интерфейс при старте
        richTextLabel.Text = Text;
        canvasLayer.Visible = false;
    }

    // По нажатию E переключает книгу: открыть или закрыть
    public void Interact()
    {
        if (isOpen)
            Close();
        else
            Open();
    }

    //Показывает интерфейс с текстом.
    public void Open()
    {
        isOpen = true;
        canvasLayer.Visible = true;
    }

    //Скрывает интерфейс с текстом.
    public void Close()
    {
        isOpen = false;
        canvasLayer.Visible = false;
    }
}