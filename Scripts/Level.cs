using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    private static Level instance;
    public static Level GetInstance() { return instance; }

    public float movementSpeed = 9;

    [Tooltip("this measure is about how much FloorPlane, moves on Z vector and then stops moving (end of leve).")]
    public float stopingMeasure = 1800f;

    [Tooltip("this measure is about distance of every obstacte/entrance/enemy game object from each other in the level.")]
    public int numberOfItems = 11;
    public float gameObjectZPosMeasure = 155f;

    private Vector3 firstPosition;
    private float distanceFromFirstPos;

    [Header("Entrances Settings")]
    [Tooltip("better to set this way: element 0: (pf_Entrance_randomMultiply), element 1: (pf_Entrance_randomSum)")]
    public GameObject[] entrances;
    private GameObject choosedEntrance;
    private bool isfirstEntranceMultiply = false;


    [Header("Enemies Settings")]
    public GameObject enemies;

    [Header("Obstacles Settings")]
    [Tooltip("better to set this way: element 0: (Main_Ramp)")]
    public GameObject[] obstacles;
    private GameObject choosedObstacle;
    private bool isRampUsed = false;

    [Header("UI Setting")]
    public GameObject UI_ReadyToPlay;
    public GameObject UI_WinScreen;
    public GameObject UI_LoseScreen;


    private GameObject firstEnemies;
    private GameObject secondEnemies;

    private bool isEveryPlayerCharacterDied = false;
    private bool isBattling = false;

    private const float BATTLE_TIME = .5f;
    private const float FINISH_Z_POS = 8f;

    private Game_States state;
    public enum Game_States
    {
        ReadyToPlay,
        Playing,
        Losing,
        Winning,
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        state = Game_States.ReadyToPlay;
        GenerateRandomLevel();
        UI_ReadyToPlay.gameObject.SetActive(true);
        UI_WinScreen.gameObject.SetActive(false);
        UI_LoseScreen.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch(state)
        {
            default:

            case Game_States.ReadyToPlay:
                Time.timeScale = 0;
                UI_ReadyToPlay.gameObject.SetActive(true);
                if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved //Touch input
                || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) //keyboad input
                    state = Game_States.Playing;
                break;
            case Game_States.Playing:
                Time.timeScale = 1;
                UI_ReadyToPlay.gameObject.SetActive(false);
                PlayLevel();
                if (isEveryPlayerCharacterDied)
                    state = Game_States.Losing;
                break;
            case Game_States.Winning:
                Time.timeScale = 0;
                UI_WinScreen.gameObject.SetActive(true);
                break;
            case Game_States.Losing:
                Time.timeScale = 0;
                UI_LoseScreen.gameObject.SetActive(true);
                break;
        }
    }

    private void PlayLevel()
    {
        //a raycast from camera to center of screen. to check if it's batteling time.
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            if (hit.collider.gameObject.name == "firstEnemies" || hit.collider.gameObject.name == "secondEnemies")
                if(PlayerMovements.GetInstance().GetPlayerRange() > 8f)
                    isBattling = true;
        //debug line:// Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.forward, Color.red);  

        //isBattling bool should be false again after each battle, in a constant timing.
        if (isBattling)
            Invoke("FalseBattlingBool", BATTLE_TIME);

        //level movement. (actually level is the whole floor, entrances, obstales, enemies...)
        distanceFromFirstPos = Vector3.Distance(firstPosition, transform.position);
        if (distanceFromFirstPos <= stopingMeasure && !isEveryPlayerCharacterDied && !isBattling)
            transform.Translate(0, 0, -1 * movementSpeed * Time.deltaTime);

        //winning condition.
        if (PlayerMovements.GetInstance().GetWonBool())
            state = Game_States.Winning;

        //losing condition
        if (PlayerMovements.GetInstance().GetCountNumber() <= 0)
            isEveryPlayerCharacterDied = true;
        if(isEveryPlayerCharacterDied)
            state = Game_States.Losing;
    }

    private void GenerateRandomLevel()
    {
        gameObjectZPosMeasure = stopingMeasure / numberOfItems - FINISH_Z_POS;
        firstPosition = transform.position;

        for (int item = 1; item <= numberOfItems; item++)
        {
            Vector3 instantiatePos = new Vector3(0f, 0f, item * gameObjectZPosMeasure);

            int EntranceIndex = Random.Range(0, entrances.Length);
            choosedEntrance = entrances[EntranceIndex];

            switch (item)
            {
                case 1: //shuld be an Entrance
                    if (choosedEntrance == entrances[0])
                        isfirstEntranceMultiply = true;
                    GameObject firstEntrance = Instantiate(choosedEntrance, instantiatePos, Quaternion.identity);
                    firstEntrance.transform.parent = GameObject.Find("Level").transform;

                    break;
                case 2: //shuld be an Entrance
                    if (isfirstEntranceMultiply)
                        choosedEntrance = entrances[1];
                    GameObject secondEntrance = Instantiate(choosedEntrance, instantiatePos, Quaternion.identity);
                    secondEntrance.transform.parent = GameObject.Find("Level").transform;
                    break;
                case 3: //shuld be some Enemies
                    firstEnemies = Instantiate(enemies, instantiatePos, Quaternion.identity);
                    firstEnemies.transform.parent = GameObject.Find("Level").transform;
                    firstEnemies.name = "firstEnemies";
                    break;
                case 7://shuld be an Entrance
                    if (isfirstEntranceMultiply)
                        choosedEntrance = entrances[2];
                    GameObject thirdEntrance = Instantiate(choosedEntrance, instantiatePos, Quaternion.identity);
                    thirdEntrance.transform.parent = GameObject.Find("Level").transform;
                    break;
                case 9://shuld be some Enemies
                    secondEnemies = Instantiate(enemies, instantiatePos, Quaternion.identity);
                    secondEnemies.transform.parent = GameObject.Find("Level").transform;
                    secondEnemies.name = "secondEnemies";
                    break;
                default: //by default, instantiate a random obstacle.
                    int obstacleIndex;

                    if (!isRampUsed) //to use ramp obstacle only once in each level.
                        obstacleIndex = Random.Range(0, obstacles.Length);
                    else
                        obstacleIndex = Random.Range(1, obstacles.Length);

                    choosedObstacle = obstacles[obstacleIndex];

                    if (choosedObstacle == obstacles[0])
                        isRampUsed = true;

                    GameObject instatiatedGO = Instantiate(choosedObstacle, instantiatePos, Quaternion.identity);
                    instatiatedGO.transform.parent = GameObject.Find("Level").transform;
                    break;
            }
        }
    }

    public void ReloadScene() 
    {
        //used this method on retry buttons.
        SceneManager.LoadScene(0);
    }

    private void FalseBattlingBool()
    {
        //used this method on an Invoke method(string usage), in the "PlayLeve()".
        isBattling = false;
    }

    public bool GetBattelingBoolInfo()
    {
        return isBattling;
    }
}
