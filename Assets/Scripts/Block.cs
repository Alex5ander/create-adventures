using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Drop DropPrefab;
    public Item item;
    public bool destroy;
    SpriteRenderer spriteRenderer;
    [SerializeField] ParticleSystem particles;
    Renderer particlesRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particlesRenderer = particles.GetComponent<Renderer>();
        particlesRenderer.material.mainTexture = spriteRenderer.sprite.texture;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void StartMining()
    {
        particles.gameObject.SetActive(true);
    }

    public virtual void Mining(float miningPower)
    {
        spriteRenderer.color -= new Color(miningPower * Time.deltaTime, miningPower * Time.deltaTime, miningPower * Time.deltaTime, 0);
        if (spriteRenderer.color.maxColorComponent <= 0)
        {
            destroy = true;
        }
    }

    public virtual void EndMining()
    {
        spriteRenderer.color = Color.white;
        particles.gameObject.SetActive(false);
    }

    public virtual void CreateDrop()
    {
        Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
        drop.Set(item);
    }
}