using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject[] bloodSplash;
    GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("PlayerControler");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            int randomPrefabNum = Random.Range(0, bloodSplash.Length);
            GameObject instatiatedBlood = Instantiate(bloodSplash[randomPrefabNum], transform.position, Quaternion.identity);
            instatiatedBlood.transform.parent = GameObject.Find("Level").transform;
            Destroy(gameObject);
            Vector3.MoveTowards(transform.position, player.transform.position, 5f);
        }
    }

    private void Update()
    {
        transform.LookAt(player.transform);
    }
}
