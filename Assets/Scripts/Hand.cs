using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] TerrainGenerator terrainGenerator;
    [SerializeField] Inventory inventory;
    [SerializeField] InventoryUI inventoryUI;
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
    float tt = 0;
    void StartMining(Texture2D texture)
    {
        if (!particles.gameObject.activeSelf)
        {
            tt = Time.time;
            particles.gameObject.SetActive(true);
            particlesRenderer.material.mainTexture = texture;
        }
    }

    public void Use(int x, int y)
    {
        Block block = terrainGenerator.GetBlock(x, y);
        Item item = inventory.Slots[inventoryUI.HotBarIndex].item;
        if (item && item.block && block == null)
        {
            inventory.Remove(inventoryUI.HotBarIndex, 1);
            terrainGenerator.CreateBlock(x, y, item.block, true);
        }
        else if (item && item.block == null && block)
        {
            particles.transform.position = new(x, y);
            StartMining(block.item.sprite.texture);
            if (Time.time - tt > 1.5f)
            {
                terrainGenerator.Remove(x, y, true);
                particles.gameObject.SetActive(false);
            }
        }
    }

    public void Use()
    {
        particles.gameObject.SetActive(false);
    }
}
