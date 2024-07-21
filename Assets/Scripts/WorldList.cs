using UnityEngine;
using UnityEngine.UI;

public class WorldList : MonoBehaviour
{
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] WorldButton worldButtonPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < SaveManger.Instance.worlds.Count; i++)
        {
            WorldButton worldButton = Instantiate(worldButtonPrefab);
            worldButton.Init(i);
            worldButton.transform.SetParent(verticalLayoutGroup.transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
