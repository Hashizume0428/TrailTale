namespace AILibrary
{
    using System;
    using EventLibrary;

    /// <summary>
    /// OpenAI APIに送信するためのJSON形式のデータ構造です。
    /// </summary>
    [Serializable]
    public class StatusInputPrompt
    {
        public string location;
        public int eventDataCount;
        public StatusEvent[] eventData;
    }

    [Serializable]
    public class ItemInputPrompt
    {
        public string location;
        public int eventDataCount;
        public ItemEvent[] eventData;
    }

    [Serializable]
    public abstract class EventData
    {
        public EventLibrary.EventType eventType;
    }
    [Serializable]
    public class StatusEvent : EventData
    {
        public string status;
        public string change;

        public StatusEvent()
        {
            eventType = EventLibrary.EventType.Status;
        }
    }

    [Serializable]
    public class ItemEvent : EventData
    {
        public string itemName;

        public ItemEvent()
        {
            eventType = EventLibrary.EventType.Item;
        }
    }

    /// <summary>
    /// InputPromptを作成するためのビルダークラスです。
    /// </summary>
    public static class InputPromptBuilder
    {
        /// <summary>
        /// StatusEventバージョンのInputPromptを作成します。
        /// </summary>
        /// <param name="location"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public static StatusInputPrompt Create(string location, StatusEventData eventData)
        {
            var statusEvents = new StatusEvent[eventData.GetOptionCount()];
            for (int i = 0; i < eventData.GetOptionCount(); i++)
            {
                var option = eventData.GetOption(i);
                statusEvents[i] = new StatusEvent
                {
                    status = option.statusType.ToString(),
                    change = option.statusChange.ToString()
                };
            }

            return new StatusInputPrompt
            {
                location = location,
                eventDataCount = statusEvents.Length,
                eventData = statusEvents
            };
        }

        public static ItemInputPrompt Create(string location, ItemEventData eventData)
        {
            var itemEvents = new ItemEvent[eventData.GetOptionCount()];
            for (int i = 0; i < eventData.GetOptionCount(); i++)
            {
                var option = eventData.GetOption(i);
                itemEvents[i] = new ItemEvent
                {
                    itemName = option.itemType.ToString()
                };
            }

            return new ItemInputPrompt
            {
                location = location,
                eventDataCount = itemEvents.Length,
                eventData = itemEvents
            };
        }
    }
}