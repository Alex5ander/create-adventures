using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameState gameState;
    void LateUpdate()
    {
        transform.position = new(Mathf.Clamp(gameState.position.x, 13.75f, 305.25f), Mathf.Clamp(gameState.position.y, 7.5f, 100), transform.position.z);
    }
}
