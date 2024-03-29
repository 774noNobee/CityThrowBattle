using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleTime : MonoBehaviour
{
    [SerializeField, Header("�_�ł�����p�[�c")]
    GameObject[] objParts;
    [SerializeField, Header("�_�ł����鎞��")]
    float invincibleTime;
    [SerializeField, Header("�_�ł�����Ԋu")]
    float invincibleInterval;

    public void InvincibleActivate()
    {
        StartCoroutine(InvincibleStart());
    }

    public IEnumerator InvincibleStart()
    {
        //�_���[�W��ɓ_�ł�����
        float invincibleCount = 0;
        while(invincibleCount<invincibleTime)
        {
            for(int i = 0; i < objParts.Length; i++) 
            {
                if (objParts[i] != null &&
                    objParts[i].GetComponent<Renderer>()!=null)
                {
                    objParts[i].GetComponent<Renderer>().enabled = !objParts[i].GetComponent<Renderer>().enabled;
                }
            }
            invincibleCount += invincibleInterval;
            yield return new WaitForSeconds(invincibleInterval);

        }

        for(int i = 0;i<objParts.Length;i++)
        {
            if (objParts[i] != null &&
                    objParts[i].GetComponent<Renderer>() != null)
            {
                objParts[i].GetComponent<Renderer>().enabled = true;
            }
        }
        switch (gameObject.tag)
        {
            case "Enemy":
                GetComponent<EnemyStatus>().status.isInvinsivle=false;
                break;
        }
        yield break;
    }
}
