using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Status
{
    public int HP;
    public int Power;
    [Header("�΂̋ʂ𔭎˂��鐔")]
    public int fireCount;
    [Tooltip("���i�̈ړ����x")]
    public float moveSpeed;
    [Tooltip("�ːi���̈ړ����x")]
    public float chargeSpeed;
    public float originMoveSpeed;
    [Tooltip("�m�b�N�o�b�N�̗͗�")]
    public float knockBackPower;
    public bool isInvinsivle;
    ///�G�̏��
    public enum statusType {Normal,Attack,Defeat }
    public statusType Type;
}

public class EnemyStatus : MonoBehaviour
{
    public Status status;
    public EnemyMovement movement;
    void Start()
    {
        movement= GetComponent<EnemyMovement>();
        status.isInvinsivle = false;
        status.originMoveSpeed = status.moveSpeed;
    }


}
