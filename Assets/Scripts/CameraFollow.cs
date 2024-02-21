using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameState gameState;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        transform.position = new(gameState.position.x, gameState.position.y, transform.position.z);
    }
}
