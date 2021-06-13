using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDangerSpots : MonoBehaviour
{
    public LayerMask DangerLayer;
    public float viewAngle;
    public float viewRadius;

    private void OnDrawGizmosSelected()
    {
        Vector3 pointTo = transform.position + ((Quaternion.Euler(0, 0, -viewAngle / 2) * transform.right).normalized * viewRadius);
        Debug.DrawLine(transform.position, pointTo, Color.red);
        pointTo = transform.position + ((Quaternion.Euler(0, 0, viewAngle / 2) * transform.right).normalized * viewRadius);
        Debug.DrawLine(transform.position, pointTo, Color.red);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Transform getDangerSpot()
    {
        Transform targDangSpot = null;
        Collider2D[] vision = Physics2D.OverlapCircleAll(transform.position, viewRadius, DangerLayer);

        if (vision.Length > 0)
        {

            Transform dang = vision[Random.Range(0, vision.Length)].transform;
            Vector2 dirDang = (dang.position - transform.position).normalized;

            if (Vector2.Angle(transform.right, dirDang) < viewAngle / 2)
            {
                //Debug.Log("test");
                targDangSpot = dang;
            }
            else
            {
                //Debug.Log("No encontre nada");
            }
        }

        return targDangSpot;
    }

    public void setUp(float viewRad)
    {
        viewRadius = viewRad;
    }
}
