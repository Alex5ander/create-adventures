using System.Collections.Generic;
using UnityEditor;
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

[CustomEditor(typeof(Item))]
class ItemEditor : Editor
{
  public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
  {
    Item item = (Item)target;
    if (item == null || item.sprites.Count == 0)
    {
      return null;
    }
    Texture2D texture = new(width, height);
    EditorUtility.CopySerialized(item.sprites[0].texture, texture);
    return texture;
  }

  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();
    Item item = (Item)target;
    EditorGUILayout.Space();
    Rect rect;
    int i = 0;
    foreach (Sprite sprite in item.sprites)
    {
      rect = GUILayoutUtility.GetLastRect();
      EditorGUI.DrawTextureTransparent(new Rect(rect.size.x / 2 - 64, rect.position.y + i * 128, 128, 128), sprite.texture);
      EditorGUILayout.Space();
      i++;
    }
    rect = GUILayoutUtility.GetLastRect();
    EditorGUI.DrawTextureTransparent(new Rect(rect.size.x / 2 - 64, rect.position.y + i * 128, 128, 128), item.dropSprite.texture);
  }
}