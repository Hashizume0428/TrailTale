using UnityEngine;
using EventLibrary;

public class EventSet : MonoBehaviour
{
    const int MAX_EVENT_COUNT = 10;

    [SerializeField]
    private StatusEventData[] statusEvents;

    [SerializeField]
    private ItemEventData[] itemEvents;

    [SerializeField]
    private BattleEventData[] battleEvents;

    public EventData GetEventData(int currentIndex, int maxIndex)
    {
        // 最大イベント数に到達した場合、バトルイベントを返す
        if (currentIndex == maxIndex)
        {
            return battleEvents[0]; // 通常戦闘イベントを返す
        }

        // 最大イベント数が5以下の場合、ステータスイベントまたはアイテムイベントを返す
        if (maxIndex <= 5)
        {
            return GetEventStatusOrItem(80); // 80%の確率でステータスイベントを返す
        }
        // 最大イベント数が6以上10以下の場合、半分の地点でバトルイベントを返す
        else if (maxIndex > 5 && maxIndex < MAX_EVENT_COUNT)
        {
            if (currentIndex == maxIndex / 2)
            {
                return battleEvents[0]; // 半分の地点でバトルイベントを返す
            }
            else
            {
                return GetEventStatusOrItem(80); // 80%の確率でステータスイベントを返す
            }
        }

        Debug.LogError("Invalid maxIndex: " + maxIndex);
        return null;
    }

    private StatusEventData GetRandomStatusEvent()
    {
        return statusEvents[Random.Range(0, statusEvents.Length)];
    }

    private ItemEventData GetRandomItemEvent()
    {
        return itemEvents[Random.Range(0, itemEvents.Length)];
    }

    // 確率に応じてステータスイベントまたはアイテムイベントを取得
    private EventData GetEventStatusOrItem(int probability)
    {
        int r = Random.Range(0, 100);

        if (r < probability)
        {
            return GetRandomStatusEvent();
        }
        else
        {
            return GetRandomItemEvent();
        }
    }
}