using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public float maxSpeed = 2;
    public float rotSpeed = 2;
    public float wanderStr = 1;

    // to make it follow your mouse
    //Vector3 target;

    Vector2 dir;
    Vector2 vel;
    Vector2 pos;
    
    Transform targFood;
    LayerMask foodLayer;
    public float viewRadius;
    public float viewAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //dir = ((Vector2)target - pos).normalized;
        SetDirection();
        Vector3 paintDir = transform.position + (transform.right.normalized * viewRadius);
        paintDir = Quaternion.AngleAxis(-viewAngle / 2, Vector3.up) * paintDir;
        Debug.DrawLine(transform.position, paintDir, Color.white);
        Vector3 paintDir2 = transform.position + (transform.right.normalized * viewRadius);
        paintDir2 = Quaternion.AngleAxis(viewAngle, Vector3.up) * paintDir2;
        Debug.DrawLine(transform.position, paintDir2, Color.red);

        Vector2 vel2 = dir * maxSpeed;
        Vector2 rot = (vel2 - vel) * rotSpeed;
        Vector2 acc = Vector2.ClampMagnitude(rot, rotSpeed) / 1;

        vel = Vector2.ClampMagnitude(vel + acc * Time.deltaTime, maxSpeed);
        pos += vel * Time.deltaTime;

        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(pos, Quaternion.Euler(0, 0, angle));
    }

    void SetDirection()
    {
        if (!targFood)
        {
            Collider2D[] vision = Physics2D.OverlapCircleAll(pos, viewRadius, foodLayer);

            if(vision.Length > 0)
            {
                Transform food = vision[Random.Range(0, vision.Length)].transform;
                Vector2 dirFood = (food.position - transform.position).normalized;

                if(Vector2.Angle(transform.right, dirFood) < viewAngle / 2)
                {
                    targFood = food;
                }
            }
            else
            {
                dir = (dir + Random.insideUnitCircle * wanderStr).normalized;
            }
        }
        else
        {
            dir = (targFood.position - transform.position).normalized;

            const float foodPickUpRadius = 0.05f;
            if(Vector3.Distance(targFood.position, transform.position) < foodPickUpRadius)
            {
                // cambiar el estado de la hormiga
                Destroy(targFood.gameObject);
                targFood = null;
            }
        }
    }
}
