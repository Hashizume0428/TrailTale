using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using AILibrary;

/// <summary>
/// OpenAI APIを使用して、AIからの応答を取得し、UIに表示するクラスです。
/// </summary>
public class AIManager : MonoBehaviour
{
    [SerializeField]
    private string OpenAIApiKey;

    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    /// <summary>
    /// OpenAI APIに送信するリクエストのJSONを作成します。
    /// </summary>
    /// <returns></returns>
    private string CreateRequestJson(string locationName, StatusEventData statusEventData)
    {
        string systemPrompt = Resources.Load<TextAsset>("Prompts/SystemPrompt").text;

        InputPrompt inputPromptData = InputPromptBuilder.Create(locationName, statusEventData);
        string _inputPrompt = JsonUtility.ToJson(inputPromptData);
        Debug.Log("_Input Prompt JSON: " + _inputPrompt);

        var request = new RequestData
        {
            model = "gpt-4o-mini",
            //model = "gpt-4o",
            response_format = new ResponseFormat { type = "json_object" },
            messages = new Messages[]
            {
                new Messages { role = "system", content = systemPrompt },
                new Messages { role = "user", content = _inputPrompt }
            },
            tool_choice = new ToolChoice
            {
                type = "function",
                function = new ToolChoiceFunction
                {
                    name = "generate_status_event"
                }
            },
            tools = new ToolRoot[]
            {
                new ToolRoot {
                    type = "function",
                    function = new ToolRootFunction
                    {
                        name = "generate_status_event",
                        description = "RPGのステータスイベントを生成する",
                        parameters = new Parameter
                        {
                            type = "object",
                            properties = new ContentProperty
                            {
                                stage = new Stage { type = "string" },
                                description = new Description { type = "string", minLength = 800 },
                                options = new StatusOption
                                {
                                    type = "array",
                                    minItems = 1,
                                    items = new Item
                                    {
                                        type = "object",
                                        properties = new StatusOptionProperty
                                        {
                                            title = new Title { type = "string" },
                                            result = new Result { type = "string" }
                                        },
                                        required = new string[] { "title", "result" }
                                    }
                                }
                            },
                            required = new string[] { "stage", "description", "options" }
                        }
                    }
                }
            },
            max_tokens = 4096
        };

        return JsonUtility.ToJson(request);
    }

    /// <summary>
    /// OpenAI APIにリクエストを送信するコルーチンです。
    /// レスポンスを受け取り、ResponseContentに変換してUIに表示します。
    /// </summary>
    /// <returns></returns>
    public async UniTask<ResponseContent> SendPrompt(string locationName, EventData eventData)
    {
        string jsonData;
        if (eventData.GetEventType() == EventData.EventType.Status)
        {
            jsonData = CreateRequestJson(locationName, (StatusEventData)eventData);
        }
        else if (eventData.GetEventType() == EventData.EventType.Item)
        {
            // TODO: ItemEventDataの処理を実装する
            Debug.LogWarning("ItemEventDataの処理は未実装です。");
            return null;
        }
        else
        {
            Debug.LogError("Unsupported event type: " + eventData.GetEventType());
            return null;
        }

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + OpenAIApiKey);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            Debug.Log("AIからのレスポンスを受信しました。");

            // レスポンスをResponseContentに変換
            var responseContent = ResponseParser.ParseResponse(request.downloadHandler.text);

            return responseContent;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

        return null;
    }
}

