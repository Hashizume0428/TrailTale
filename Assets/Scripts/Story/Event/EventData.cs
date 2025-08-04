using UnityEngine;
using EventLibrary;
using System;

public abstract class EventData : ScriptableObject
{
    public abstract EventLibrary.EventType GetEventType();
    
}