using UnityEngine;
using System.Collections;
using LocationLibrary;

public class StoryManager : MonoBehaviour
{
    [SerializeField]
    private AIManager aiManager;

    [SerializeField]
    private NearByLocation nearByLocation;

    [SerializeField]
    private LatLng latLng = new LatLng
    {
        latitude = 34.6851f,
        longitude = 135.8048f
    }; // 和歌山大学の緯度経度


    private IEnumerator Start()
    {
        Debug.Log("近くのランドマークを検索しています...");
        yield return StartCoroutine(nearByLocation.SearchNearByLocation(latLng));
        Debug.Log("ランドマークの検索が完了しました。AIにプロンプトを送信します...");
        yield return StartCoroutine(aiManager.SendPromptCoroutine(nearByLocation.responseData.places[0].displayName.text));
        Debug.Log("AIからの応答を受信しました。");
    }
}