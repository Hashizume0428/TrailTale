using UnityEngine;
using AILibrary;

/// <summary>
/// AIからのレスポンスをJSON形式で解析してResponseContentに変換します。
/// </summary>
public class ResponseParser : MonoBehaviour
{
    public static ResponseContent ParseResponse(string json)
    {
        ChatResponse response = JsonUtility.FromJson<ChatResponse>(json);

        if (response.choices != null && response.choices.Length > 0)
        {
            string content = response.choices[0].message.content;
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
