namespace AILibrary
{
    using System;
    using UnityEngine;

    /// <summary>
    /// AIからのレスポンスをJSON形式でChatResponseに変換し、さらにResponseContentに変換します。
    /// </summary>
    public class ResponseParser : MonoBehaviour
    {
        public static ResponseContent ParseResponse(string json)
        {
            ChatResponse response = JsonUtility.FromJson<ChatResponse>(json);

            if (response.choices != null && response.choices.Length > 0)
            {
                string content = response.choices[0].message.tool_calls[0].function.arguments;
                Debug.Log("contentの中身: " + content);

                ResponseContent responseContent = JsonUtility.FromJson<ResponseContent>(content);
                return responseContent;
            }
            else
            {
                Debug.LogWarning("レスポンスのchoicesが空です");
                return null;
            }
        }
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
        public ToolCall[] tool_calls;
    }

    [Serializable]
    public class ToolCall
    {
        public string id;
        public string type;
        public ResponseFunction function; 
    }

    [Serializable]
    public class ResponseFunction
    {
        public string name;
        public string arguments;
    }

    [Serializable]
    public class Arguments
    {

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