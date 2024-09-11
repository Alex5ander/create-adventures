using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hotbar : MonoBehaviour
{
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] Inventory inventory;
    [SerializeField] SlotUI SlotUIPrefab;
    public List<SlotUI> SlotsUI = new();
    int index = 0;
    public UnityEvent<Sprite> OnChange;
    readonly Dictionary<KeyCode, int> keyCodes = new()
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        index = SaveManger.Instance.GetWorld().hotBarIndex;
        for (int i = 0; i < 9; i++)
        {
            Slot slot = inventory.Slots[i];
            Item item = slot.item;
            SlotUI slotUI = Instantiate(SlotUIPrefab);
            slotUI.transform.SetParent(transform, false);
            slotUI.SetData(item != null ? item.sprite : null, slot.amount);
            slotUI.OnItemBeginDrag += inventoryUI.OnItemBeginDrag;
            slotUI.OnItemDrop += inventoryUI.OnItemDrop;
            slotUI.OnItemEndDrag += inventoryUI.OnItemEndDrag;
            SlotsUI.Add(slotUI);
            if (index == i)
            {
                slotUI.Select();
                OnChange.Invoke(inventory.Slots[index].item.sprite);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode keyCode in keyCodes.Keys)
        {
            if (Input.GetKeyDown(keyCode))
            {
                World world = SaveManger.Instance.GetWorld();
                world.hotBarIndex = keyCodes[keyCode];

                SlotUI slotUI = SlotsUI[index];
                slotUI.Deselect();
                index = world.hotBarIndex;
                slotUI = SlotsUI[index];
                slotUI.Select();
                OnChange.Invoke(inventory.Slots[index].item.sprite);
            }
        }
    }
}
