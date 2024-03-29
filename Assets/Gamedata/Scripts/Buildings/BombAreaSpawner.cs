using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAreaSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject bombArea;
    [SerializeField]
    GameObject explodeEffect;
    bool isAlreadyBombed;
    [SerializeField]
    AudioClip bombSE;
    void Start()
    {
        isAlreadyBombed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BombSpawn(Transform spawnPos)
    {
        if (isAlreadyBombed) return;
        SoundSys.instance.PlaySE(bombSE);
        bombArea.SetActive(true);
        GameObject effect=Instantiate(explodeEffect,transform.position,Quaternion.identity);
        //effect.transform.localScale = new Vector3(1, 1, 1);
        isAlreadyBombed=true;
    }
}
