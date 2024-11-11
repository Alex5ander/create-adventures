using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] GameObject SlotSelector;
    public Image image;
    public TextMeshProUGUI text;
    public event Action<SlotUI> OnItemBeginDrag, OnItemDrag, OnItemDrop, OnItemEndDrag;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnBeginDrag(PointerEventData eventData) => OnItemBeginDrag.Invoke(this);

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData) => OnItemDrop.Invoke(this);


    public void OnEndDrag(PointerEventData eventData) => OnItemEndDrag.Invoke(this);

    public void SetData(Sprite sprite, int amount)
    {
        image.gameObject.SetActive(amount > 0);
        text.enabled = image.gameObject.activeSelf;

        image.sprite = sprite;
        text.text = amount.ToString();
    }

    public void Select() => SlotSelector.SetActive(true);

    public void Deselect() => SlotSelector.SetActive(false);
}
