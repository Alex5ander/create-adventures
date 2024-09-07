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

    public virtual void Mining(float miningPower)
    {
        if (!particles.gameObject.activeSelf)
        {
            particles.gameObject.SetActive(true);
            particles.Play();
        }
        spriteRenderer.color -= new Color(miningPower * Time.deltaTime, miningPower * Time.deltaTime, miningPower * Time.deltaTime, 0);
        if (spriteRenderer.color.maxColorComponent <= 0)
        {
            CreateDrop();
        }
    }

    public virtual void EndMining()
    {
        spriteRenderer.color = Color.white;
        particles.gameObject.SetActive(false);
    }

    public virtual void CreateDrop()
    {
        Destroy(gameObject.transform.parent.gameObject);
        Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
        drop.Set(item);
    }
}