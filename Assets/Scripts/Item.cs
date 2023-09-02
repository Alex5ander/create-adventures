using System;
using System.Collections.Generic;
using System.Linq;

public enum ItemType
{
    NONE,
    DIRT,
    STONE,
    SAND,
    COAL,
    IRON,
    GOLD,
    DIAMOND,
    IRONAXE,
    IRONPICK,
    IRONSWORD,
    GOLDAXE,
    GOLDPICK,
    GOLDSWORD,
    DIAMONDAXE,
    DIAMONDPICK,
    DIAMONDSWORD,
    WOOD,
    LEAVES,
    WATER,
    CACTUS,
    SNOW
}

public enum ItemSubType
{
    BLOCK,
    TOOL
}

[Serializable]
public class Item
{
    public int index;
    public ItemType type;
    public ItemSubType subtype;
    public int amount = 1;

    static ItemType[] Tools =
    {
        ItemType.IRONAXE,
        ItemType.IRONPICK,
        ItemType.IRONSWORD,
        ItemType.GOLDAXE,
        ItemType.GOLDPICK,
        ItemType.GOLDSWORD,
        ItemType.DIAMONDAXE,
        ItemType.DIAMONDPICK,
        ItemType.DIAMONDSWORD
    };

    static Dictionary<ItemType, float> damages = new()
    {
        [ItemType.IRONSWORD] = 8,
        [ItemType.IRONPICK] = 7,
        [ItemType.IRONAXE] = 7,

        [ItemType.GOLDSWORD] = 10,
        [ItemType.GOLDPICK] = 9,
        [ItemType.GOLDAXE] = 9,

        [ItemType.DIAMONDSWORD] = 15,
        [ItemType.DIAMONDPICK] = 14,
        [ItemType.DIAMONDAXE] = 14
    };

    public Item(int index, ItemType type, int amount = 1)
    {
        this.index = index;
        this.type = type;
        this.amount = amount;
        subtype = Tools.Contains(type) ? ItemSubType.TOOL : ItemSubType.BLOCK;
        this.index = index;
    }

    public float GetDamage() => damages[type];
}