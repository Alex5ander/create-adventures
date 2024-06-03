using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModifiedBlock
{
  public int x;
  public int y;
  public Block block;

  public ModifiedBlock(Block block, int x, int y)
  {
    this.block = block;
    this.x = x;
    this.y = y;
  }
}

[Serializable]
public class World
{
  public int seed = 0;
  public List<Slot> Slots = new();
  public int hotBarIndex = 0;
  public Vector2 playerPosition = new(20, 100);
  public Quaternion playerRotation = Quaternion.identity;
  public List<ModifiedBlock> modifiedBlocks = new();
  public World(int seed, Item[] items)
  {
    this.seed = seed;
    for (int i = 0; i < 36; i++)
    {
      if (i < items.Length)
      {
        Slots.Add(new Slot(items[i], 1));
      }
      else
      {
        Slots.Add(new Slot(null, 0));
      }
    }
  }
}

[Serializable]
[CreateAssetMenu]
public class SaveGame : ScriptableObject
{
  public int worldIndex = 0;
  public List<World> worlds = new();
  public World GetWorld() => worlds[worldIndex];
}