using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public enum PheromoneType
    {
        Idle = 0,
        Food,
        Danger
    };

    bool active = false;

    PheromoneType type = PheromoneType.Idle;
    public float strength = 0;

    public LayerMask idleLM;
    public LayerMask foodLM;
    public LayerMask dangerLM;

    float maxStr;
    float creationTime = 0;

    public bool destroyable = true;

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

    void normalizeLayerMask(ref LayerMask l)
    {
        int layerNumber = -1;
        int layer = l.value;
        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        l.value = layerNumber;
    }

    // Update is called once per frame
    void Update()
    {
        if (active && destroyable)
        {
            strength -= Time.deltaTime;
            Color col = c;
            col.a = (strength / maxStr) - 0.3f;
            render.material.color = col;
            if(strength <= 0)
            {
                GameManager.instance().addPheromonesMap(-1);
                Destroy(transform.gameObject);
            }
        }
    }

    public void activatePheromone(PheromoneType t, float str = 10)
    {
        type = t;
        creationTime = Time.time;
        strength = str;
        maxStr = str;
        active = true;
        if (type == PheromoneType.Idle)
        {
            normalizeLayerMask(ref idleLM);
            gameObject.layer = idleLM.value;
            c = Color.gray;
        }
        else if(type == PheromoneType.Food)
        {
            normalizeLayerMask(ref foodLM);
            gameObject.layer = foodLM.value;
            c = Color.blue;
        }
        else
        {
            normalizeLayerMask(ref dangerLM);
            gameObject.layer = dangerLM.value;
            c = Color.red;
        }
    }

    public float getStrength()
    {
        return strength;
    }
}
