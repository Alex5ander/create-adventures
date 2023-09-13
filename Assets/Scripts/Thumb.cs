using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Thumb : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] GameObject Parent;
    [SerializeField] UnityEvent<float, float> _OnDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float scale = Parent.GetComponent<RectTransform>().localScale.x;

        Vector2 parentSize = Parent.GetComponent<RectTransform>().sizeDelta * scale;

        Vector2 center = Parent.transform.position;

        Vector2 normal = (eventData.position - center).normalized;
        
        Vector2 thumbSize = GetComponent<RectTransform>().sizeDelta * scale;

        Vector2 size = (parentSize - thumbSize) / 4;

        transform.position = center + (normal * size);
        _OnDrag.Invoke(normal.x, normal.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = Parent.transform.position;
        _OnDrag.Invoke(0, 0);
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
