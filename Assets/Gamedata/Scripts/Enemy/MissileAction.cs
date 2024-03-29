using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAction : MonoBehaviour
{
    [SerializeField]
    public GameObject missile;
    [SerializeField, Header("�~�T�C����f���n�߂�܂ł̊Ԋu")]
    public float waitMissileTime;
    [SerializeField, Header("�~�T�C�����m�̓f���Ԋu")]
    public float betweenMissileTime;
    [SerializeField, Header("��x�ɉ����̃~�T�C����f����")]
    public int missileCount;
    [SerializeField, Header("�~�T�C�����ˉ�")]
    public AudioClip missileSE;
}
