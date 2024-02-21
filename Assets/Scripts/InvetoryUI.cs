using System.Collections.Generic;
using UnityEngine;

public class InvetoryUI : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] List<SlotUI> SlotsUI;
    void Awake()
    {
        inventory._OnChange = OnChange;
    }
    // Start is called before the first frame update
    void Start()
    {

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

    void OnChange()
    {
        foreach (Slot slot in inventory.Slots)
        {
            int index = inventory.Slots.IndexOf(slot);
            Item item = inventory.GetByIndex(index);
            SlotsUI[index].Set(index, item != null ? item.dropSprite : null, slot.amount);
        }
    }
}
