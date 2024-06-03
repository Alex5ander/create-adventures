using System;

[Serializable]
public class Slot
{
    public Item item;
    public int amount;
    public Slot(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}
