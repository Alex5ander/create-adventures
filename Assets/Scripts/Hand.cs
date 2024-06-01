using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour, ISaveManager
{
    [SerializeField] Inventory inventory;
    [SerializeField] InvetoryUI invetoryUI;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Damageable damageable;
    Dictionary<KeyCode, int> keyCodes = new()
    {
        [KeyCode.Keypad1] = 0,
        [KeyCode.Keypad2] = 1,
        [KeyCode.Keypad3] = 2,
        [KeyCode.Keypad4] = 3,
        [KeyCode.Keypad5] = 4,
        [KeyCode.Keypad6] = 5,
        [KeyCode.Keypad7] = 6,
        [KeyCode.Keypad8] = 7,
        [KeyCode.Keypad9] = 8,
        [KeyCode.Alpha1] = 0,
        [KeyCode.Alpha2] = 1,
        [KeyCode.Alpha3] = 2,
        [KeyCode.Alpha4] = 3,
        [KeyCode.Alpha5] = 4,
        [KeyCode.Alpha6] = 5,
        [KeyCode.Alpha7] = 6,
        [KeyCode.Alpha8] = 7,
        [KeyCode.Alpha9] = 8,
    };
    public int selected = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode keyCode in keyCodes.Keys)
        {
            if (Input.GetKeyDown(keyCode))
            {
                UpdateHotBar(keyCodes[keyCode]);
            }
        }
    }

    void PointerDown(Vector3 position)
    {

    }

    void PointerUp()
    {

    }
    void HandleMouse()
    {
        if (Input.GetMouseButton(0))
        {
            PointerDown(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            PointerUp();
        }
    }

    void HandleTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.fingerId == MouseFollower.fingerId)
            {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    PointerDown(touch.position);
                }
                else
                {
                    PointerUp();
                }
            }
        }
    }

    public void UpdateHand()
    {

        Item item = inventory.GetByIndex(selected);
        if (item)
        {
            transform.localScale = item.block ? Vector2.one / 2 : Vector2.one;
            spriteRenderer.sprite = item.sprite;

            damageable.damage = item.damage;
            damageable.knockback = item.knockback;
        }
        else
        {
            damageable.damage = 1;
            damageable.knockback = 1;
            spriteRenderer.sprite = null;
        }
    }

    public void UpdateHotBar(int index)
    {
        World world = SaveManger.saveGame.GetWorld();
        world.hotBarSelectedIndex = selected;

        SlotUI slot = invetoryUI.SlotsUI[index];
        slot.Deselect();
        selected = index;
        slot = invetoryUI.SlotsUI[selected];
        slot.Select();

        UpdateHand();
    }

    public void Load(SaveGame saveGame)
    {
        World world = saveGame.GetWorld();
        UpdateHotBar(world.hotBarSelectedIndex);
    }
}
