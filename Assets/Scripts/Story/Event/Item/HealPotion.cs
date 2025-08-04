using UnityEngine;
using EventLibrary;

namespace EventLibrary
{
    [CreateAssetMenu(fileName = "HealPotion", menuName = "Items/HealPotion")]
    public class HealPotion : Item
    {
        public int healAmount;

        public override void Use(/* Player player */)
        {
            // 回復効果を適用する処理
            // player.Heal(healAmount);
        }
    }
}