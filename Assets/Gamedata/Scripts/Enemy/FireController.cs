using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField,Header("地面で残る時間")]
    float remainTime;
    public enum FireType { FireBall,FireMark}
    [Header("火の玉と生成する位置を同時に扱う")]
    public FireType mode;
    Rigidbody rb;
    [SerializeField,Header("生成する火の玉")]
    GameObject fireBall;
    [SerializeField]
    AudioClip fireSE;
    private void OnEnable()
    {
        //switch (mode)
        //{
        //    case FireType.FireBall:
        //        //rb = GetComponent<Rigidbody>();
        //        break;
        //    case FireType.FireMark:
        //        Invoke("MarkDisable", remainTime);
        //        break;
        //}
        if(mode==FireType.FireMark)
        {
            Invoke("MarkDisable", remainTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (mode == FireType.FireMark) return;
        switch (LayerMask.LayerToName(other.gameObject.layer))
        {
            case "Ground":
                rb=gameObject.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                StartCoroutine(GroundFire());
                break;
            case "Player":
                PlayercharaControll.instance.Damage();
                break;
        }
        if(other.gameObject.tag=="Player"&&mode==FireType.FireBall)
        {
            PlayercharaControll.instance.Damage();
        }

    }


    void MarkDisable()
    {
        //火球出現の目印を消して火球を生成
        Vector3 firePos=new Vector3(transform.position.x, transform.position.y+20, transform.position.z);
        Instantiate(fireBall, firePos, Quaternion.identity);
        SoundSys.instance.PlaySE(fireSE);
        gameObject.SetActive(false);
    }

    IEnumerator GroundFire()
    {
        yield return new WaitForSeconds(remainTime);
        Destroy(gameObject);
        yield break;
    }
}
