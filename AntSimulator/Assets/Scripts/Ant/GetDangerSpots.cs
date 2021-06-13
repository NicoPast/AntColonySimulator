using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDangerSpots : MonoBehaviour
{
    // Layer Mask de los lugares de peligro
    [SerializeField]
    LayerMask DangerLayer;

    // angulo de vision
    [SerializeField]
    float viewAngle;
    
    // radio de vision
    [SerializeField]
    float viewRadius;

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

    // detecta si hay un punto de peligro en su campo de vision
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
                targDangSpot = dang;
            }
        }

        return targDangSpot;
    }

    // configuramos el detector
    public void setUp(float viewRad)
    {
        viewRadius = viewRad;
    }
}
