﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : MonoBehaviour
{
    public enum PheromoneType
    {
        Home = 0,
        Food,
        Danger
    };

    bool active = false;

    PheromoneType type = PheromoneType.Home;
    public float strength = 0;

    public LayerMask idleLM;
    public LayerMask foodLM;
    public LayerMask dangerLM;

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

    void normalizeLayerMask(ref LayerMask l)
    {
        int layerNumber = 0;
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

    public void activatePheromone(PheromoneType t, float str = 10)
    {
        type = t;
        creationTime = Time.time;
        strength = str;
        maxStr = str;
        active = true;
        if (type == PheromoneType.Home)
        {
            normalizeLayerMask(ref idleLM);
            Debug.Log(idleLM.value);
            gameObject.layer = idleLM.value;
            c = Color.gray;
        }
        else if(type == PheromoneType.Food)
        {
            normalizeLayerMask(ref foodLM);
            Debug.Log(foodLM.value);
            gameObject.layer = foodLM.value;
            c = Color.blue;
        }
        else
        {
            normalizeLayerMask(ref dangerLM);
            Debug.Log(dangerLM.value);
            gameObject.layer = dangerLM.value;
            c = Color.red;
        }
    }

    public float getStrength()
    {
        return strength;
    }
}
