using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using AILibrary;

/// <summary>
/// OpenAI APIを使用して、AIからの応答を取得し、UIに表示するクラスです。
/// </summary>
public class AIManager : MonoBehaviour
{
    [SerializeField]
    private string OpenAIApiKey;

    [SerializeField]
    private DisplayResponse displayResponse;

    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    // private string CreateInputPromptJson()
    // {
    //     var prompt = new InputPrompt
    //     {
    //         theme = "未来都市",
    //         location = "和歌山大学",
    //         summary = "",
    //         eventData = new StatusEvent
    //         {
    //             type = "status",
    //             options = new Option[]
    //             {
    //                 new Option { status = "攻撃力", value = 10 },
    //                 new Option { status = "防御力", value = 2 },
    //                 new Option { status = "体力", value = 5}
    //             }
    //         }
    //     };
    //     return JsonUtility.ToJson(prompt);
    // }

    /// <summary>
    /// OpenAI APIに送信するリクエストのJSONを作成します。
    /// </summary>
    /// <returns></returns>
    private string CreateRequestJson(string locationName)
    {
        string systemPrompt = Resources.Load<TextAsset>("Prompts/SystemPrompt").text;
        string inputPrompt = Resources.Load<TextAsset>("Prompts/InputPrompt").text;
        Debug.Log("Input Prompt JSON: " + inputPrompt);

        InputPrompt inputPromptData = new InputPrompt
        {
            location = locationName,
            eventData = new StatusEvent[]
            {
                new StatusEvent { status = "攻撃力", change = "up" },
                new StatusEvent { status = "防御力", change = "up" },
                new StatusEvent { status = "体力", change = "down" }
            }
        };

        string _inputPrompt = JsonUtility.ToJson(inputPromptData);
        Debug.Log("_Input Prompt JSON: " + _inputPrompt);

        var request = new RequestData
        {
            model = "gpt-4o-mini",
            response_format = new ResponseFormat { type = "json_object" },
            messages = new Messages[]
            {
                new Messages { role = "system", content = systemPrompt },
                new Messages { role = "user", content = inputPrompt }
            },
            max_tokens = 2048
        };

        return JsonUtility.ToJson(request);
    }

    /// <summary>
    /// OpenAI APIにリクエストを送信するコルーチンです。
    /// レスポンスを受け取り、ResponseContentに変換してUIに表示します。
    /// </summary>
    /// <returns></returns>
    public IEnumerator SendPromptCoroutine(string locationName)
    {
        string jsonData = CreateRequestJson(locationName);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + OpenAIApiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            Debug.Log("AIからのレスポンスを受信しました。");

            // レスポンスをResponseContentに変換
            var responseContent = ResponseParser.ParseResponse(request.downloadHandler.text);

            // ResponseContentをUIに表示
            displayResponse.DisplayResponseContent(responseContent);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}

