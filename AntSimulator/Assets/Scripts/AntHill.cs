using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntHill : MonoBehaviour
{
    public float food = 0;

    public float antCost = 0;
    public uint numOfAnts;

    public float spawnRadius = 0;

    public bool limitMaxAnts = false;
    public int maxAnts = 100;

    public Transform antsPool;
    public Transform pheromonePool;

    public GameObject antPref;

    private void OnDrawGizmosSelected()
    {
        Color c = Color.green;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance().showFoodAntHill();
    }

    // Update is called once per frame
    void Update()
    {
        spawnAnt();
    }

    void spawnAnt()
    {
        if(food >= antCost)
        {
            if (limitMaxAnts && numOfAnts >= maxAnts)
                return;
            food -= antCost;
            GameManager.instance().showFoodAntHill();
            spawnAntForced();
        }
    }

    public void addFood(int num)
    {
        food += num;
        GameManager.instance().showFoodAntHill();
    }

    public void spawnAntForced()
    {
        if (limitMaxAnts && numOfAnts >= maxAnts)
            return;
        Vector3 pos = transform.position + (Vector3)Random.insideUnitCircle * Random.Range(0, spawnRadius);
        GameObject ant = Instantiate(antPref, pos, Random.rotation, antsPool);
        ant.GetComponent<Ant>().setUpAnt(pheromonePool, transform, this);
        numOfAnts++;
        GameManager.instance().addAnt(1);
    }

    public void changeLimitAnts()
    {
        limitMaxAnts = !limitMaxAnts;
    }
}
