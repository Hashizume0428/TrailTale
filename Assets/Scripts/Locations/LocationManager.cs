using UnityEngine;
using System.Collections.Generic;
using LocationLibrary;
using Cysharp.Threading.Tasks;
using System.Linq;

public class LocationManager : MonoBehaviour
{
    public void Init()
    {
        // ログから緯度経度のリストを取得
        LocationLogReader locationLogReader = new LocationLogReader();
        string log = locationLogReader.Read();
        var latLngList = locationLogReader.ParseLogToLatLng(log);

        // イベントが発生する位置のプールを作成
        List<int> eventOccurredPool = Enumerable.Range(0, latLngList.Count).ToList();

        // イベントが発生する位置のリストを作成
        var eventOccurred = new List<int>();

        // TODO : イベントが発生する位置をランダムに選択するロジックを実装

    }
}