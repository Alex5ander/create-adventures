using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldButton : MonoBehaviour
{
    [SerializeField] Button WorldSelectButton;
    [SerializeField] Button WorldDeleteButton;
    public int index;
    public void Init(int index)
    {
        this.index = index;
        TextMeshProUGUI textMeshProUGUI = WorldSelectButton.GetComponentInChildren<TextMeshProUGUI>();
        textMeshProUGUI.text = "# World " + (index + 1);
        WorldSelectButton.onClick.AddListener(delegate { MainScene.SelectedWorld(index); });
        WorldDeleteButton.onClick.AddListener(delegate
        {
            MainScene.DeleteWorld(index);
            Destroy(gameObject);
        });
    }
}
