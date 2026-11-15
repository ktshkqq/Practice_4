using Godot;
using System;

// Расширения для узлов Godot
public static class NodeExtensions
{
    // Ищет вверх по дереву ближайшего родителя нужного типа (или интерфейса).
    // Нужен, когда луч попал в дочерний коллайдер, а скрипт висит на родителе.
    public static T GetParentOfType<T>(this Node node) where T: class
    {
        Node current = node.GetParent();
        while(current != null)
        {
            if (current is T typedParent)
            {
                return typedParent;
            }
            current = current.GetParent();
        }
        return null;
    }
}