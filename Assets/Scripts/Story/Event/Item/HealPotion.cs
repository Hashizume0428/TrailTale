using UnityEngine;
using EventLibrary;

namespace EventLibrary
{
    [CreateAssetMenu(fileName = "HealPotion", menuName = "Items/HealPotion")]
    public class HealPotion : Item
    {
        public int healAmount;

        public override void Use(HeroController hero)
        {
            if (hero == null)
            {
                Debug.LogWarning("HeroController is null. Cannot apply heal effect.");
                return;
            }
            // 回復効果を適用する処理
            hero.Heal(healAmount);
        }
    }
}