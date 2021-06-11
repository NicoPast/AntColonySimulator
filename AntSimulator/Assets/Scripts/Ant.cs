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

    public AntHill antHill;

    public Transform antHillTr;

    bool frontTick = true;

    public float collisionAngle;
    public float warningRadius;
    public float dangerRadius;
    public float warningRotStr;
    public float dangerRotStr;

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

        checkCollision();

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
            ph.GetComponent<Pheromone>().activatePheromone(t, pheromoneStr);
        }
    }

    void checkCollision()
    {
        RaycastHit rF, rL, rR;
        Physics.Raycast(transform.position, transform.right, out rF);
        Vector3 dL = (Quaternion.Euler(0, 0, -collisionAngle / 2) * transform.right).normalized;
        Physics.Raycast(transform.position, dL, out rL);
        Vector3 dR = (Quaternion.Euler(0, 0, collisionAngle / 2) * transform.right).normalized;
        Physics.Raycast(transform.position, dR, out rR);

        pintarRayo(rF);
        dir += (Vector2)calcularFrontal(rF, rR, rL);

        pintarRayo(rR);
        pintarRayo(rL);
        dir += (Vector2)calcularLaterales(rR, rL);
    }

    Vector3 calcularFrontal(RaycastHit front, RaycastHit derecha, RaycastHit izquierda)
    {
        Vector3 d = Vector3.zero;
        if (front.collider)
        {
            //si vamos a colisionar con un obstaculo
            //marcamos que hay peligro frontal para no tratar los posibles peligros laterales
            if (!frontTick)
                frontTick = true;

            //tratamos la direccion en la que debe avanzar el agente para evitar la colision frontal
            d = (Quaternion.Euler(0,0,collisionAngle) * (derecha.point - transform.position)).normalized;
            if (derecha.distance < izquierda.distance)
            {
                d = Quaternion.Euler(0, 0, 180) * d;
            }
            else if (Mathf.Abs(derecha.distance - izquierda.distance) < 0.1)
                d = Vector3.zero;
            d -= (front.point - transform.position).normalized;
            if (front.distance > warningRadius)
                d *= 0;
            else if (front.distance < warningRadius && front.distance > dangerRadius)
                d *= warningRotStr;
            else if (front.distance < dangerRadius)
                d *= dangerRotStr;

        }
        return d;
    }

    Vector3 calcularLaterales(RaycastHit derecha, RaycastHit izquierda)
    {
        RaycastHit rMax;
        Vector3 d = Vector3.zero;

        //si hay peligro de colision frontal, decidimos evitar los peligros a los laterales
        //tratamos la direccion en la que debe avanzar el agente para evitar las colisiones
        if (!frontTick)
        {
            d = (Quaternion.Euler(0, 0, collisionAngle) * (derecha.point - transform.position)).normalized;
            if (derecha.distance < izquierda.distance)
            {
                rMax = derecha;
                d = Quaternion.Euler(0, 0, 180) * d;
            }
            else
            {
                rMax = izquierda;
            }

            if (rMax.distance > warningRadius)
                return Vector3.zero;
            if (rMax.distance < warningRadius && rMax.distance > dangerRadius)
            {
                d *= warningRotStr;
            }
            else if (rMax.distance < dangerRadius)
            {
                d *= dangerRotStr;   // Reza a la Virgen
            }
        }
        return d;
    }

    void pintarRayo(RaycastHit r)
    {
        float distanciaSeg = r.distance < warningRadius ? r.distance : warningRadius;
        float distanciaPan = r.distance < dangerRadius ? r.distance : dangerRadius;

        Debug.DrawRay(transform.position, r.point - transform.position, Color.blue);
        Debug.DrawRay(transform.position, (r.point - transform.position).normalized * distanciaSeg, Color.yellow);
        Debug.DrawRay(transform.position, (r.point - transform.position).normalized * distanciaPan, Color.red);
    }

    public void setUpAnt(Transform phPool, Transform antHTr, AntHill antH)
    {
        pheromonePool = phPool;
        antHillTr = antHTr;
        antHill = antH;
    }
}