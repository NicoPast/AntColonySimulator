using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public enum AntState
    {
        Idle = 0,
        WithFood,
        Danger
    }

    public float maxSpeed = 2;
    public float rotSpeed = 2;
    public float wanderStr = 1;

    public float viewRadius = 10;
    public float viewAngle = 60;

    // to make it follow your mouse
    //Vector3 target;

    Vector2 dir;
    Vector2 vel;
    Vector2 pos;
    
    Transform targFood;
    bool withFood = false;

    public AntState state = AntState.Idle;

    public AntHill antHill;

    public Transform antHillTr;

    public CollisionDetector collisionDetector;
    public GetFood getFood;
    public HandlePheromones handlePheromones;
    public GetDangerSpots getDangSpot;

    public bool turningAround = false;

    Vector2 lastPosA;
    Vector2 lastPosB;
    float timerLP;
    bool skipLastP = true;
    public float lastPosDist;
    public float timeToUpdateLastPos;

    float timerDan;
    public float timeToBeInDanger;

    const float foodDropRadius = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        changeState(AntState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        //target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dir = ((Vector2)target - pos).normalized;

        if (getDangSpot.getDangerSpot() != null)
        {
            changeState(AntState.Danger);
        }
        // para evitar remolinos
        if (state == AntState.Danger)
            timerDan += Time.deltaTime;
        timerLP += Time.deltaTime;
        if(timerLP > timeToUpdateLastPos)
        {
            timerLP = 0;
            lastPosB = lastPosA;
            lastPosA = transform.position;
            if (!skipLastP && Vector2.Distance(transform.position, lastPosB) < lastPosDist)
            {
                changeState(AntState.Danger);
            }
            skipLastP = false;
        }
        if (turningAround)
        {
            dir = transform.right * -10;
            Debug.DrawRay(transform.position, dir, Color.green);
            turningAround = false;
        }
        else
        {
            SetDirection();
        }

        Vector2 vel2 = dir * maxSpeed;
        Vector2 rot = (vel2 - vel) * rotSpeed;
        Vector2 acc = Vector2.ClampMagnitude(rot, rotSpeed) / 1;

        vel = Vector2.ClampMagnitude(vel + acc * Time.deltaTime, maxSpeed);
        pos += vel * Time.deltaTime;

        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(pos, Quaternion.Euler(0, 0, angle));

        handlePheromones.LeavePheromones();
    }

    void SetDirection()
    {
        if(state == AntState.Idle)
        {
            IdleDir();
        }
        else
        {
            dir = (dir + Random.insideUnitCircle * wanderStr).normalized;
            dir += handlePheromones.detectPheromones();
            dir += ((Vector2)antHillTr.position - pos).normalized * 0.5f;
            if (Vector2.Distance(transform.position, antHillTr.position) < viewRadius && Vector2.Angle(transform.right, antHillTr.position - transform.position) < viewAngle / 2)
                dir = ((Vector2)antHillTr.position - pos).normalized;

            if(timerDan > timeToBeInDanger)
            {
                timerDan = 0;
                if (withFood)
                    changeState(AntState.WithFood);
                else changeState(AntState.Idle);
            }

            if (Vector2.Distance(antHillTr.position, transform.position) < foodDropRadius)
            {
                // cambiar el estado de la hormiga
                if (state == AntState.WithFood)
                {
                    changeState(AntState.Idle);
                    antHill.addFood(1);
                    withFood = false;
                }
                else
                {
                    // cambiar el estado de la hormiga
                    if (withFood)
                        changeState(AntState.WithFood);
                    else changeState(AntState.Idle);
                }
            }
        }

        dir += collisionDetector.checkCollision();
    }

    void IdleDir()
    {
        dir = (dir + Random.insideUnitCircle * wanderStr).normalized;

        if (!targFood)
        {
            targFood = getFood.getFood();

            dir += handlePheromones.detectPheromones();
        }
        else
        {
            Debug.DrawLine(transform.position, targFood.position, Color.red);
            //Debug.Log("Voy para mi comida");
            dir = (targFood.position - transform.position).normalized;

            const float foodPickUpRadius = 0.5f;
            if (Vector2.Distance(targFood.position, transform.position) < foodPickUpRadius)
            {
                // cambiar el estado de la hormiga
                Destroy(targFood.gameObject);
                changeState(AntState.WithFood);
                withFood = true;
                targFood = null;
            }
        }
    }

    public void changeState(AntState s)
    {
        if (state == s)
            return;
        state = s;
        switch (s)
        {
            case AntState.Idle:
                handlePheromones.changePheromoneSpawn(Pheromone.PheromoneType.Idle);
                break;
            case AntState.WithFood:
                handlePheromones.changePheromoneSpawn(Pheromone.PheromoneType.Food);
                break;
            case AntState.Danger:
                handlePheromones.changePheromoneSpawn(Pheromone.PheromoneType.Danger);
                break;
            default:
                Debug.LogError("Ant changed to unknown state!");
            break;
        }
        turningAround = true;
        skipLastP = true;
    }

    public void setUpAnt(Transform phPool, Transform antHTr, AntHill antH)
    {
        handlePheromones.setUp(phPool);
        antHillTr = antHTr;
        antHill = antH;
    }
}