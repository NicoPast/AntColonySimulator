using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public float radius;
    float value;

    public LayerMask objPheromone;
    public LayerMask dangerPheromone;

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

    public float calculateSensorValue()
    {
        value = 0;

        Collider2D[] objPh = Physics2D.OverlapCircleAll(transform.position, radius, objPheromone);
        Collider2D[] dangerPh = Physics2D.OverlapCircleAll(transform.position, radius, dangerPheromone);

        //Debug.Log(vision.Length);

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

    public void setObjectivePheromone(LayerMask obj)
    {
        objPheromone = obj;
    }
}
