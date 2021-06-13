using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntHill : MonoBehaviour
{
    // food in the colony
    [SerializeField]
    float food = 0;

    // cantidad de comida para producir una hormiga
    [SerializeField]
    float antCost = 0;

    // numero de hormigas
    [SerializeField]
    uint numOfAnts;

    // radio de spawn alrededor del hormiguero
    [SerializeField]
    float spawnRadius = 0;

    // tope superior de hormigas en pantalla
    [SerializeField]
    bool limitMaxAnts = false;
    [SerializeField]
    int maxAnts = 100;
    
    // transform padres de las hormigas y las feromonas
    [SerializeField]
    Transform antsPool;
    [SerializeField]
    Transform pheromonePool;

    // prefab de la hormiga
    [SerializeField]
    GameObject antPref;

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

    // spawnea una hormiga si las condiciones lo permite
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

    // añade comida al hormiguero
    public void addFood(int num)
    {
        food += num;
        GameManager.instance().showFoodAntHill();
    }

    // spawnea una hormiga independientemente de las condiciones
    public void spawnAntForced()
    {
        if (limitMaxAnts && numOfAnts >= maxAnts)
            return;
        Vector3 pos = transform.position + (Vector3)Random.insideUnitCircle * Random.Range(0, spawnRadius);
        GameObject ant = Instantiate(antPref, pos, Random.rotation, antsPool);
        ant.GetComponent<Ant>().setUpAnt(pheromonePool, this);
        numOfAnts++;
        GameManager.instance().addAnt(1);
    }

    // activa o desactiva el tope de hormigas maximo
    public void changeLimitAnts()
    {
        limitMaxAnts = !limitMaxAnts;
    }

    public int getMaxAnts()
    {
        return maxAnts;
    }

    public void setMaxAnts(int max)
    {
        maxAnts = max;
    }

    public float getFood()
    {
        return food;
    }

    public uint getAnts()
    {
        return numOfAnts;
    }

    public float getAntCost()
    {
        return antCost;
    }

    public void setAntCost(float cost)
    {
        antCost = cost;
    }
}
