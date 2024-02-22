using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
  public List<Sprite> sprites;
  public Sprite dropSprite;
  public float damage;
  public bool placeable;
  public bool solid;
}