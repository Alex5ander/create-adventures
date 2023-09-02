using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Color SelectedColor = new(1f, 1f, 1f);
    Color DeSelectedColor = new(1f, 1f, 1f, 0.5f);
    [SerializeField] Image UIBackground;
    [SerializeField] public Image UIImage;
    [SerializeField] TextMeshProUGUI UIText;
    [SerializeField] SpriteManager SpriteManager;
    CanvasGroup group;
    Item item;
    public int index = 0;

    public Action<Slot> _OnPointerClick;
    public Action<Slot> _OnBeginDrag;
    public Action _OnEndDrag;

    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        UIBackground.color = SelectedColor;
    }

    public void DeSelect()
    {
        UIBackground.color = DeSelectedColor;
    }

    public Item GetItem() => item;

    public void SetItem(Item item)
    {
        this.item = item;
        if(this.item != null)
        {
            this.item.index = index;
        }
        UIImage.enabled = item != null;
        UIText.enabled = item != null;
        UIImage.sprite = item != null ? SpriteManager.getItemSprite(item.type) : null;
        UIText.text = item != null ? item.amount.ToString() : "";
    }

    public void Add(int amount = 1)
    {
        item.amount += amount;
        UIText.text = item.amount.ToString();
    }

    public void Remove(int amount = 1) { 
        item.amount -= amount;
        if(item.amount == 0)
        {
            item = null;
            UIImage.enabled = false;
            UIText.enabled = false;
        }else
        {
            UIText.text = item.amount.ToString();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Slot slot = eventData.pointerDrag.GetComponent<Slot>();
        if(slot != null)
        {
            if(item == null)
            {
                SetItem(slot.item);
                slot.SetItem(null);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        group.alpha = 0.5f;
        if(item != null)
        {
            _OnBeginDrag?.Invoke(this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        group.alpha = 1;
        _OnEndDrag?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData) => _OnPointerClick?.Invoke(this);
}
