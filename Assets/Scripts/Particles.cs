using UnityEngine;

public class Particles : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    Renderer particlesRenderer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            gameObject.SetActive(false);
        }
    }

    public void Play(Vector2 position, Sprite sprite)
    {
        particlesRenderer = particles.GetComponent<Renderer>();
        transform.position = position;
        particlesRenderer.material.mainTexture = sprite.texture;
        gameObject.SetActive(true);
    }
}