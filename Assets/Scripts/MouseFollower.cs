using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] GameState gameState;
    [SerializeField] Inventory inventory;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Sprite defaultSprite;
    static public int fingerId = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (MainScene.isMobile)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.fingerId != Thumb.fingerId && touch.phase == TouchPhase.Began)
                {
                    fingerId = touch.fingerId;
                    transform.position = touch.position;
                    break;
                }
                if (touch.fingerId == fingerId && touch.phase == TouchPhase.Moved)
                {
                    transform.position = touch.position;
                    break;
                }
                if (touch.fingerId == fingerId && touch.phase == TouchPhase.Ended)
                {
                    fingerId = -1;
                    transform.position = touch.position;
                    break;
                }
            }
        }
        else
        {
            transform.position = Input.mousePosition;
        }
        if (inventory.Open && gameState.inventorySelectedIndex != -1)
        {
            Slot slot = inventory.Slots[gameState.inventorySelectedIndex];
            Item item = inventory.GetByIndex(gameState.inventorySelectedIndex);
            if (item != null && slot.amount > 0)
            {
                text.alpha = 1;
                text.text = slot.amount.ToString();
                image.sprite = item.dropSprite;
            }
        }
        else
        {
            text.alpha = 0;
            image.sprite = defaultSprite;
        }
    }
}
