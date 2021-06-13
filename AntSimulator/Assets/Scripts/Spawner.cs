using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // comprueba si el mouse esta encima del mapa y si clicas poder spawnear comida y lugares de peligro
    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0)){
            GameManager.instance().spawnFood();
        }
        else if(Input.GetMouseButtonDown(1))
        {
            GameManager.instance().spawnDanger();
        }
    }
}
