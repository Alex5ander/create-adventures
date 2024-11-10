using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    public Item item;
    public void Set(Item item)
    {
        this.item = item;
        GetComponent<SpriteRenderer>().sprite = item.sprite;
    }
}
