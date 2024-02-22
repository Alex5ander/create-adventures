using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] GameState gameState;
    [SerializeField] Drop DropPrefab;
    public Item item;
    public bool destroy;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<BoxCollider2D>().enabled = item.solid;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.selectedBlock == this)
        {
            Item item = gameState.inventory.GetByIndex(gameState.hotBarSelectedIndex);
            if (item != null)
            {
                spriteRenderer.color -= new Color(0, 0, 0, item.damage * Time.deltaTime);
                if (spriteRenderer.color.a <= 0)
                {
                    destroy = true;
                }
            }
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void CreateDrop()
    {
        Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
        drop.Set(item);
    }
}
