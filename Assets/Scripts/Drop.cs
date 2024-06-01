using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField] GameState gameState;
    Item item;
    public void Set(Item item)
    {
        this.item = item;
        GetComponent<SpriteRenderer>().sprite = item.sprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameState.inventory.Add(item, 1);
            Destroy(gameObject);
        }
    }
}
