using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] Image UIImage;
    [SerializeField] TextMeshProUGUI UIText;
    [SerializeField] ObjectManager ObjectManager;
    CanvasGroup group;
    public Item item;
    [SerializeField] Texture2D cursorTexture;
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        Cursor.SetCursor(cursorTexture, new(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void OnBeginDrag(Item item)
    {
        this.item = item;
        group.alpha = 1;
        UIImage.sprite = ObjectManager.getItemSprite(item.type);
        UIText.text = item.amount.ToString();
    }

    public void OnEndDrag()
    {
        item = null;
        group.alpha = 0;
    }
}
