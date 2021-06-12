using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    enum AntState
    {
        Idle = 0,
        WithFood
    }

    public float maxSpeed = 2;
    public float rotSpeed = 2;
    public float wanderStr = 1;

    // to make it follow your mouse
    //Vector3 target;

    Vector2 dir;
    Vector2 vel;
    Vector2 pos;
    
    public LayerMask foodLayer;
    Transform targFood;
    public float viewRadius;
    public float viewAngle;

    AntState state = AntState.Idle;

    public float pheromoneSpawnRate;
    float timer = 0;
    public float pheromoneStr;

    public GameObject pheromone;
    public Transform pheromonePool;

    public Sensor frontSensor;
    public Sensor leftSensor;
    public Sensor rightSensor;

    public AntHill antHill;

    public Transform antHillTr;

    public Vector2 leftForcePhSensor;
    public Vector2 rightForcePhSensor;

    public CollisionDetector cD;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pointTo = transform.position + ((Quaternion.Euler(0, 0, -viewAngle/2) * transform.right).normalized * viewRadius);
        Debug.DrawLine(transform.position, pointTo, Color.black);
        pointTo = transform.position + ((Quaternion.Euler(0, 0, viewAngle / 2) * transform.right).normalized * viewRadius);
        Debug.DrawLine(transform.position, pointTo, Color.black);
        //Debug.DrawLine(transform.position, (transform.position + transform.forward).normalized * viewRadius, Color.green);
        //Debug.DrawLine(transform.position, (transform.position + transform.up).normalized * viewRadius, Color.red);
        //rotated = Quaternion.AngleAxis(viewAngle, transform.forward) * (transform.position + transform.right).normalized;
        //Debug.DrawLine(transform.position, rotated * viewRadius, Color.white);
        //Vector3 paintDir2 = transform.position + (transform.right * viewRadius);
        //paintDir2 = Quaternion.AngleAxis(viewAngle, Vector3.forward) * paintDir2;
        //Debug.DrawLine(transform.position, paintDir2, Color.red);

        //target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dir = ((Vector2)target - pos).normalized;
        SetDirection();

        dir += cD.checkCollision();

        detectPheromones();

        Vector2 vel2 = dir * maxSpeed;
        Vector2 rot = (vel2 - vel) * rotSpeed;
        Vector2 acc = Vector2.ClampMagnitude(rot, rotSpeed) / 1;

        vel = Vector2.ClampMagnitude(vel + acc * Time.deltaTime, maxSpeed);
        pos += vel * Time.deltaTime;

        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(pos, Quaternion.Euler(0, 0, angle));

        LeavePheromones();
    }

    void SetDirection()
    {
        if(state == AntState.Idle)
        {
            IdleDir();
        }
        else
        {
            dir = (dir + ((Vector2)antHillTr.position - pos)).normalized;

            const float foodDropRadius = 0.5f;
            if (Vector2.Distance(antHillTr.position, transform.position) < foodDropRadius)
            {
                // cambiar el estado de la hormiga
                state = AntState.Idle;
                antHill.addFood(1);
            }
        }
    }

    void IdleDir()
    {
        dir = (dir + Random.insideUnitCircle * wanderStr).normalized;

        if (!targFood)
        {
            Collider2D[] vision = Physics2D.OverlapCircleAll(pos, viewRadius, foodLayer);

            //Debug.Log(vision.Length);

            if (vision.Length > 0)
            {
                //Debug.Log("Encontre algo");

                Transform food = vision[Random.Range(0, vision.Length)].transform;
                Vector2 dirFood = (food.position - transform.position).normalized;

                if (Vector2.Angle(transform.right, dirFood) < viewAngle / 2)
                {
                    //Debug.Log("test");
                    targFood = food;
                }
                else
                {
                    //Debug.Log("No encontre nada");
                }
            }
        }
        else
        {
            //Debug.Log("Voy para mi comida");
            dir = (targFood.position - transform.position).normalized;

            const float foodPickUpRadius = 0.5f;
            if (Vector2.Distance(targFood.position, transform.position) < foodPickUpRadius)
            {
                // cambiar el estado de la hormiga
                Destroy(targFood.gameObject);
                state = AntState.WithFood;
                targFood = null;
            }
        }
    }

    void LeavePheromones()
    {
        timer += Time.deltaTime;
        if(timer >= pheromoneSpawnRate)
        {
            timer = 0;
            GameObject ph = Instantiate(pheromone, transform.position, transform.rotation, pheromonePool);
            Pheromone.PheromoneType t = (state == AntState.Idle) ? Pheromone.PheromoneType.Home : Pheromone.PheromoneType.Food;
            //Pheromone.PheromoneType t = Pheromone.PheromoneType.Danger;
            ph.GetComponent<Pheromone>().activatePheromone(t, pheromoneStr);
        }
    }

    void detectPheromones()
    {
        float fs = frontSensor.calculateSensorValue();
        float ls = leftSensor.calculateSensorValue();
        float rs = rightSensor.calculateSensorValue();

        if (fs < 0 && ls < 0 && rs < 0)
            dir += (Vector2)transform.right * -1;

        if(fs > Mathf.Max(ls, rs))
        {
            dir += (Vector2)transform.right;
        }
        else if(ls > rs)
        {
            dir = leftForcePhSensor.normalized;
        }
        // it may be all 0
        else if(rs > ls)
        {
            dir = rightForcePhSensor.normalized;
        }
        else
        {
            Debug.Log("No pheromones detected");
        }
    }

    public void setUpAnt(Transform phPool, Transform antHTr, AntHill antH)
    {
        pheromonePool = phPool;
        antHillTr = antHTr;
        antHill = antH;
        //frontSensor.setObjectivePheromone();
        //frontSensor.setObjectivePheromone();
        //frontSensor.setObjectivePheromone();
    }
}