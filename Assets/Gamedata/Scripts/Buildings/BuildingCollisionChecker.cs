using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BuildingCollisionChecker : MonoBehaviour
{
    [SerializeField]
    public BuildingStatus buildingStatus;
    [SerializeField]
    BoxCollider col;
    [SerializeField,Header("�������m���Ԃ������Ƃ��̉�")]
    private AudioClip hitSE;
    void Start()
    {
        buildingStatus=transform.parent.GetComponent<BuildingStatus>();
        col=GetComponent<BoxCollider>();
        Vector3 colSize = transform.parent.GetComponent<BoxCollider>().size;
        colSize.x += 1;
        colSize.y += 1;
        colSize.z += 1;
        col.size = colSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == transform.parent.gameObject) return;

        if(other.gameObject.CompareTag("Buildings"))
        {
            if(hitSE != null)
            {
                SoundSys.instance.PlaySE(hitSE);
            }
            //����̒������̂̂Ƃ������s���悤�ɂ���
            var otherStatus=other.GetComponent<BuildingStatus>();
            if(otherStatus==null) return;
            if(otherStatus.status.type==global::buildingStatus.BuildingType.Decoration)
            {
                return;
            }
            if(buildingStatus.MassCheck(transform.parent.gameObject,other.gameObject))//�d���̔�r
            {
                //true��������
                if (buildingStatus.status.type != global::buildingStatus.BuildingType.Boaling)
                {
                    buildingStatus.CollisionActive(transform.parent.gameObject);
                    gameObject.SetActive(false);
                    
                }
                else
                {
                    buildingStatus.BuildingBlow(other.gameObject, transform.position, global::buildingStatus.BlowType.toBill);
                }

            }else
            {
                buildingStatus.CollisionActive(transform.parent.gameObject);
            }
            
        }
    }
}
