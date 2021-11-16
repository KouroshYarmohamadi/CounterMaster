using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    [Header("Set a Range Number for Enemies")]
    [SerializeField] private int fromNumber = 30;
    [SerializeField] private int toNumber = 70;
    private int numberOfEnemies = 1;
    [SerializeField] private float enemyArmyDistanceFromLocalZero = 65f;


    [Header("Prefab Settings")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] TextMesh numberText;
    private float range;
    private int playerCountedNumber;
    private string HoldersName;

    Animator[] animators;
    BoxCollider boxCollider;

    [Header("Mesh Collider Settings")]
    public GameObject myMeshCollider;
    public float meshColliderScale = 1f;


    private void Awake()
    {
        numberOfEnemies = Random.Range(fromNumber, toNumber);
        numberText = GetComponent<TextMesh>();
        HoldersName = gameObject.name;
        DefineRange();
        InstansiateEnemies(numberOfEnemies);
        animators = GetComponentsInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        playerCountedNumber = PlayerMovements.GetInstance().GetCountNumber();

        if (transform.childCount <= 1) // " 1 " because of existance of red ring in between of child objects
            Destroy(gameObject);
    }
    void DefineRange()
    {
        switch (numberOfEnemies)
        {
            case int i when (1 <= numberOfEnemies && numberOfEnemies <= 4):
                range = 6f;
                break;
            case int i when (5 <= numberOfEnemies && numberOfEnemies <= 9):
                range = 8f;
                break;
            case int i when (10 <= numberOfEnemies && numberOfEnemies <= 19):
                range = 11f;
                break;
            case int i when (20 <= numberOfEnemies && numberOfEnemies <= 29):
                range = 15f;
                break;
            case int i when (30 <= numberOfEnemies && numberOfEnemies <= 39):
                range = 18f;
                break;
            case int i when (40 <= numberOfEnemies && numberOfEnemies <= 49):
                range = 20f;
                break;
            case int i when (50 <= numberOfEnemies && numberOfEnemies <= 59):
                range = 23f;
                break;
            case int i when (60 <= numberOfEnemies && numberOfEnemies <= 69):
                range = 25f;
                break;
            case int i when (70 <= numberOfEnemies && numberOfEnemies <= 79):
                range = 27f;
                break;
            case int i when (80 <= numberOfEnemies && numberOfEnemies <= 89):
                range = 28f;
                break;
            case int i when (90 <= numberOfEnemies && numberOfEnemies <= 99):
                range = 29f;
                break;
            case int i when (100 <= numberOfEnemies && numberOfEnemies <= 109):
                range = 30f;
                break;
            case int i when (110 <= numberOfEnemies && numberOfEnemies <= 119):
                range = 31f;
                break;
            case int i when (120 <= numberOfEnemies && numberOfEnemies <= 129):
                range = 32f;
                break;
            case int i when (130 <= numberOfEnemies && numberOfEnemies <= 139):
                range = 33f;
                break;
            case int i when (140 <= numberOfEnemies && numberOfEnemies <= 149):
                range = 34f;
                break;
            case int i when (150 <= numberOfEnemies && numberOfEnemies <= 179):
                range = 36f;
                break;
            case int i when (180 <= numberOfEnemies && numberOfEnemies <= 209):
                range = 39f;
                break;
            case int i when (210 <= numberOfEnemies && numberOfEnemies <= 350):
                range = 45f;
                break;
            default:
                range = 47f;
                break;
        }
        myMeshCollider.transform.localScale = new Vector3(range / 5f, 1f, range / 5f); //changing size of rounded mesh collidere
    }

    void InstansiateEnemies(int num)
    {
        num = numberOfEnemies;
        for (int i = 1; i < num; i++)
        {
            Vector3 holderPosition = transform.position;

            Vector2 positin = Random.insideUnitCircle * range;
            Vector3 rotatedPos = new Vector3(positin.x, 0, positin.y + enemyArmyDistanceFromLocalZero);

            GameObject instantiatedGO = Instantiate(enemyPrefab,
                                                    holderPosition + rotatedPos, 
                                                    Quaternion.Euler(new Vector3(0f,0f,0f)));
            instantiatedGO.transform.parent = GameObject.Find(HoldersName).transform;
        }
        numberText.text = num + "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")// || other.gameObject.tag == "PlayerControler")
        {
            Destroy(myMeshCollider);
            foreach (Animator animator in animators)
            {
                if (animator != null)
                    animator.SetFloat("Forward", 1);
            }
        }
    }

}
