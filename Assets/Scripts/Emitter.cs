using UnityEngine;

public class Emitter : MonoBehaviour
{
    [SerializeField] GameState gameState;
    [SerializeField] ParticleSystem particles;
    [SerializeField] GameObject emitter;
    Renderer particlesRenderer;
    // Start is called before the first frame update
    void Start()
    {
        particlesRenderer = particles.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.selectedBlock != null)
        {
            emitter.transform.position = gameState.selectedBlock.transform.position;
            particlesRenderer.material.mainTexture = gameState.selectedBlock.item.dropSprite.texture;
        }
        Item item = gameState.inventory.GetByIndex(gameState.hotBarSelectedIndex);
        if (gameState.selectedBlock != null && item != null && !item.placeable && gameState.selectedBlock != null && !emitter.activeSelf)
        {
            emitter.SetActive(true);
        }
        else if (gameState.selectedBlock == null && emitter.activeSelf)
        {
            emitter.SetActive(false);
        }
    }
}
