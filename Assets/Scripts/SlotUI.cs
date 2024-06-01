using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] Image background;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameState gameState;
    [SerializeField] Sprite SelectedSprite;
    [SerializeField] Hand hand;
    int index;
    public void OnBeginDrag(PointerEventData eventData)
    {
        gameState.inventorySelectedIndex = index;
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        gameState.inventory.Swap(index, gameState.inventorySelectedIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        gameState.inventorySelectedIndex = -1;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Set(int index, Sprite sprite, int amount)
    {
        this.index = index;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (index < 9)
        {
            hand.UpdateHotBar(index);
        }
    }
}
