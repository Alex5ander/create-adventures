using System.Collections;
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
        if (Input.GetMouseButtonUp(0))
        {
            isMining = false;
            life = 100;
        }
    }
    bool isMining;
    public IEnumerator MiningCoroutine(float miningPower)
    {
        isMining = true;

        while (isMining)
        {
            life -= miningPower;
            if (life <= 0)
            {
                isMining = false;
                Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
                drop.Set(item);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Mining(float miningPower)
    {
        if (!isMining && life == 100)
        {
            StartCoroutine(MiningCoroutine(miningPower));
        }
    }
}