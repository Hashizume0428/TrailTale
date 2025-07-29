using UnityEngine;
using EventLibrary;

public abstract class EventData : ScriptableObject
{
    public abstract EventLibrary.EventType GetEventType();
}