using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Inventory : MonoBehaviour
{

    int maxStack = 64;
    CanvasGroup group;
    [SerializeField] List<Slot> slots = new();
    [SerializeField] MouseFollower mouseFollower;
    [SerializeField] GameObject Hand;
    public int selected = 0;
    bool open = false;

    Dictionary<KeyCode, int> keys = new()
    {
        [KeyCode.Keypad1] = 0,
        [KeyCode.Keypad2] = 1,
        [KeyCode.Keypad3] = 2,
        [KeyCode.Keypad4] = 3,
        [KeyCode.Keypad5] = 4,
        [KeyCode.Keypad6] = 5,
        [KeyCode.Keypad7] = 6,
        [KeyCode.Keypad8] = 7,
        [KeyCode.Keypad9] = 8,
        [KeyCode.Alpha1] = 0,
        [KeyCode.Alpha2] = 1,
        [KeyCode.Alpha3] = 2,
        [KeyCode.Alpha4] = 3,
        [KeyCode.Alpha5] = 4,
        [KeyCode.Alpha6] = 5,
        [KeyCode.Alpha7] = 6,
        [KeyCode.Alpha8] = 7,
        [KeyCode.Alpha9] = 8,
    };

    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        foreach (Slot slot in slots)
        {
            {
                slot._OnBeginDrag = (Slot s) => mouseFollower.OnBeginDrag(s.GetItem());

                slot._OnEndDrag = () =>
                {
                    mouseFollower.OnEndDrag();
                    SelectItem(selected);
                };

                if(slots.IndexOf(slot) < 9)
                {
                    slot._OnPointerClick = (Slot s) => SelectItem(slots.IndexOf(s));
                }
            }
            slot.index = slots.IndexOf(slot);
        }

        foreach(Item item in MainScene.world.items)
        {
            Slot slot = slots[item.index];
            slot.SetItem(item);
        }

        SelectItem(MainScene.world.selected);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Toggle();
        }

        foreach (KeyCode key in keys.Keys)
        {
            if (Input.GetKeyDown(key))
            {
                SelectItem(keys[key]);
            }
        }
    }

    public bool IsOpen() => open;

    public void Toggle()
    {
        open = !open;
        group.interactable = open;
        group.alpha = open ? 1 : 0;
        group.blocksRaycasts = open;
    }

    void SelectItem(int index)
    {
        Slot oldSlot = slots[selected];
        oldSlot.DeSelect();
        selected = index;
        Slot currentSlot = slots[selected];
        currentSlot.Select();

        Item item = currentSlot.GetItem();
        SpriteRenderer sprite = Hand.GetComponent<SpriteRenderer>();
        sprite.sprite = item != null ? currentSlot.UIImage.sprite : null;
        Hand.transform.localScale = item != null && item.subtype == ItemSubType.BLOCK ? new(0.5f, 0.5f, 0.5f) : Vector3.one;

        OnChange();
    }

    public Item GetItem() => slots[selected].GetItem();

    public List<Item> GetItems() => slots.FindAll(slot => slot.GetItem() != null).Select(e => e.GetItem()).ToList();

    public bool Add(ItemType type, int amount = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Slot slot = slots[i];
            Item item = slot.GetItem();
            if (item != null && item.type == type && item.amount < maxStack)
            {
                slot.Add(amount);
                if (i == selected)
                {
                    SelectItem(i);
                }
                OnChange();
                return true;
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            Slot slot = slots[i];
            Item item = slot.GetItem();
            if (item == null)
            {
                slot.SetItem(new(i, type, amount));
                if (i == selected)
                {
                    SelectItem(i);
                }
                OnChange();
                return true;
            }
        }

        return false;
    }

    public bool Remove(ItemType type, int amount = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Slot slot = slots[i];
            Item item = slot.GetItem();
            if (item != null && item.type == type && item.amount >= amount)
            {
                slot.Remove(amount);
                if(i == selected)
                {
                    SelectItem(i);
                }
                OnChange();
                return true;
            }
        }

        return false;
    }

    public void OnChange()
    {
        MainScene.world.items = GetItems();
        MainScene.world.selected = selected;
        MainScene.Save();
    }
}