using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameState gameState;
    void LateUpdate()
    {
        transform.position = new(gameState.position.x, gameState.position.y, transform.position.z);
    }
}
