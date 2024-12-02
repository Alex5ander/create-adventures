using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseFollower : MonoBehaviour
{
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
                // if (touch.fingerId != Thumb.fingerId && fingerId == -1 && touch.phase == TouchPhase.Began)
                // {
                //     fingerId = touch.fingerId;
                // }

                if (touch.fingerId == fingerId)
                {
                    transform.position = touch.position;
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        fingerId = -1;
                        break;
                    }
                }
            }
        }
        else
        {
            transform.position = Input.mousePosition;
        }
    }

    public void SetData(Sprite sprite, string text)
    {
        this.text.alpha = sprite ? 1 : 0;
        this.text.text = text;
        image.sprite = sprite ? sprite : defaultSprite;
    }
}