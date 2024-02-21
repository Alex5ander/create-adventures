using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] GameState gameState;
    [SerializeField] SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Item item = gameState.inventory.GetByIndex(gameState.hotBarSelectedIndex);
        if (item != null)
        {
            transform.localScale = item.placeable ? Vector2.one / 2 : Vector2.one;
            spriteRenderer.sprite = item.dropSprite;
        }
        else
        {
            spriteRenderer.sprite = null;
        }
    }
}
