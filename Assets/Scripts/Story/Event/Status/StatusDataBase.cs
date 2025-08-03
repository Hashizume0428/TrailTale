using UnityEngine;
using EventLibrary;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StatusDataBase", menuName = "Status/StatusDataBase")]
public class StatusDataBase : ScriptableObject
{
    [SerializeField]
    private List<Status> statuses;

    public List<Status> GetStatuses()
    {
        return statuses;
    }

    public Status GetStatusByType(StatusType statusType)
    {
        return statuses.Find(status => status.statusType == statusType);
    }
}
