#if UNITY_EDITOR
using UnityEditor;
# endif
using UnityEngine;


[CreateAssetMenu(fileName = "MaterialItem", menuName = "Scriptable Objects/MaterialItem")]
public class MaterialItem : Item
{
  public Block block;
  public override void Use(int x, int y, Inventory inventory, TerrainGenerator terrain)
  {

  }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MaterialItem))]
class MaterialItemEditor : Editor
{
  public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
  {
    Item item = (Item)target;
    Texture2D texture = new(width, height);
    EditorUtility.CopySerialized(item.sprite.texture, texture);
    return texture;
  }

  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();
    Item item = (Item)target;
    EditorGUILayout.Space();
    Rect rect;
    int i = 0;
    rect = GUILayoutUtility.GetLastRect();
    EditorGUI.DrawTextureTransparent(new Rect(rect.size.x / 2 - 64, rect.position.y + i * 128, 128, 128), item.sprite.texture);
    EditorGUILayout.Space();
  }
}
#endif