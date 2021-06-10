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
            if(Vector2.Distance(targFood.position, transform.position) < foodPickUpRadius)
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
            ph.GetComponent<Pheromone>().activatePheromone(Pheromone.PheromoneType.Home, pheromoneStr);
        }
    }
}