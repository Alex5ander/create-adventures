using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ObjectManager", menuName = "ObjectManager")]
public class ObjectManager : ScriptableObject
{
    [SerializeField] List<BlockSprite> blocks = new();

    public Sprite getItemSprite(ItemType type) => blocks.Find(block => block.type == type).itemSprite;

    public Sprite getSprite(ItemType type, int meta = 0) => blocks.Find(block => block.type == type).sprites[meta];
}
