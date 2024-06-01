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

    }

    public void Play(Vector2 position, Sprite sprite)
    {
        if (!gameObject.activeSelf)
        {
            particlesRenderer = particles.GetComponent<Renderer>();
            transform.position = position;
            particlesRenderer.material.mainTexture = sprite.texture;
            gameObject.SetActive(true);
        }
    }

    public void Stop()
    {
        gameObject.SetActive(false);
    }
}