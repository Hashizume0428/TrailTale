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
        public ToolChoice tool_choice;
        public ToolRoot[] tools; // Optional: If you want to use tools
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
}