using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;

public struct AntUpdateJob : IJobParallelFor
{
    public void Execute(int index)
    {

    }
}

public class AntManager : MonoBehaviour
{
    [SerializeField]
    List<Ant> ants;

    private void Update()
    {
        var job = new AntUpdateJob();
        var jobHandle = job.Schedule(ants.Count, 1);
        jobHandle.Complete();
    }
}

public class Ant : MonoBehaviour
{
    public enum AntState
    {
        Idle = 0,
        WithFood,
        Danger,
        Undefined
    }

    public struct Data
    {
        Transform transform;

        float maxSpeed;
        float rotSpeed;
        float wanderStr;

        float viewRadius;
        float viewAngle;

        // vectores utilizados para el movimiento de la hormiga
        Vector2 dir;
        Vector2 vel;
        Vector2 pos;

        // comida objetivo y si tiene comida
        Transform targFood;
        bool withFood; // = false;

        AntState state; //= AntState.Undefined;

        AntHill antHill;

        CollisionDetector collisionDetector;
        GetFood getFood;
        HandlePheromones handlePheromones;
        GetDangerSpots getDangSpot;

        // buleano para darse la vuelta
        bool turningAround; //= false;

        // ultimas posiciones para evitar dar vueltas en circulos
        Vector2 lastPosA;
        Vector2 lastPosB;
        // timer para evitar quedarse dando vueltas
        float timerLP;
        // se salta una ejecucion, para cambio de estados y el primer momento
        bool skipLastP; // = true;

        float lastPosDist;
        float timeToUpdateLastPos;

        float timeToBeInDanger;
        float timerDan;

        float foodDropRadius; //= 0.5f;
        float foodPickUpRadius; //= 0.2f;

        public Data(Ant ant)
        {
            transform = ant.transform;

            maxSpeed = ant.maxSpeed;
            rotSpeed = ant.rotSpeed;
            wanderStr = ant.wanderStr;

            viewRadius = ant.viewRadius;
            viewAngle = ant.viewAngle;

            dir = Vector2.zero;
            vel = Vector2.zero;
            pos = ant.transform.position;

            targFood = null;
            withFood = false;

            state = AntState.Undefined;

            antHill = ant.antHill;

            collisionDetector = ant.collisionDetector;
            getFood = ant.getFood;
            handlePheromones = ant.handlePheromones;
            getDangSpot = ant.getDangSpot;

            turningAround = false;

            lastPosA = Vector2.zero;
            lastPosB = Vector2.zero;
            timerLP = 0;
            skipLastP = true;

            lastPosDist = ant.lastPosDist;
            timeToUpdateLastPos = ant.timeToUpdateLastPos;

            timeToBeInDanger = ant.timeToBeInDanger;
            timerDan = 0;

            foodDropRadius = ant.foodDropRadius;
            foodPickUpRadius = ant.foodPickUpRadius;

            changeState(AntState.Idle);
        }

        public void changeState(AntState s)
        {
            if (state == s)
                return;
            switch (state)
            {
                case AntState.Idle:
                    GameManager.instance().addIdleAnt(-1);
                    break;
                case AntState.WithFood:
                    GameManager.instance().addFoodAnt(-1);
                    break;
                case AntState.Danger:
                    GameManager.instance().addDangerAnt(-1);
                    break;
                case AntState.Undefined:
                    break;
                default:
                    Debug.LogError("Ant changed to unknown state!");
                    break;
            }
            switch (s)
            {
                case AntState.Idle:
                    GameManager.instance().addIdleAnt(1);
                    handlePheromones.changePheromoneSpawn(Pheromone.PheromoneType.Idle);
                    break;
                case AntState.WithFood:
                    GameManager.instance().addFoodAnt(1);
                    handlePheromones.changePheromoneSpawn(Pheromone.PheromoneType.Food);
                    break;
                case AntState.Danger:
                    GameManager.instance().addDangerAnt(1);
                    handlePheromones.changePheromoneSpawn(Pheromone.PheromoneType.Danger);
                    break;
                default:
                    Debug.LogError("Ant changed to unknown state!");
                    break;
            }
            state = s;
            turningAround = true;
            skipLastP = true;
        }

