using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Inventory : ScriptableObject
{
    public List<Slot> Slots = new();
    public event Action<int> OnChange;
    public bool Open;
    public void Add(Item item, int amount)
    {
        Slot availableSlot = Slots.Find(Slot => Slot.item == item);
        Slot emptySlot = Slots.Find(Slot => Slot.amount == 0);
        if (availableSlot != null)
        {
            availableSlot.amount += amount;
            Save();
            OnChange.Invoke(Slots.IndexOf(availableSlot));
        }
        else
        {
            emptySlot.item = item;
            emptySlot.amount = amount;
            Save();
            OnChange.Invoke(Slots.IndexOf(emptySlot));
        }
    }
    public void Remove(int index, int amount)
    {
        Slot availableSlot = Slots.ElementAt(index);
        if (availableSlot != null)
        {
            availableSlot.amount -= amount;
            if (availableSlot.amount == 0)
            {
                availableSlot.item = null;
            }
            Save();
            OnChange.Invoke(Slots.IndexOf(availableSlot));
        }
    }

    public void Swap(int old, int slot)
    {
        (Slots[slot], Slots[old]) = (Slots[old], Slots[slot]);
        Save();
        OnChange.Invoke(old);
        OnChange.Invoke(slot);
    }
    public void Save()
    {
        World world = SaveManger.Instance.saveGame.GetWorld();
        world.Slots = Slots;
    }
    public void Load()
    {
        Slots = SaveManger.Instance.saveGame.GetWorld().Slots;
    }
}
