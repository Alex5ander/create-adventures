using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] Image UICrosshair;
    [SerializeField] Image UIImage;
    [SerializeField] TextMeshProUGUI UIText;
    [SerializeField] ObjectManager ObjectManager;
    Item item = null;
    // Start is called before the first frame update
    void Start()
    {

    }
    public static int fingerId = -1;
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
                }

                if (touch.fingerId == fingerId)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        fingerId = -1;
                    }
                    else
                    {
                        transform.position = new(touch.position.x, touch.position.y + Camera.main.scaledPixelHeight / 4f);
                    }
                }
            }
        }
        else
        {
            transform.position = Input.mousePosition;
        }
        UICrosshair.enabled = item == null;
    }

    public void OnBeginDrag(Item item)
    {
        this.item = item;
        UIImage.sprite = ObjectManager.getItemSprite(item.type);
        UIText.text = item.amount.ToString();

        UIImage.enabled = true;
        UIText.enabled = true;
    }

    public void OnEndDrag()
    {
        item = null;
        UIImage.enabled = false;
        UIText.enabled = false;
    }
}
