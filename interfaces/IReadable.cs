using Godot;
using System;

// Читаемые объекты: показывают и скрывают интерфейс с текстом
public interface IReadable: IInteractable
{
    // Показать интерфейс с текстом
    void Open()
    {}
    // Закрыть интерфейс с текстом
    void Close()
    {}
}