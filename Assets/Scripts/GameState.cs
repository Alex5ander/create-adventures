using UnityEngine;

[CreateAssetMenu]
public class GameState : ScriptableObject
{
    public int life = 10;
    public Vector3 position;
    public int inventorySelectedIndex = -1;
    public Block[,] blocks;
    public Inventory inventory;
}
