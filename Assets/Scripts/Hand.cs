using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetData(Item item)
    {
        print(item);
        if (item)
        {
            transform.localScale = item.block ? Vector2.one / 2 : Vector2.one;
            spriteRenderer.sprite = item.sprite;
        }
        else
        {
            spriteRenderer.sprite = null;
        }
    }
}
