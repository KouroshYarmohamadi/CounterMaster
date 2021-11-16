using System.Collections;
using UnityEngine;

//this class only instantiates particles and it attached on obstacle like hammer and saturn.
public class ParticlePlayer : MonoBehaviour
{
    public GameObject particle;
    private GameObject instantiatedGameObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            instantiatedGameObject = Instantiate(particle, transform);
            StartCoroutine(DestroyInstatiatedPaerticle());
        }
    }

    IEnumerator DestroyInstatiatedPaerticle()
    {
        yield return new WaitForSeconds(1f);
        Destroy(instantiatedGameObject);
    }
}
