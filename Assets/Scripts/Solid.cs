using UnityEngine;

public class Solid : Block
{
    [SerializeField] Drop DropPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Mining(float miningPower)
    {
        life -= miningPower;
        if (life <= 0)
        {
            Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
            drop.Set(item);
        }
    }
}