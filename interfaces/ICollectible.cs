using Godot;
using System;

// Собираемые объекты: описывает, что происходит с объектом после подбора
public interface ICollectible: IInteractable
{
    // Последствия подбора
    void OnCollect(){}
}