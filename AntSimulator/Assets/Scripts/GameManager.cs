using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static private GameManager _instance;

    // textos de la izquierda
    [SerializeField]
    Text antsText;
    [SerializeField]
    Text foodText;
    [SerializeField]
    Text antHillFoodText;
    [SerializeField]
    Text dangerText;
    [SerializeField]
    Text pheromoneText;

    // textos de la derecha
    [SerializeField]
    Text antsIdleText;
    [SerializeField]
    Text antsFoodText;
    [SerializeField]
    Text antsDangerText;

    // Sliders de la UI
    [SerializeField]
    Text antCostText;
    [SerializeField]
    Slider antCostSlid;
    [SerializeField]
    Text antMaxText;
    [SerializeField]
    Slider antMaxSlid;

    //datos para mostrar en pantalla
    int numAnts;
    int numFood;
    int numDanger;
    int numPheromones;

    int numAntsIdle = 0;
    int numAntsFood = 0;
    int numAntsDanger = 0;

    // prefabs de comida y peligro
    [SerializeField]
    GameObject foodPref;
    [SerializeField]
    GameObject dangerPref;

    // el hormiguero 
    [SerializeField]
    AntHill antHill;

    // las pool de la escena
    [SerializeField]
    Transform foodPool;
    [SerializeField]
    Transform dangerPool;
    [SerializeField]
    Transform pheromonePool;

    static public GameManager instance()
    {
        return _instance;
    }

    void Awake()
    {
        if (!_instance)
            _instance = this;
        else
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        // setea los textos y las variables al dato correspondiente
        numFood = foodPool.childCount;
        numDanger = dangerPool.childCount;
        numAnts = (int)antHill.getAnts();
        numPheromones = pheromonePool.childCount;
        antsText.text = "Number of Ants: " + numAnts;
        foodText.text = "Food on Map: " + numFood;
        dangerText.text = "Danger on Map: " + numDanger;
        pheromoneText.text = "Pheromones on Map: " + numPheromones;

        antsIdleText.text = numAntsIdle + ": Idle Ants";
        antsFoodText.text = numAntsFood + ": Food Ants";
        antsDangerText.text = numAntsDanger + ": Danger Ants";
    }

    // Update is called once per frame
    void Update()
    {

    }

    // añade los listeners al slider para conseguir sus valores
    private void OnEnable()
    {
        antCostSlid.onValueChanged.AddListener(delegate { updateAntCost(); });
        antMaxSlid.onValueChanged.AddListener(delegate { updateAntMax(); });
    }

    // elimina los listener de los sliders
    private void OnDisable()
    {
        antCostSlid.onValueChanged.RemoveListener(delegate { updateAntCost(); });
        antMaxSlid.onValueChanged.RemoveListener(delegate { updateAntMax(); });
    }

    // -----------------------------------------------------------------
    // Actualiza las variables de los textos en funcion de la simulacion
    // -----------------------------------------------------------------

    public void addAnt(int ants)
    {
        numAnts += ants;
        if (numAnts < 0)
            numAnts = 0;
        antsText.text = "Number of Ants: " + numAnts;
    }

    public void addIdleAnt(int ants)
    {
        numAntsIdle += ants;
        if (numAntsIdle < 0)
            numAntsIdle = 0;
        antsIdleText.text = numAntsIdle + ": Idle Ants";
    }

    public void addFoodAnt(int ants)
    {
        numAntsFood += ants;
        if (numAntsFood < 0)
            numAntsFood = 0;
        antsFoodText.text = numAntsFood + ": Food Ants";
    }

    public void addDangerAnt(int ants)
    {
        numAntsDanger += ants;
        if (numAntsDanger < 0)
            numAntsDanger = 0;
        antsDangerText.text = numAntsDanger + ": Danger Ants";
    }

    public void addFoodMap(int foods)
    {
        numFood += foods;
        if (numFood < 0)
            numFood = 0;
        foodText.text = "Food on Map: " + numFood;
    }

    public void addDangersMap(int dangers)
    {
        numDanger += dangers;
        if (numDanger < 0)
            numDanger = 0;
        dangerText.text = "Dangers on Map: " + numDanger;
    }

    public void addPheromonesMap(int phs)
    {
        numPheromones += phs;
        if (numPheromones < 0)
            numPheromones = 0;
        pheromoneText.text = "Pheromones on Map: " + numPheromones;
    }

    public void showFoodAntHill()
    {
        antHillFoodText.text = "Food on AntHill: " + antHill.getFood();
    }

    public void updateAntCost()
    {
        antHill.setAntCost(antCostSlid.value);
        antCostText.text = "Ant Cost: " + antCostSlid.value;
    }

    public void updateAntMax()
    {
        antHill.setMaxAnts((int)antMaxSlid.value);
        antMaxText.text = "Max ants: " + (int)antMaxSlid.value;
    }

    // -----------------------------------------------------------------
    // FIN
    // -----------------------------------------------------------------

    // resetea la escena
    public void resetScene()
    {
        SceneManager.LoadScene(0);
    }

    // apaga la app cuando es una build
    public void turnOff()
    {
        Application.Quit();
    }

    // spawnea comida en funcion de la posicion del mouse
    public void spawnFood()
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Instantiate(foodPref, target, Quaternion.identity, foodPool);

        addFoodMap(1);
    }

    // spawnea un punto de peligro en funcion de la posicion del mouse
    public void spawnDanger()
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Instantiate(dangerPref, target, Quaternion.identity, foodPool);

        addDangersMap(1);
    }
}
