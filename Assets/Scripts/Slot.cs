using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] Image UIBackground;
    [SerializeField] public Image UIImage;
    [SerializeField] TextMeshProUGUI UIText;
    [SerializeField] ObjectManager ObjectManager;
    CanvasGroup group;
    Item item;
    public int index = 0;

    public Action<Slot> _OnPointerClick;
    public Action<Slot> _OnBeginDrag;
    public Action _OnEndDrag;
    [SerializeField] Sprite sprite;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        UIBackground.sprite = sprite;
    }

    public void DeSelect()
    {
        UIBackground.sprite = null;
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
        UIImage.sprite = item != null ? ObjectManager.getItemSprite(item.type) : null;
        UIText.text = item != null ? item.amount.ToString() : "";
    }

    public void Add(int amount = 1)
    {
        item.amount += amount;
        UIText.text = item.amount.ToString();
        animator.SetTrigger("Change");
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
        animator.SetTrigger("Change");
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
