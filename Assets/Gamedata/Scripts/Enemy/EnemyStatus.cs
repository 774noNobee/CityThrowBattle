using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Status
{
    public int HP;
    public int Power;
    [Header("火の玉を発射する数")]
    public int fireCount;
    [Tooltip("普段の移動速度")]
    public float moveSpeed;
    [Tooltip("突進時の移動速度")]
    public float chargeSpeed;
    public float originMoveSpeed;
    [Tooltip("ノックバックの力量")]
    public float knockBackPower;
    public bool isInvinsivle;
    ///敵の状態
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
