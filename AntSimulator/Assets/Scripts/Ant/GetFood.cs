using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFood : MonoBehaviour
{
    public LayerMask foodLayer;
    public float viewAngle;
    public float viewRadius;

    private void OnDrawGizmosSelected()
    {
        Vector3 pointTo = transform.position + ((Quaternion.Euler(0, 0, -viewAngle / 2) * transform.right).normalized * viewRadius);
        Debug.DrawLine(transform.position, pointTo, Color.black);
        pointTo = transform.position + ((Quaternion.Euler(0, 0, viewAngle / 2) * transform.right).normalized * viewRadius);
        Debug.DrawLine(transform.position, pointTo, Color.black);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform getFood()
    {
        Transform targFood = null;
        Collider2D[] vision = Physics2D.OverlapCircleAll(transform.position, viewRadius, foodLayer);

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

        return targFood;
    }

    public void setUp(float viewRad)
    {
        viewRadius = viewRad;
    }
}
