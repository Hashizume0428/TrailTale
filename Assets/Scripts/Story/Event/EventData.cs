using UnityEngine;

public abstract class EventData : ScriptableObject
{
    public enum EventType
    {
        Status,
        Item,
    }

    public abstract EventType GetEventType();
}