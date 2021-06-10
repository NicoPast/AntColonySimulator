using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public enum PheromoneType
    {
        Home = 0,
        Food
    };

    bool active = false;

    PheromoneType type = PheromoneType.Home;
    public float strength = 0;
    float maxStr;
    float creationTime = 0;

    MeshRenderer render;

    Color c;
    
    Pheromone(PheromoneType t, float str) {
        type = t;
        strength = str;
    }

    // Start is called before the first frame update
    void Start()
    {
        render = transform.gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            strength -= Time.deltaTime;
            Color col = c;
            col.a = (strength / maxStr) - 0.3f;
            render.material.color = col;
            if(strength <= 0)
            {
                Destroy(transform.gameObject);
            }
        }
    }

    public void activatePheromone(PheromoneType t, float str = 1)
    {
        type = t;
        creationTime = Time.time;
        strength = str;
        maxStr = str;
        active = true;
        if (type == PheromoneType.Home)
        {
            c = Color.gray;
        }
        else
        {
            c = Color.blue;
        }
    }
}
