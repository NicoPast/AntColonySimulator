using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePheromones : MonoBehaviour
{
    // timer para dejar las pheromonas
    [SerializeField]
    float pheromoneSpawnRate;
    float timer = 0;

    // fuerza de la feromona a spawnear
    [SerializeField]
    float pheromoneStr;

    // prefab y transform padre de las feromonas
    [SerializeField]
    GameObject pheromone;
    [SerializeField]
    Transform pheromonePool;

    // sensores que detectan las feromonas
    [SerializeField]
    Sensor frontSensor;
    [SerializeField]
    Sensor leftSensor;
    [SerializeField]
    Sensor rightSensor;

    // fuerza con la que influyen las feromonas a la direccion 
    [SerializeField]
    float influenceStr;

    // Pheromones Layer Mask
    [SerializeField]
    LayerMask idleLM;
    [SerializeField]
    LayerMask foodLM;
    [SerializeField]
    LayerMask dangerLM;

    // Dirección de giro en función del sensor
    [SerializeField]
    Vector2 leftForcePhSensor;
    [SerializeField]
    Vector2 rightForcePhSensor;

    // feromona que deja la hormiga
    Pheromone.PheromoneType type = Pheromone.PheromoneType.Idle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // deja un rastro de pheromonas cada spawnRate de tiempo
    public void LeavePheromones()
    {
        timer += Time.deltaTime;
        if (timer >= pheromoneSpawnRate)
        {
            timer = 0;
            GameObject ph = Instantiate(pheromone, transform.position, transform.rotation, pheromonePool);
            ph.GetComponent<Pheromone>().activatePheromone(type, pheromoneStr);
            GameManager.instance().addPheromonesMap(1);
        }
    }

    // Detecta las pheromonas con los escaners y se guia por el que mas influencia tenga
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
        // pueden ser todos 0
        else if (rs > ls)
        {
            Debug.DrawRay(transform.position, -1 * transform.up, Color.red);
            dir = -1 * transform.up;
        }

        return dir * influenceStr;
    }

    // cambia las pheromonas que spawnea la hormiga
    public void changePheromoneSpawn(Pheromone.PheromoneType t)
    {
        type = t;
        LayerMask lm = (t == Pheromone.PheromoneType.Idle) ? foodLM : idleLM;
        updateSensors(lm);
    }

    // actualiza el objetivo de los escaners
    void updateSensors(LayerMask lm)
    {
        frontSensor.setObjectivePheromone(lm);
        leftSensor.setObjectivePheromone(lm);
        rightSensor.setObjectivePheromone(lm);
    }

    // configuramos el detector de feromonas
    public void setUp(Transform phPool)
    {
        pheromonePool = phPool;
        updateSensors(foodLM);
    }
}
