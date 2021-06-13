using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    // radio de deteccion
    [SerializeField]
    float radius;

    //value of the sensor
    float value;

    // layerMasks que usara el sensor
    [SerializeField]
    LayerMask objPheromone;
    [SerializeField]
    LayerMask dangerPheromone;
    [SerializeField]
    LayerMask dangerSpot;

    // referencia a la hormiga padre
    [SerializeField]
    Ant ant;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // calcula el valor del sensor
    public float calculateSensorValue()
    {
        value = 0;

        // detecta las colisiones con un circulo en el radio y posicion del sensor
        Collider2D[] objPh = Physics2D.OverlapCircleAll(transform.position, radius, objPheromone);
        Collider2D[] dangerPh = Physics2D.OverlapCircleAll(transform.position, radius, dangerPheromone | dangerSpot);

        // si encuentra algun objeto lo gestiona
        if (objPh.Length > 0)
        {
            foreach(Collider2D ph in objPh)
            {
                value += ph.GetComponent<Pheromone>().getStrength();
            }
        }
        if (dangerPh.Length > 0)
        {
            foreach (Collider2D ph in dangerPh)
            {
                value -= ph.GetComponent<Pheromone>().getStrength();
            }
        }
        return value;
    }

    // cambia las pheromonas objetivo
    public void setObjectivePheromone(LayerMask obj)
    {
        objPheromone = obj;
    }
}
