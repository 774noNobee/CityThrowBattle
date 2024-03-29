using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class Indicator : MonoBehaviour
{
    [SerializeField]
    Transform playerTr, cameraTr, enemyTr;
    [SerializeField]
    RectTransform indicatorRectTr;
    [SerializeField]
    float angle;
    private void Start()
    {
        indicatorRectTr.gameObject.SetActive(false);
    }
    void Update()
    {
        if(enemyTr != null&&!enemyTr.GetComponent<MeshRenderer>().isVisible&&enemyTr.GetComponent<EnemyStatus>().status.Type!=Status.statusType.Defeat)
        {
            indicatorRectTr.gameObject.SetActive(true);
            var rotation = Quaternion.LookRotation(enemyTr.position - playerTr.position);
            angle = (cameraTr.eulerAngles - rotation.eulerAngles).y;
            indicatorRectTr.localEulerAngles = new Vector3(0, 0, angle);
            indicatorRectTr.gameObject.transform.GetChild(0).localEulerAngles = new Vector3(0, 0, -angle);
        }else
        {
            indicatorRectTr.gameObject.SetActive(false);
        }
        
    }
}
