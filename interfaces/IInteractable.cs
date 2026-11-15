using Godot;
using System;

// Базовый интерфейс: всё, с чем игрок может взаимодействовать лучом из камеры
public interface IInteractable
{
    // Название объекта, показывается в центре экрана при наведении
    string ItemName { get; }

    // Действие по нажатию E
    void Interact(){}
}