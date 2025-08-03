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
    private ApiKeyData apiKeyData;

    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    /// <summary>
    /// OpenAI APIに送信するリクエストのJSONを作成します。
    /// </summary>
    /// <returns></returns>
    private string CreateRequestJson(string locationName, EventData eventData)
    {
        string systemPrompt;
        string inputPrompt;

        if (eventData.GetEventType() == EventLibrary.EventType.Status)
        {
            systemPrompt = Resources.Load<TextAsset>("Prompts/StatusEventSystemPrompt").text;
            StatusEventData statusEventData = (StatusEventData)eventData;
            StatusInputPrompt inputPromptData = InputPromptBuilder.Create(locationName, statusEventData);
            inputPrompt = JsonUtility.ToJson(inputPromptData);
        }
        else if (eventData.GetEventType() == EventLibrary.EventType.Item)
        {
            systemPrompt = Resources.Load<TextAsset>("Prompts/ItemEventSystemPrompt").text;
            ItemEventData itemEventData = (ItemEventData)eventData;
            ItemInputPrompt inputPromptData = InputPromptBuilder.Create(locationName, itemEventData);
            inputPrompt = JsonUtility.ToJson(inputPromptData);
        }
        else
        {
            Debug.LogError("Unsupported event type: " + eventData.GetEventType());
            return null;
        }

        Debug.Log("Input Prompt JSON: " + inputPrompt);

        var request = new RequestData
        {
            model = "gpt-4o-mini",
            //model = "gpt-4o",
            response_format = new ResponseFormat { type = "json_object" },
            messages = new Messages[]
            {
                new Messages { role = "system", content = systemPrompt },
                new Messages { role = "user", content = inputPrompt }
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
                        description = "RPGのイベントを生成する",
                        parameters = new Parameter
                        {
                            type = "object",
                            properties = new ContentProperty
                            {
                                stage = new Stage { type = "string" },
                                description = new Description { type = "string", minLength = 800 },
                                options = new Option
                                {
                                    type = "array",
                                    minItems = 1,
                                    items = new Item
                                    {
                                        type = "object",
                                        properties = new OptionProperty
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
        int optionCount;
        if (eventData.GetEventType() == EventLibrary.EventType.Status)
        {
            StatusEventData statusEventData = (StatusEventData)eventData;

            jsonData = CreateRequestJson(locationName, statusEventData);
            optionCount = statusEventData.GetOptionCount();
        }
        else if (eventData.GetEventType() == EventLibrary.EventType.Item)
        {
            ItemEventData itemEventData = (ItemEventData)eventData;
            jsonData = CreateRequestJson(locationName, itemEventData);
            optionCount = itemEventData.GetOptionCount();
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
        request.SetRequestHeader("Authorization", "Bearer " + apiKeyData.APIKey);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            Debug.Log("AIからのレスポンスを受信しました。");

            // レスポンスをResponseContentに変換
            var responseContent = ResponseParser.ParseResponse(request.downloadHandler.text);

            if (responseContent.options.Length > optionCount)
            {
                Debug.LogWarning("AIからのオプション数が設定された最大値を超えています。オプションの数を調整します。");
                // オプション数を最大値に調整
                System.Array.Resize(ref responseContent.options, optionCount);
            }

            return responseContent;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }

        return null;
    }
}

