using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour, ISaveManager
{
    public List<Slot> Slots;
    public UnityEvent _OnChange;
    public bool Open;
    public void Add(Item item, int amount)
    {
        Slot availableSlot = Slots.Find(Slot => Slot.item == item);
        Slot emptySlot = Slots.Find(Slot => Slot.amount == 0);
        if (availableSlot != null)
        {
            availableSlot.amount += amount;
            OnChange();
        }
        else
        {
            emptySlot.item = item;
            emptySlot.amount = amount;
            OnChange();
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
            OnChange();
        }
    }

    public Item GetByIndex(int index) => Slots[index].item;

    public void Swap(int old, int slot)
    {
        (Slots[slot], Slots[old]) = (Slots[old], Slots[slot]);
        OnChange();
    }
    public void OnChange()
    {
        _OnChange.Invoke();
        World world = SaveManger.saveGame.GetWorld();
        world.Slots = Slots;
    }
    public void Load(SaveGame saveGame)
    {
        Slots = saveGame.GetWorld().Slots;
        _OnChange.Invoke();
    }
}

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