using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModifiedBlock
{
  public int x;
  public int y;
  public Item type;
  public int meta;

  public ModifiedBlock(Item type, int x, int y)
  {
    this.type = type;
    this.x = x;
    this.y = y;
  }
}

[Serializable]
public class World
{
  public int seed = 0;
  public List<Slot> Slots = new();
  public int hotBarSelectedIndex = 0;
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
public class SaveGame
{
  public int worldIndex = 0;
  public List<World> worlds = new();
  public World GetWorld() => worlds[worldIndex];
}