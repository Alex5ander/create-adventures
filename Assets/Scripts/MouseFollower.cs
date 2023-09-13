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

    // Update is called once per frame
    void Update()
    {
        transform.position = item != null ? Input.mousePosition : Camera.main.WorldToScreenPoint(Player.pointerPos);
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
