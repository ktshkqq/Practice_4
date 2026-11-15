using Godot;
using System;

// Разрушаемые объекты: получают урон и умирают
public interface IDamageable: IInteractable
{
    // Получение урона, уменьшает здоровье
    void ReceiveDamage(float damage);
    // Смерть объекта, когда здоровье закончилось
    void Die();
}