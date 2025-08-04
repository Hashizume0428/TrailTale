namespace EventLibrary
{
    public enum EventType
    {
        Status,
        Item,
        Battle,
    }

    public enum StatusType
    {
        ATK,
        DEF,
        HP,
        SPD,
        MP,
        None
    }

    public enum StatusChange
    {
        Up = 1,
        Down = -1,
        None = 0
    }

    public enum ItemType
    {
        HealPotion_S = 0,
        HealPotion_M = 1,
        HealPotion_L = 2,     
    }

    public enum BattleType
    {
        Normal = 0,
        Boss = 1,
    }
}