        public void Update()
        {
            //target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //dir = ((Vector2)target - pos).normalized;

            // si encuentra un lugar de peligro cambia el estado
            if (getDangSpot.getDangerSpot() != null)
            {
                changeState(AntState.Danger);
            }
            // para evitar remolinos detecta si esta cerca de una posicion en la que estuvo
            timerLP += Time.deltaTime;
            if (timerLP > timeToUpdateLastPos)
            {
                timerLP = 0;
                lastPosB = lastPosA;
                lastPosA = transform.position;
                // cambia a peligro para hacer que otras hormigas sigan su camino
                if (!skipLastP && Vector2.Distance(transform.position, lastPosB) < lastPosDist)
                {
                    changeState(AntState.Danger);
                }
                skipLastP = false;
            }
            // si se da la vuelta solo darte la vuelta
            if (turningAround)
            {
                dir = transform.right * -10;
                Debug.DrawRay(transform.position, dir, Color.green);
                turningAround = false;
            }
            else
            {
                // si no se da la vuelta busca la direccion siguiendo la logica normal
                SetDirection();
            }

            // actualiza la posicion, velocidad y aceleracion de la hormiga
            Vector2 vel2 = dir * maxSpeed;
            Vector2 rot = (vel2 - vel) * rotSpeed;
            Vector2 acc = Vector2.ClampMagnitude(rot, rotSpeed) / 1;

            vel = Vector2.ClampMagnitude(vel + acc * Time.deltaTime, maxSpeed);
            pos += vel * Time.deltaTime;

            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(pos, Quaternion.Euler(0, 0, angle));

            // por ultimo deja la pheromona si puede
            handlePheromones.LeavePheromones();
        }

        // calcula la direccion que debera seguir la hormiga
        void SetDirection()
        {
            if (state == AntState.Idle)
            {
                IdleDir();
            }
            else
            {
                // calcula la direccion con aleatoriedad + las feromonas + volver al hormiguero
                dir = (dir + Random.insideUnitCircle * wanderStr).normalized;
                dir += handlePheromones.detectPheromones();
                dir += ((Vector2)antHill.transform.position - pos).normalized * 0.5f;
                // si ya ha visto el hormiguero, va directo
                if (Vector2.Distance(transform.position, antHill.transform.position) < viewRadius && Vector2.Angle(transform.right, antHill.transform.position - transform.position) < viewAngle / 2)
                    dir = ((Vector2)antHill.transform.position - pos).normalized;

                // actualiza el tiempo si esta en peligro
                if (state == AntState.Danger)
                {
                    timerDan += Time.deltaTime;
                    // si se ha pasado su tiempo vuelve a su estado anterior
                    if (timerDan > timeToBeInDanger)
                    {
                        timerDan = 0;
                        if (withFood)
                            changeState(AntState.WithFood);
                        else changeState(AntState.Idle);
                    }
                }

                // si colisiona con el hormiguero, deja la comida o vuelve a su estado anterior
                if (Vector2.Distance(antHill.transform.position, transform.position) < foodDropRadius)
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

        // calcula la direccion en el estado idle de la hormiga
        void IdleDir()
        {
            // direccion aleatoria
            dir = (dir + Random.insideUnitCircle * wanderStr).normalized;

            // si no ha encontrado comida, buscala
            if (!targFood)
            {
                targFood = getFood.getFood();

                dir += handlePheromones.detectPheromones();
            }
            // si la has encontrado ve directamente a por ella
            else
            {
                Debug.DrawLine(transform.position, targFood.position, Color.red);
                //Debug.Log("Voy para mi comida");
                dir = (targFood.position - transform.position).normalized;

                if (Vector2.Distance(targFood.position, transform.position) < foodPickUpRadius)
                {
                    // cambiar el estado de la hormiga
                    Destroy(targFood.gameObject);
                    changeState(AntState.WithFood);
                    GameManager.instance().addFoodMap(-1);
                    withFood = true;
                    targFood = null;
                }
            }
        }
    }

    // maxima velocidad de la hormiga
    [SerializeField]
    float maxSpeed = 2;
    // fuerza de rotacion
    [SerializeField]
    float rotSpeed = 2;
    // fuerza de deambular
    [SerializeField]
    float wanderStr = 1;

    // radio de vision
    [SerializeField]
    float viewRadius = 10;
    // angulo de vision
    [SerializeField]
    float viewAngle = 60;

    // to make it follow your mouse
    //Vector3 target;

    [SerializeField]
    AntState state = AntState.Undefined;

    [SerializeField]
    AntHill antHill;

    //[SerializeField]
    //Transform antHillTr;

    // Scripts de la hormiga
    [SerializeField]
    CollisionDetector collisionDetector;
    [SerializeField]
    GetFood getFood;
    [SerializeField]
    HandlePheromones handlePheromones;
    [SerializeField]
    GetDangerSpots getDangSpot;

    // distancia en la que detecta girar en circulos
    [SerializeField]
    float lastPosDist;
    // tiempo de refresco de deteccion de atasque
    [SerializeField]
    float timeToUpdateLastPos;

    // tiempo en el que esta en peligro
    [SerializeField]
    float timeToBeInDanger;

    // constantes para la colision con el hormiguero y la comida
    [SerializeField]
    float foodDropRadius = 0.5f;
    [SerializeField]
    float foodPickUpRadius = 0.2f;

    // configuramos la hormiga
    public void setUpAnt(Transform phPool, AntHill antH)
    {
        handlePheromones.setUp(phPool);
        antHill = antH;
    }
}