using UnityEngine;
using EventLibrary;

/// <summary>
/// StatusEventは、ステータスの変更を表すイベントデータです。
/// </summary>
[CreateAssetMenu(fileName = "StatusEvent", menuName = "Story/Event/StatusEvent")]
public class StatusEventData : EventData
{
    [System.Serializable]
    public class Option
    {
        public StatusType statusType;
        public StatusChange statusChange;
    }

    private const int MAX_OPTIONS = 4;

    [SerializeField, Header("最大4つの選択肢を設定可能")]
    private Option[] options;

    /// <summary>
    /// ステータスイベントの種類を取得します。
    /// </summary>
    /// <returns></returns>
    public override EventLibrary.EventType GetEventType()
    {
        return EventLibrary.EventType.Status;
    }

    /// <summary>
    /// 指定されたインデックスのオプションを取得します。
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Option GetOption(int index)
    {
        if (index < 0 || index >= options.Length)
        {
            Debug.LogError("Invalid option index: " + index);
            return default;
        }
        return options[index];
    }

    /// <summary>
    /// オプションの数を取得します。
    /// </summary>
    /// <returns></returns>
    public int GetOptionCount()
    {
        return options.Length;
    }

    /// <summary>
    /// 値が変更されたときに呼び出され、オプションの数が最大値を超えないように調整します。
    /// </summary>
    public void OnValidate()
    {
        if (options.Length > MAX_OPTIONS)
        {
            Debug.LogWarning("オプションの数が最大値を超えています。最大" + MAX_OPTIONS + "個まで設定可能です。");
            System.Array.Resize(ref options, MAX_OPTIONS);
        }
    }
}