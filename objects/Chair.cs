using Godot;

// Мебель (ящик/стул): разрушаемый объект (IDamageable), E ничего не делает
[GlobalClass]
public partial class Chair : StaticBody3D, IDamageable
{
    // Название для подсказки в центре экрана
    [Export] public string ItemName { get; set; } = "Записка";
    // Здоровье объекта
    [Export] private float health = 10f;
    // Проигрыватель анимаций hit и Destroy
    [Export] private AnimationPlayer animationPlayer;

    // Разрушается ли объект сейчас
    private bool isDying = false;

    // По нажатию E ничего не происходит, только сообщение в консоль
    public void Interact()
    {
        if (isDying)
            return;

        GD.Print("Это стул");
    }

    // Получение урона: уменьшает здоровье, при нуле умирает, иначе проигрывает анимацию удара
    public void ReceiveDamage(float damage)
    {
        if (isDying)
            return;

        health -= damage;

        if (health <= 0)
            Die();
        else
            animationPlayer?.Play("hit");
    }

    // Смерть: проигрывает Destroy и удаляет объект по окончании анимации
    public void Die()
    {
        isDying = true;

        if (animationPlayer != null && animationPlayer.HasAnimation("Destroy"))
        {
            animationPlayer.Play("Destroy");
            animationPlayer.AnimationFinished += OnAnimationFinished;
        }
        else
        {
            QueueFree();
        }
    }

    // Когда доиграла анимация Destroy, удаляем объект со сцены
    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "Destroy")
            QueueFree();
    }
}