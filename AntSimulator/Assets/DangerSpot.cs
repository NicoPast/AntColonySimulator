using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerSpot : MonoBehaviour
{
    public float strength;

    Pheromone ph;

    // Start is called before the first frame update
    void Start()
    {
        ph = gameObject.GetComponent<Pheromone>();
        ph.activatePheromone(Pheromone.PheromoneType.Danger, strength);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
