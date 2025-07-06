namespace AILibrary
{
    using System;

    /// <summary>
    /// RequestData
    /// </summary>
    [Serializable]
    public class RequestData
    {
        public string model;
        public ResponseFormat response_format;
        public Messages[] messages;
        public int max_tokens;
    }

    [Serializable]
    public class ResponseFormat
    {
        public string type;
    }

    [Serializable]
    public class Messages
    {
        public string role;
        public string content;
    }

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

    /// <summary>
    /// ChatResponse
    /// </summary>
    [Serializable]
    public class ChatResponse
    {
        public Choice[] choices;
    }

    [Serializable]
    public class Choice
    {
        public Message message;
    }

    [Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    /// <summary>
    /// ResponseContent
    /// </summary>
    [Serializable]
    public class ResponseContent
    {
        public string stage;
        public string description;
        public ResponseOption[] options;
    }

    [Serializable]
    public class ResponseOption
    {
        public string title;
        public string result;
    }
}