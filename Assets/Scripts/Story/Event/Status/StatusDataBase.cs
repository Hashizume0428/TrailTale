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

    public Status GetRandomStatus()
    {
        if (statuses.Count == 0)
        {
            Debug.LogWarning("No statuses available in the database.");
            return null;
        }
        int randomIndex = Random.Range(0, statuses.Count);
        return statuses[randomIndex];
    }
}
