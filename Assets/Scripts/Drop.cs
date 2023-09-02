using UnityEngine;

public class Drop : MonoBehaviour
{
    public ItemType type;
    [SerializeField] SpriteManager spriteManager;

    public void SetItem(ItemType type)
    {
        this.type  = type;
        GetComponent<SpriteRenderer>().sprite = spriteManager.getItemSprite(type);
    }
}
