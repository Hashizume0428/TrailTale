using UnityEngine;
using EventLibrary;

[CreateAssetMenu(fileName = "ItemEventData", menuName = "ScriptableObjects/Event/ItemEventData")]
public class ItemEventData : EventData
{
    public ItemType itemType;

    public override EventLibrary.EventType GetEventType()
    {
        return EventLibrary.EventType.Item;
    }
}