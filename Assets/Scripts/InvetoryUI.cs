using System.Collections.Generic;
using UnityEngine;

public class InvetoryUI : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] MouseFollower mouseFollower;
    [SerializeField] Transform hotBarTransform;
    [SerializeField] Hand hand;
    [SerializeField] SlotUI SlotUIPrefab;
    public List<SlotUI> SlotsUI = new();
    public int InventoryIndex;
    public int HotBarIndex;
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
        inventory.Load();
        inventory.OnChange += OnChange;
        HotBarIndex = SaveManger.Instance.saveGame.GetWorld().hotBarIndex;
        for (int i = 0; i < inventory.Slots.Count; i++)
        {
            Slot slot = inventory.Slots[i];
            Item item = slot.item;
            SlotUI slotUI = Instantiate(SlotUIPrefab);
            Transform parent = transform;
            if (i < 9)
            {
                parent = hotBarTransform;
            }
            slotUI.transform.SetParent(parent, false);
            slotUI.SetData(item != null ? item.sprite : null, slot.amount);
            slotUI.OnItemBeginDrag += OnItemBeginDrag;
            slotUI.OnItemDrop += OnItemDrop;
            slotUI.OnItemEndDrag += OnItemEndDrag;
            SlotsUI.Add(slotUI);
        }
        SlotsUI[HotBarIndex].Select();
        hand.SetData(inventory.Slots[HotBarIndex].item);
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
                World world = SaveManger.Instance.saveGame.GetWorld();
                world.hotBarIndex = keyCodes[keyCode];

                SlotUI slotUI = SlotsUI[HotBarIndex];
                slotUI.Deselect();
                HotBarIndex = world.hotBarIndex;
                slotUI = SlotsUI[HotBarIndex];
                slotUI.Select();
                hand.SetData(inventory.Slots[HotBarIndex].item);
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

    void OnChange(int index)
    {
        Slot slot = inventory.Slots[index];
        Item item = slot.item;
        SlotsUI[index].SetData(item ? item.sprite : null, slot.amount);
        if (index == HotBarIndex)
        {
            hand.SetData(item);
        }
    }

    void OnItemBeginDrag(SlotUI slotUI)
    {
        InventoryIndex = SlotsUI.IndexOf(slotUI);
        mouseFollower.SetData(slotUI.image.sprite, slotUI.text.text);
    }

    void OnItemDrop(SlotUI slotUI)
    {
        inventory.Swap(InventoryIndex, SlotsUI.IndexOf(slotUI));
        mouseFollower.SetData(null, "");
    }

    void OnItemEndDrag(SlotUI slotUI)
    {
        InventoryIndex = -1;
        mouseFollower.SetData(null, "");
    }
}
