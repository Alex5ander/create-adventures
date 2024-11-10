using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    public int index = 0;
    [SerializeField] Transform hotbarTransform;
    [SerializeField] Inventory inventory;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] MouseFollower mouseFollower;
    [SerializeField] SlotUI SlotUIPrefab;
    public List<SlotUI> SlotsUI = new();
    public UnityEvent<Sprite> ChangeHandSprite;
    public int InventoryIndex;
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
    // Start is called before the first frame update
    void Start()
    {
        index = SaveManger.Instance.GetWorld().hotBarIndex;
        for (int i = 0; i < SlotsUI.Count; i++)
        {
            SlotUI slotUI = SlotsUI[i];
            Slot slot = inventory.Slots[i];
            Item item = slot.item;
            slotUI.SetData(item != null ? item.sprite : null, slot.amount);
            slotUI.OnItemBeginDrag += OnItemBeginDrag;
            slotUI.OnItemDrop += OnItemDrop;
            slotUI.OnItemEndDrag += OnItemEndDrag;
            if (index == i)
            {
                slotUI.Select();
                UpdateHandSprite();
            }
        }
        inventory.OnChange += OnChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            Toggle();
        }
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
                UpdateHandSprite();
            }
        }
    }

    public void Toggle()
    {
        canvasGroup.alpha = canvasGroup.alpha == 0 ? 1 : 0;
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
        inventory.Open = !inventory.Open;
    }

    public void OnChange(int index)
    {
        Slot slot = inventory.Slots[index];
        Item item = slot.item;
        SlotsUI[index].SetData(item ? item.sprite : null, slot.amount);
        if (index == this.index)
        {
            UpdateHandSprite();
        }
    }

    public void OnItemBeginDrag(SlotUI slotUI)
    {
        InventoryIndex = SlotsUI.IndexOf(slotUI);
        mouseFollower.SetData(slotUI.image.sprite, slotUI.text.text);
    }

    public void OnItemDrop(SlotUI slotUI)
    {
        inventory.Swap(InventoryIndex, SlotsUI.IndexOf(slotUI));
        mouseFollower.SetData(null, "");
    }

    public void OnItemEndDrag(SlotUI slotUI)
    {
        InventoryIndex = -1;
        mouseFollower.SetData(null, "");
    }

    public void UpdateHandSprite()
    {
        ChangeHandSprite.Invoke(inventory.Slots[index].item ? inventory.Slots[index].item.sprite : null);
    }
}
