using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMovements : MonoBehaviour
{
    private static PlayerMovements instance;
    public static PlayerMovements GetInstance() { return instance; }
    
    private TextMesh scoreNumber; 
    private CapsuleCollider capsuleCol; 

    public GameObject myMeshCollider; //meshCollider is the rounded one that controls characters placement and limit them in a round circle.
    public float meshColliderScale = 1f;
    Vector3 meshColliderAccuratePos;

    public GameObject[] bloods;

    public GameObject characterPrefab;
    public CapsuleCollider charactersCapsuleCol;
    private GameObject mainCam;

    private int countNumber;
    private float movement = 0f;
    public float playerMoveSpeed = 22f;
    public float cameraMoveSpeed = 20f;
    public float cameraXPosMovementValue = 12f;
    public float screenEdgeTransformPosition_X = 41.4f;
    private float playerRange = 5f;
    float previousPlayerRange = 5f;

    float positiveXPos = 0f;
    float negativeXPos = 0f;

    private int randomPrefabNum; //for blood splash instantiation
    private bool isWon = false;




    private void Awake()
    {
        countNumber = 1;
        instance = this;
        mainCam = Camera.main.gameObject;
        scoreNumber = GetComponent<TextMesh>();
        capsuleCol = GetComponent<CapsuleCollider>();
        meshColliderAccuratePos = myMeshCollider.transform.localPosition;
    }

    private void Update()
    {
        if (!Level.GetInstance().GetBattelingBoolInfo())
        {
            //CharactersMovement_KeyboardInput(); //this  part is only for keyboard input and testing.
            CharactersMovement_TouchInput();
        }

        CharactersXPosCheck();       

        //from this line to end of UPDATE function, is to prevent bugs that may happen.
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(new Vector3(child.transform.position.x, 0f, child.transform.position.z),
                                              new Vector3(transform.position.x,0f,transform.position.z));
            if (distance > previousPlayerRange)
            {
                Vector3 holderPosition = transform.position;
                Vector2 position = Random.insideUnitCircle * playerRange;
                Vector3 rotatedPos = new Vector3(position.x, 0, position.y);
                Vector3 fromZeroTransform = rotatedPos + holderPosition;

                child.transform.position = new Vector3(fromZeroTransform.x, fromZeroTransform.y, fromZeroTransform.z);
            }
        }
        if (myMeshCollider.transform.localPosition != meshColliderAccuratePos)
        {
            myMeshCollider.transform.position = new Vector3(meshColliderAccuratePos.x + transform.position.x, 
                                                            meshColliderAccuratePos.y,
                                                            meshColliderAccuratePos.z + transform.position.z);
        }
        capsuleCol.radius = playerRange + 5;
        randomPrefabNum = Random.Range(0, bloods.Length);
    }

    private void LateUpdate()
    {
        CameraMovement();
    }

    private void CharactersMovement_TouchInput()
    {
        const float LERP_NUMBER = .98f;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector3 pos = transform.position;

            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Vector2 smoothTouchInput = Vector2.Lerp(pos, touchDeltaPosition, LERP_NUMBER);
            transform.Translate(smoothTouchInput * playerMoveSpeed * Time.deltaTime, 0);
            pos.x = Mathf.Clamp(transform.position.x,
                                -screenEdgeTransformPosition_X - negativeXPos,
                                screenEdgeTransformPosition_X - positiveXPos);
            transform.position = pos;
        }
    }

    private void CharactersMovement_KeyboardInput()
    {
        Vector3 pos = transform.position;
        movement = Input.GetAxisRaw("Horizontal"); //Keyboard controls 
        transform.Translate(movement * 50f /*speed*/ * Time.deltaTime, 0, 0);
        pos.x = Mathf.Clamp(transform.position.x,
                            -screenEdgeTransformPosition_X - negativeXPos, 
                            screenEdgeTransformPosition_X - positiveXPos);
        transform.position = pos;
    }

    private void CharactersXPosCheck()
    {
        int childCount = 0;
        if (countNumber != childCount)
        {
            childCount = countNumber;
            positiveXPos = 0f; 
            negativeXPos = 0f;
            foreach (Transform child in transform)
            {
                if (child.transform.localPosition.x >= 0f)
                {
                    if (positiveXPos <= child.transform.localPosition.x)
                    {
                        positiveXPos = child.transform.localPosition.x;
                    }
                }

                if (child.transform.localPosition.x <= 0f)
                {
                    if (negativeXPos >= child.transform.localPosition.x)
                    {
                        negativeXPos = child.transform.localPosition.x;
                    }
                }
            }
        }

    }

    private void CameraMovement()
    {
        Vector3 playerPositon = transform.position;
        float halfwayToEdge = screenEdgeTransformPosition_X / 2;

        switch(playerPositon.x)
        {
            case float i when (playerPositon.x + playerRange >= halfwayToEdge):
                mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition,
                                                               new Vector3(cameraXPosMovementValue, 0, 0), //camera right side position
                                                               cameraMoveSpeed * Time.deltaTime);
                break;
            case float i when (playerPositon.x - playerRange <= -halfwayToEdge):
                mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition,
                                                               new Vector3(-cameraXPosMovementValue, 0, 0), //camera left side position
                                                               cameraMoveSpeed * Time.deltaTime);
                break;
            case float i when (playerPositon.x + playerRange >= halfwayToEdge/2):
                mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition,
                                                               new Vector3(cameraXPosMovementValue/2, 0, 0), //camera semi-right position
                                                               cameraMoveSpeed * Time.deltaTime);
                break;
            case float i when (playerPositon.x - playerRange <= -halfwayToEdge/2):
                mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition,
                                                               new Vector3(-cameraXPosMovementValue/2, 0, 0), //camera semi-left position
                                                               cameraMoveSpeed * Time.deltaTime);
                break;
            default:
                mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition,
                                                               new Vector3(0, 0, 0), //camera main position (middle)
                                                               cameraMoveSpeed * Time.deltaTime);
                break;
        }
    }

    public void InstantiateSingleCharacter()
    {
        GameObject instantiatedGO = Instantiate(characterPrefab, transform.localPosition, Quaternion.identity);
        instantiatedGO.transform.parent = GameObject.FindGameObjectWithTag("PlayerControler").transform;
        instantiatedGO.transform.position = new Vector3(instantiatedGO.transform.position.x, 0f, instantiatedGO.transform.position.z);
        UpdateCountNumber();
    }

    public void HandleCapsuleCollider()
    {
        const float SOMETHING_MORE = 5f;
        capsuleCol.enabled = false;
        capsuleCol.radius = playerRange + SOMETHING_MORE;
        StartCoroutine(ReEnableCapsulCol());
    }

    public void UpdateCountNumber()
    {
        countNumber = transform.childCount - 2; // ( -2 ) because of existence of "CircleCollider" & "CubeCollider" gameobject between childs.
        scoreNumber.text = countNumber.ToString();

        //this switch alignes playerRange number by number to characters.
        switch (countNumber) 
        {
            default:

            case int i when (1 <= countNumber  && countNumber <= 4):
                playerRange = 6f;
                break;
            case int i when (5 <= countNumber && countNumber <= 9):
                playerRange = 8f;
                break;
            case int i when (10 <= countNumber && countNumber<= 19):
                playerRange = 11f;
                break;
            case int i when (20 <= countNumber && countNumber <= 29):
                playerRange = 15f;
                break;
            case int i when (30 <= countNumber && countNumber <= 39):
                playerRange = 18f;
                break;
            case int i when (40 <= countNumber && countNumber <= 49):
                playerRange = 20f;
                break;
            case int i when (50 <= countNumber && countNumber <= 59):
                playerRange = 23f;
                break;
            case int i when (60 <= countNumber && countNumber <= 69):
                playerRange = 25f;
                break;
            case int i when (70 <= countNumber && countNumber <= 79):
                playerRange = 27f;
                break;
            case int i when (80 <= countNumber && countNumber <= 89):
                playerRange = 28f;
                break;
            case int i when (90 <= countNumber && countNumber <= 99):
                playerRange = 29f;
                break;
            case int i when (100 <= countNumber && countNumber <= 109):
                playerRange = 30f;
                break;
            case int i when (110 <= countNumber && countNumber <= 119):
                playerRange = 31f;
                break;
            case int i when (120 <= countNumber && countNumber <= 129):
                playerRange = 32f;
                break;
            case int i when (130 <= countNumber && countNumber <= 139):
                playerRange = 33f;
                break;
            case int i when (140 <= countNumber && countNumber <= 149):
                playerRange = 34f;
                break;
            case int i when (150 <= countNumber && countNumber <= 179):
                playerRange = 36f;
                break;
            case int i when (180 <= countNumber && countNumber <= 209):
                playerRange = 39f;
                break;
            case int i when (210 <= countNumber && countNumber <= 350):
                playerRange = 45f;
                break;
            case int i when (351 <= countNumber && countNumber <= 500):
                playerRange = 47f;
                break;
        }

        //changing size of rounded mesh collider.
        if (playerRange > previousPlayerRange)
            UpdateMeshColliderScale();
        else if(playerRange < previousPlayerRange)
            Invoke("UpdateMeshColliderScale", 2f);
    }

    private void UpdateMeshColliderScale()
    {
        myMeshCollider.transform.localScale = new Vector3(playerRange / 5, 1f, playerRange / 5);
        previousPlayerRange = playerRange;
    }

    public int GetCountNumber()
    {
        return countNumber;
    }
       
    private void OnTriggerEnter(Collider other)
    {
        GameObject firstCharacter = null;
        float maximumZPlace = -30f; // " -30 " number is for safety.

        if (other.gameObject.tag == "Enemy")
        {
            for (int x = 2; x <= transform.childCount-1; x++)// GetChild(2) because of existence of "CircleCollider" & "CubeCollider" gameobject between childs.
            {
                if (transform.GetChild(x).position.z > maximumZPlace) //in battle, front characters should die first.
                {
                    maximumZPlace = transform.GetChild(x).position.z;
                    firstCharacter = transform.GetChild(x).gameObject;
                }
            }
            if (firstCharacter != null)
            {
                GameObject instatiatedBlood = Instantiate(bloods[randomPrefabNum], firstCharacter.transform.position, Quaternion.identity);
                instatiatedBlood.transform.parent = GameObject.Find("Level").transform;

                firstCharacter.gameObject.transform.parent = null;
                Destroy(firstCharacter);
            }
            maximumZPlace = -30f;
            UpdateCountNumber();
        }

        if (other.gameObject.tag == "EndGame")
            isWon = true;
    }

    public float GetPlayerRange()
    {
        return playerRange;
    }

    public Vector3 GetPlayerCenterPosition()
    {
        return transform.position;
    }

    public bool GetWonBool()
    {
        return isWon;
    }

    IEnumerator ReEnableCapsulCol()
    {
        yield return new WaitForSeconds(.8f);
        capsuleCol.enabled = true;
    }
}
