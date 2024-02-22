using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public List<Slot> Slots;
    public Action _OnChange;
    public bool Open;
    public void Add(int type, int amount)
    {
        Slot availableSlot = Slots.Find(Slot => Slot.type == type);
        Slot emptySlot = Slots.Find(Slot => Slot.amount == 0);
        if (availableSlot != null)
        {
            availableSlot.amount += amount;
            OnChange();
        }
        else
        {
            emptySlot.type = type;
            emptySlot.amount = amount;
            OnChange();
        }
    }
    public void Remove(int index, int amount)
    {
        Slot availableSlot = Slots.ElementAt(index);
        if (availableSlot.type != -1)
        {
            availableSlot.amount -= amount;
            if (availableSlot.amount == 0)
            {
                availableSlot.type = -1;
            }
            OnChange();
        }
    }

    public Item GetByIndex(int index)
    {
        if (Slots[index] != null && Slots[index].type != -1)
        {
            return GameManager.Instance.items.Find(e => e.type == Slots[index].type);
        }
        return null;
    }

    public void Swap(int old, int slot)
    {
        int oldType = Slots[old].type;
        int oldAmount = Slots[old].amount;
        int type = Slots[slot].type;
        int amount = Slots[slot].amount;

        Slots[old].type = type;
        Slots[old].amount = amount;
        Slots[slot].type = oldType;
        Slots[slot].amount = oldAmount;
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
    public int type;
    public int amount;
    public Slot(int type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
}