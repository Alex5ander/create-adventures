using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] InvetoryUI invetoryUI;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Damageable damageable;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetData(Item item)
    {
        if (item)
        {
            transform.localScale = item.block ? Vector2.one / 2 : Vector2.one;
            spriteRenderer.sprite = item.sprite;

            damageable.damage = item.damage;
            damageable.knockback = item.knockback;
        }
        else
        {
            damageable.damage = 1;
            damageable.knockback = 1;
            spriteRenderer.sprite = null;
        }
    }

    Block selectedBlock;
    public void Use(Block block)
    {
        Item item = inventory.Slots[invetoryUI.HotBarIndex].item;
        if (selectedBlock != block)
        {
            if (selectedBlock)
            {
                selectedBlock.EndMining();
                selectedBlock = null;
            }
            selectedBlock = block;
        }

        if (selectedBlock)
        {
            selectedBlock.Mining(!item ? 0 : item.miningPower);
        }
    }
}
