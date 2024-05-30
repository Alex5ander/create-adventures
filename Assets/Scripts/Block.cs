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
        if (!item.solid)
        {
            GetComponent<BoxCollider2D>().isTrigger = !item.solid;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.selectedBlock == this)
        {
            Item item = gameState.inventory.GetByIndex(gameState.hotBarSelectedIndex);
            if (item != null)
            {
                spriteRenderer.color -= new Color(item.damage * Time.deltaTime, item.damage * Time.deltaTime, item.damage * Time.deltaTime, 0);
                if (spriteRenderer.color.maxColorComponent <= 0)
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

    public void OnTriggerEnter2D(Collider2D _)
    {
        Color color = spriteRenderer.color;
        color.a = 0.75f;
        spriteRenderer.color = color;
    }

    public void OnTriggerExity2D(Collider2D _)
    {
        spriteRenderer.color = Color.white;
    }

    public void CreateDrop()
    {
        Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
        drop.Set(item);
    }
}