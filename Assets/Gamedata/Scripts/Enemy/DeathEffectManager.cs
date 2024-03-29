using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectManager : MonoBehaviour
{
    [SerializeField]
    GameObject explosions;
    [SerializeField]
    int explosionNum;
    [SerializeField]
    float explosionInterval;
    void Start()
    {
        StartCoroutine(EffectGenerate());
    }

    void Update()
    {
        
    }

    IEnumerator EffectGenerate()
    {
        int explosionCount = 0;
        while(explosionCount<explosionNum)
        {
            GameObject explosion = Instantiate(explosions);
            explosion.transform.parent = transform;
            int xPos = Random.Range(-5, 6);
            int yPos = Random.Range(0, 6);
            int zPos = Random.Range(-5, 6);
            Vector3 pos = new Vector3(xPos, yPos, zPos);
            explosion.transform.localPosition = pos;
            //最初の爆発のときだけカメラを振動させる
            if(explosionCount<1)
            {
                explosion.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            }
            yield return new WaitForSeconds(explosionInterval);
            explosionCount++;
        }
        //勝ったテキストをここで表示
        if(explosionCount>=explosionNum)
        {
            MainGameSys.instance.Result(true);
        }

        yield break;
        
    }
}
