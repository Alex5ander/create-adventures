using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] MouseFollower mouseFollower;
    [SerializeField] SlotUI SlotUIPrefab;
    public List<SlotUI> SlotsUI = new();
    public int InventoryIndex;
    // Start is called before the first frame update
    void Start()
    {
        inventory.OnChange += OnChange;
        for (int i = 9; i < inventory.Slots.Count; i++)
        {
            Slot slot = inventory.Slots[i];
            Item item = slot.item;
            SlotUI slotUI = Instantiate(SlotUIPrefab);
            slotUI.transform.SetParent(transform, false);
            slotUI.SetData(item != null ? item.sprite : null, slot.amount);
            slotUI.OnItemBeginDrag += OnItemBeginDrag;
            slotUI.OnItemDrop += OnItemDrop;
            slotUI.OnItemEndDrag += OnItemEndDrag;
            SlotsUI.Add(slotUI);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            Toggle();
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
}
