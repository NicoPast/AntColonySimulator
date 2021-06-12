using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public float collisionAngle;
    public float warningRadius;
    public float dangerRadius;
    public float warningRotStr;
    public float dangerRotStr;

    bool frontTick = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 checkCollision()
    {
        Vector2 dir = Vector2.zero;
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

        return dir;
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
            d = (Quaternion.Euler(0, 0, collisionAngle) * (derecha.point - transform.position)).normalized;
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
}
