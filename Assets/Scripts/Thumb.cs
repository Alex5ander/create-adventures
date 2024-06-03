using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Thumb : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] GameObject Parent;
    [SerializeField] UnityEvent<float, float> _OnDrag;
    public static int fingerId = -1;

    public void OnDrag(PointerEventData eventData)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.fingerId == fingerId && touch.phase == TouchPhase.Moved)
            {
                Vector2 parentSize = Parent.GetComponent<RectTransform>().sizeDelta / 2;
                Vector2 center = Parent.transform.position;
                Vector2 normal = (touch.position - center).normalized;
                Vector2 thumbSize = GetComponent<RectTransform>().sizeDelta;
                Vector2 size = parentSize - (thumbSize / 2);
                transform.position = center + (normal * size);
                _OnDrag.Invoke(normal.x, normal.y);
                break;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.fingerId != MouseFollower.fingerId && touch.phase == TouchPhase.Began)
            {
                fingerId = touch.fingerId;
                transform.position = touch.position;
                break;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.fingerId == fingerId && touch.phase == TouchPhase.Ended)
            {
                fingerId = -1;
                transform.position = Parent.transform.position;
                _OnDrag.Invoke(0, 0);
                break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Parent.SetActive(MainScene.isMobile);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
