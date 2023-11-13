using System;
using TMPro;
using UnityEngine;

public class WorldButton : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] public Action<World> _OnClick;

    public World world;

    public void OnWorldPressed() => _OnClick.Invoke(world);

    public void OnDeletePressed()
    {
        MainScene.DeleteWorld(world);
        Destroy(gameObject);
    }
}
