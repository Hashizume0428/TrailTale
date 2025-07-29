using UnityEngine;
using EventLibrary;

namespace EventLibrary
{
    public abstract class Item : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public ItemType itemType;

        // アイテム使用時の効果を適用するメソッド
        public abstract void Use( /* Player player */ );
    }
}
