using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    void LateUpdate()
    {
        transform.position = new(Mathf.Clamp(target.position.x, 13.75f, 305.25f), Mathf.Clamp(target.position.y, 7.5f, 100), transform.position.z);
    }
}
