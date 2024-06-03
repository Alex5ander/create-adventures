using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] Inventory inventory;
    [SerializeField] Image background;
    public Image image;
    public TextMeshProUGUI text;
    [SerializeField] Sprite SelectedSprite;
    public event Action<SlotUI> OnItemBeginDrag, OnItemDrag, OnItemDrop, OnItemEndDrag;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnItemBeginDrag.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        OnItemDrop.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnItemEndDrag.Invoke(this);
    }

    public void SetData(Sprite sprite, int amount)
    {
        image.enabled = amount > 0;
        image.sprite = sprite;
        text.enabled = amount > 0;
        text.text = amount.ToString();
    }

    public void Select()
    {
        background.sprite = SelectedSprite;
    }

    public void Deselect()
    {
        background.sprite = null;
        background.color = new(1, 1, 1, 0.5f);
    }
}
