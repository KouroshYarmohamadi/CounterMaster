using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceManager : MonoBehaviour
{
    public EntranceMode entranceMode;
    public int fromNumber;
    public int toNumber;
    private int number;
    private TextMesh text;
    public GameObject popupText;
    public float popUpTextDestroyTime = .8f;
    private bool triggered;
    private bool isDone;
    private MeshRenderer meshRenderer;
    private int sumNumberInMultiplySituation;
    private const int FIVE = 5;

    public enum EntranceMode
    {
        Sum,
        Multiply
    }

    private void Awake()
    {
        text = GetComponent<TextMesh>();
        triggered = false;
        isDone = false;
        number = Random.Range(fromNumber, toNumber);
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        switch (entranceMode)
        {
            case EntranceMode.Sum:
                text.text = "+" + number;
                break;

            case EntranceMode.Multiply:
                text.text = "×" + number;
                break;
        }
    }
    private void FixedUpdate()
    {
    }
    void Update()
    {
        if (!isDone)
        {
            switch (entranceMode)
            {
                case EntranceMode.Sum:
                    if (triggered)
                    {
                        StartCoroutine(SumCharacters()); //this functions are coroutines to be smoother.
                        HandleRestOfEntrance();
                    }
                    break;

                case EntranceMode.Multiply:
                    if (triggered)
                    {
                        StartCoroutine(MultiplyCharacters());
                        HandleRestOfEntrance();
                    }
                    break;
            }
        }

    }

    private void HandleRestOfEntrance()
    {
        PlayerMovements.GetInstance().HandleCapsuleCollider();
        meshRenderer.enabled = false;
        isDone = true;
        popUpNumber();  
    }

    private void popUpNumber()
    {
        GameObject textGO = Instantiate(popupText, PlayerMovements.GetInstance().GetPlayerCenterPosition(), 
                            Quaternion.identity, GameObject.Find("Player").transform);
        switch (entranceMode) 
        {
            case EntranceMode.Sum:
                textGO.GetComponent<TextMesh>().text = "+" + number.ToString();
                break;

            case EntranceMode.Multiply:
                textGO.GetComponent<TextMesh>().text = "+" + sumNumberInMultiplySituation.ToString();
                break;
        }
        Destroy(textGO, popUpTextDestroyTime);
    }

    IEnumerator MultiplyCharacters()
    {
        //to add or multiplying Characters by any entrance, in every frame we add only 5 characters to make game look better.
        int countNumber = PlayerMovements.GetInstance().GetCountNumber();
        int multipliedNum = countNumber * number;
        sumNumberInMultiplySituation = multipliedNum - countNumber;
        
        if (sumNumberInMultiplySituation % FIVE == 0)
        {
            while (PlayerMovements.GetInstance().GetCountNumber() < multipliedNum)
            {
                InstantitateCertainNumberOfCharacters(FIVE);
                yield return null;
            }
        }
        else
        {
            for (int i = sumNumberInMultiplySituation / FIVE; i >= 1; i--)
            {
                InstantitateCertainNumberOfCharacters(FIVE);
                yield return null;
            }
            for (int j = 1; j <= sumNumberInMultiplySituation % FIVE; j++)
            {
                PlayerMovements.GetInstance().InstantiateSingleCharacter();
                yield return null;
            }
        }
    }

    IEnumerator SumCharacters()
    {
        //to add or SumUp Characters by any entrance, in every frame we add only 5 characters to make game look better.
        int countNumber = PlayerMovements.GetInstance().GetCountNumber();
        if (number % FIVE == 0)
        {
            while (PlayerMovements.GetInstance().GetCountNumber() < countNumber + number)
            {
                InstantitateCertainNumberOfCharacters(FIVE);
                yield return null;
            }
        }
        else
        {
            for (int i = number / FIVE; i >= 1; i--)
            {
                InstantitateCertainNumberOfCharacters(5);
                yield return null;
            }
            for (int j = 1; j <= number % FIVE; j++)
            {
                PlayerMovements.GetInstance().InstantiateSingleCharacter();
                yield return null;
            }
        }
    }

    private void InstantitateCertainNumberOfCharacters(int num)
    {
        for (int n = 0; n < num; n++)
        {
            PlayerMovements.GetInstance().InstantiateSingleCharacter();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CubeCollider")
        {
            triggered = true;
            Destroy(transform.GetChild(0).gameObject); //deleting the blue sprite on trriggering.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "PlayerControler")
        {
            triggered = false;
        }
    }
}
