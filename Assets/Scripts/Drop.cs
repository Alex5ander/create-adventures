using UnityEngine;

public class Drop : MonoBehaviour
{
    public ItemType type;
    [SerializeField] ObjectManager ObjectManager;

    public void SetItem(ItemType type)
    {
        this.type  = type;
        GetComponent<SpriteRenderer>().sprite = ObjectManager.getItemSprite(type);
    }
}
