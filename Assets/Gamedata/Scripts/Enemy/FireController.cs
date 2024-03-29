using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField,Header("�n�ʂŎc�鎞��")]
    float remainTime;
    public enum FireType { FireBall,FireMark}
    [Header("�΂̋ʂƐ�������ʒu�𓯎��Ɉ���")]
    public FireType mode;
    Rigidbody rb;
    [SerializeField,Header("��������΂̋�")]
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
        //�΋��o���̖ڈ�������ĉ΋��𐶐�
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
