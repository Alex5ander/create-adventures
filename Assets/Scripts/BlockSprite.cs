using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "Block")]
public class BlockSprite : ScriptableObject
{
    [SerializeField] public ItemType type;
    [SerializeField] public Sprite[] sprites;
    [SerializeField] public Sprite itemSprite;
}












