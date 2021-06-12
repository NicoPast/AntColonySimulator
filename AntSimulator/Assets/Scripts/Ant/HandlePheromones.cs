using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePheromones : MonoBehaviour
{
    public float pheromoneSpawnRate;
    float timer = 0;
    public float pheromoneStr;

    public GameObject pheromone;
    public Transform pheromonePool;

    public Sensor frontSensor;
    public Sensor leftSensor;
    public Sensor rightSensor;

    public float influenceStr;

    public LayerMask idleLM;
    public LayerMask foodLM;
    public LayerMask dangerLM;

    public Vector2 leftForcePhSensor;
    public Vector2 rightForcePhSensor;

    Pheromone.PheromoneType type = Pheromone.PheromoneType.Idle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LeavePheromones()
    {
        timer += Time.deltaTime;
        if (timer >= pheromoneSpawnRate)
        {
            timer = 0;
            GameObject ph = Instantiate(pheromone, transform.position, transform.rotation, pheromonePool);
            ph.GetComponent<Pheromone>().activatePheromone(type, pheromoneStr);
        }
    }

    public Vector2 detectPheromones()
    {
        Vector2 dir = Vector2.zero;
        float fs = frontSensor.calculateSensorValue();
        float ls = leftSensor.calculateSensorValue();
        float rs = rightSensor.calculateSensorValue();

        if (fs < 0 && ls < 0 && rs < 0)
            dir = (Vector2)transform.right * -1;
        else if (fs > Mathf.Max(ls, rs))
        {
            Debug.DrawRay(transform.position, transform.right, Color.red);
            dir = transform.right;
        }
        else if (ls > rs)
        {
            Debug.DrawRay(transform.position, transform.up, Color.red);
            dir = transform.up;
        }
        // it may be all 0
        else if (rs > ls)
        {
            Debug.DrawRay(transform.position, -1 * transform.up, Color.red);
            dir = -1 * transform.up;
        }

        return dir * influenceStr;
    }

    public void changePheromoneSpawn(Pheromone.PheromoneType t)
    {
        type = t;
        LayerMask lm = (t == Pheromone.PheromoneType.Idle) ? foodLM : idleLM;
        Debug.Log(lm.value);
        updateSensors(lm);
    }

    void updateSensors(LayerMask lm)
    {
        frontSensor.setObjectivePheromone(lm);
        leftSensor.setObjectivePheromone(lm);
        rightSensor.setObjectivePheromone(lm);
    }

    public void setUp(Transform phPool)
    {
        pheromonePool = phPool;
        updateSensors(foodLM);
    }
}
