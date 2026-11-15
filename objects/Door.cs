using Godot;

// Дверь: разрушаемый объект (IDamageable) с анимациями открытия, удара и разрушения
[GlobalClass]
public partial class Door : Node3D, IDamageable
{
    // Название для подсказки в центре экрана
    [Export] public string ItemName { get; set; } = "Записка";
    // Здоровье двери
    [Export] public float health = 50f;
    // Проигрыватель анимаций Open, hit и Destroy
    [Export] private AnimationPlayer animationPlayer;

    // Состояния двери: открыта ли, разрушается ли
    private bool isOpen = false;
    private bool isDying = false;

    // По нажатию E открывает или закрывает дверь 
    public void Interact()
    {
        if (isDying || animationPlayer == null)
            return;

        if (isOpen)
            animationPlayer.PlayBackwards("Open");
        else
            animationPlayer.Play("Open");

        isOpen = !isOpen;
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

    // Смерть: проигрывает Destroy и удаляет дверь по окончании анимации
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

    // Когда доиграла анимация Destroy, удаляем дверь со сцены
    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "Destroy")
            QueueFree();
    }
}