namespace AILibrary
{
    using System;

    /// <summary>
    /// InputPrompt
    /// </summary>
    [Serializable]
    public class InputPrompt
    {
        public string location;
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
}