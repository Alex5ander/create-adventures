using UnityEngine;

[CreateAssetMenu]
public class GameState : ScriptableObject
{
    public Vector3 position;
    public int hotBarSelectedIndex;
    public int inventorySelectedIndex = -1;
    public Block selectedBlock;
    public Block[,] blocks;
    public Inventory inventory;
}
