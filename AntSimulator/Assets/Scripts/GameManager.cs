using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static private GameManager _instance;

    public Text antsText;
    public Text foodText;
    public Text antHillFoodText;
    public Text dangerText;
    public Text pheromoneText;

    public Text antCostText;
    public Slider antCostSlid;
    public Text antMaxText;
    public Slider antMaxSlid;

    uint numAnts;
    int numFood;
    int numDanger;
    int numPheromones;

    public GameObject foodPref;
    public GameObject dangerPref;

    public AntHill antHill;
    public Transform foodPool;
    public Transform dangerPool;
    public Transform pheromonePool;

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
        numFood = foodPool.childCount;
        numDanger = dangerPool.childCount;
        numAnts = antHill.numOfAnts;
        numPheromones = pheromonePool.childCount;
        antsText.text = "Number of Ants: " + numAnts;
        foodText.text = "Food on Map: " + numFood;
        dangerText.text = "Danger on Map: " + numDanger;
        pheromoneText.text = "Pheromones on Map: " + numPheromones;

        //Input.GetMouseButtonDown(0).
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        antCostSlid.onValueChanged.AddListener(delegate { updateAntCost(); });
        antMaxSlid.onValueChanged.AddListener(delegate { updateAntMax(); });
    }

    private void OnDisable()
    {
        antCostSlid.onValueChanged.RemoveListener(delegate { updateAntCost(); });
        antMaxSlid.onValueChanged.RemoveListener(delegate { updateAntMax(); });
    }

    public void addAnt(uint ants)
    {
        numAnts += ants;
        antsText.text = "Number of Ants: " + numAnts;
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
        antHillFoodText.text = "Food on AntHill: " + antHill.food;
    }

    public void updateAntCost()
    {
        antHill.antCost = antCostSlid.value;
        antCostText.text = "Ant Cost: " + antCostSlid.value;
    }

    public void updateAntMax()
    {
        antHill.maxAnts = (int)antMaxSlid.value;
        antMaxText.text = "Max ants: " + (int)antMaxSlid.value;
    }

    public void resetScene()
    {
        SceneManager.LoadScene(0);
    }

    public void turnOff()
    {
        Application.Quit();
    }

    public void spawnFood()
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Instantiate(foodPref, target, Quaternion.identity, foodPool);

        addFoodMap(1);
    }

    public void spawnDanger()
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Instantiate(dangerPref, target, Quaternion.identity, foodPool);

        addDangersMap(1);
    }
}
