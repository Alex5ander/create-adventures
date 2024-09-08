using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] InvetoryUI invetoryUI;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Damageable damageable;
    [SerializeField] ParticleSystem particles;
    Renderer particlesRenderer;
    // Start is called before the first frame update
    void Start()
    {
        particlesRenderer = particles.GetComponent<Renderer>();
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
                particles.gameObject.SetActive(false);
                selectedBlock = null;
            }
        }

        if (selectedBlock == null && block)
        {
            selectedBlock = block;
            if (!particles.gameObject.activeSelf)
            {
                particles.transform.position = selectedBlock.transform.position;
                particles.gameObject.SetActive(true);
                particlesRenderer.material.mainTexture = selectedBlock.item.sprite.texture;
            }
        }

        if (selectedBlock)
        {
            float damage = !item ? 0 : item.miningPower;
        }
    }
}
