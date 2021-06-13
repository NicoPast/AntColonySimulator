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

    // espera a setUp para el ciclo de ejecucion
    bool active = false;

    // tipo y fuerza de la pheromona
    PheromoneType type = PheromoneType.Idle;
    [SerializeField]
    float strength = 0;

    // layerMasks de los distintos tipos de pheromona
    [SerializeField]
    LayerMask idleLM;
    [SerializeField]
    LayerMask foodLM;
    [SerializeField]
    LayerMask dangerLM;

    // variables para el calcuo de la fuerza
    float maxStr;
    float creationTime = 0;

    // se actualizara en cada frame?
    [SerializeField]
    bool destroyable = true;

    // datos para el renderizado
    MeshRenderer render;
    Color c;

    // Start is called before the first frame update
    void Start()
    {
        render = transform.gameObject.GetComponent<MeshRenderer>();
    }

    // normaliza la mascara a las potencias de 2
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

            // si se ha evaporado por completo destruyete
            if(strength <= 0)
            {
                GameManager.instance().addPheromonesMap(-1);
                Destroy(transform.gameObject);
            }
        }
    }

    // activa las pheromonas y les pone el valor adecuado
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
