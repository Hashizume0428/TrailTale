using UnityEngine;
using EventLibrary;

[CreateAssetMenu(fileName = "BattleEventData", menuName = "Story/Event/BattleEventData")]
public class BattleEventData : EventData
{
    public override EventLibrary.EventType GetEventType()
    {
        return EventLibrary.EventType.Battle;
    }

    public BattleType battleType; // 戦闘の種類
}