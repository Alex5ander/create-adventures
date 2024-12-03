using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseFollower : MonoBehaviour
{
    InputAction pointAction;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Sprite defaultSprite;
    static public int fingerId = -1;
    // Start is called before the first frame update
    void Start()
    {
        pointAction = InputSystem.actions.FindAction("UI/Point");
        pointAction.performed += Move;
    }

    void OnDisable()
    {
        pointAction.performed -= Move;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Move(InputAction.CallbackContext e)
    {
        transform.position = e.ReadValue<Vector2>();
    }

    public void SetData(Sprite sprite, string text)
    {
        this.text.alpha = sprite ? 1 : 0;
        this.text.text = text;
        image.sprite = sprite ? sprite : defaultSprite;
    }
}