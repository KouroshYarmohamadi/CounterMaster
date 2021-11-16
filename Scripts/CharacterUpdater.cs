using System.Collections;
using UnityEngine;

//CharacterUpdater cllass is attached on every playerCharacter.
public class CharacterUpdater : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    public GameObject[] bloodPrefabs;
    public float jumpTime = 1.7f;

    Vector3 groundedPos;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        groundedPos = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.forward);
        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        //start "Running" animation.
        animator.SetFloat("Forward", 1);
        animator.SetBool("isJump", false);
    }

    private void Update()
    {
        //applyng animators root motion in battling situation, to make characters go toward enemies.
        if (Level.GetInstance().GetBattelingBoolInfo())
            animator.applyRootMotion = true;
        else
            animator.applyRootMotion = false;            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            int randomPrefabNum = Random.Range(0, bloodPrefabs.Length);
            GameObject instatiatedBlood = Instantiate(bloodPrefabs[randomPrefabNum], transform.position, Quaternion.identity);
            instatiatedBlood.transform.parent = GameObject.Find("Level").transform;
            gameObject.transform.parent = null;
            PlayerMovements.GetInstance().UpdateCountNumber();
            Destroy(gameObject);
        }
        
        if (other.gameObject.tag == "Ramp")
            Jump();
    }

    private void Jump()
    {
        animator.SetBool("isJump", true);
        StartCoroutine(FalseIsJumpBool());
    }

    IEnumerator FreezeYPos()
    {
        yield return new WaitForSeconds(jumpTime);
        if (transform.localPosition != groundedPos)
        {
            transform.localPosition = groundedPos;
        }

        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotation;
    }

    IEnumerator FalseIsJumpBool()
    {
        yield return new WaitForSeconds(.5f);
        animator.SetBool("isJump", false);
    }
}
