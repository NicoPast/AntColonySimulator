using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerSpot : MonoBehaviour
{
    // fuerza del punto
    [SerializeField]
    float strength;

    [SerializeField]
    LayerMask dangerSpot;

    // script de la pheromona
    Pheromone ph;

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

    // Start is called before the first frame update
    void Start()
    {
        ph = gameObject.GetComponent<Pheromone>();
        ph.activatePheromone(Pheromone.PheromoneType.Danger, strength);
        normalizeLayerMask(ref dangerSpot);
        gameObject.layer = dangerSpot.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
