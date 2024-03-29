using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{

    [SerializeField,Header("’…’eŒã‚Ì”š•—")]
    GameObject explodeArea;
    [SerializeField]
    GameObject explodeEffect;
    [SerializeField]
    GameObject smokeEffect;
    [SerializeField]
    float missileSpeed;
    [SerializeField]
    AudioClip bombSE;
    [SerializeField]
    float remainTime;
    void Start()
    {
        Invoke("Delete", remainTime);
    }

    void Update()
    {
        //transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        transform.Translate((-Vector3.forward) * missileSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        Instantiate(explodeArea,transform.position,Quaternion.identity);
        Instantiate(explodeEffect, collision.GetContact(0).point, Quaternion.identity);
        smokeEffect.transform.parent = null;
        smokeEffect.GetComponent<ParticleSystem>().Stop();
        SoundSys.instance.PlaySE(bombSE);
        Destroy(smokeEffect, 1f);
        Destroy(gameObject);
    }
    void Delete()
    {
        Instantiate(explodeArea, transform.position, Quaternion.identity);
        Instantiate(explodeEffect, transform.position, Quaternion.identity);
        smokeEffect.transform.parent = null;
        smokeEffect.GetComponent<ParticleSystem>().Stop();
        Destroy(smokeEffect, 1f);
        Destroy(gameObject);
    }
}
