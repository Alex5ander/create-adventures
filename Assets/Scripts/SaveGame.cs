using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModifiedBlock
{
  public int x;
  public int y;
  public int type;
  public int meta;

  public ModifiedBlock(int type, int x, int y)
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
  public World(int seed)
  {
    this.seed = seed;
    for (int i = 0; i < 36; i++)
    {
      Slots.Add(new Slot(-1, 0));
    }
    Slots[0].type = 10;
    Slots[0].amount = 1;
  }
}

[Serializable]
public class SaveGame
{
  public int worldIndex = 0;
  public List<World> worlds = new();
  public World GetWorld() => worlds[worldIndex];
}