using UnityEngine;
using EventLibrary;

namespace EventLibrary
{
    [CreateAssetMenu(fileName = "Status", menuName = "Status/Status")]
    public class Status : ScriptableObject
    {
        public Sprite icon;
        public StatusType statusType;
    }
}
