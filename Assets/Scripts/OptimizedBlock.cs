using UnityEngine;

public class OptimizedBlock : MonoBehaviour
{
    public Block block;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (block == null)
        {
            Destroy(gameObject);
        }
    }

    void OnBecameVisible()
    {
        block.gameObject.SetActive(true);
    }

    void OnBecameInvisible()
    {
        block.gameObject.SetActive(false);
    }
}
