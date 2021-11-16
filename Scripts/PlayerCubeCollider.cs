using UnityEngine;


//entrances knows this cube collider by it tag and triggeres by this cube collider.
public class PlayerCubeCollider : MonoBehaviour
{
    private static PlayerCubeCollider instance;
    public static PlayerCubeCollider GetInstance() { return instance; }

    private bool isJumpTime = false;
    public bool GetJumpTime() { return isJumpTime; }

    BoxCollider CubeCollider;

    private void Start()
    {
        instance = this;
        CubeCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        transform.localPosition = new Vector3(0f, 0f, PlayerMovements.GetInstance().GetPlayerRange());

        if (Level.GetInstance().GetBattelingBoolInfo())
            CubeCollider.enabled = false;
        else
            CubeCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ramp")
            isJumpTime = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ramp")
            isJumpTime = false;
    }
}
