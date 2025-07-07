namespace AILibrary
{
    using System;

    /// <summary>
    /// OpenAI APIに送信するためのJSON形式のデータ構造です。
    /// </summary>
    [Serializable]
    public class InputPrompt
    {
        public string location;
        public int eventDataCount;
        public StatusEvent[] eventData;
    }
    [Serializable]
    public class StatusEvent
    {
        public string status;
        public string change;
    }

    [Serializable]
    public class ItemEvent
    {
        public string effect;
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
        public static InputPrompt Create(string location, StatusEventData eventData)
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

            return new InputPrompt
            {
                location = location,
                eventDataCount = statusEvents.Length,
                eventData = statusEvents
            };
        }
    }
}