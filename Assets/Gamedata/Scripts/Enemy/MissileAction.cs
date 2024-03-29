using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAction : MonoBehaviour
{
    [SerializeField]
    public GameObject missile;
    [SerializeField, Header("ミサイルを吐き始めるまでの間隔")]
    public float waitMissileTime;
    [SerializeField, Header("ミサイル同士の吐く間隔")]
    public float betweenMissileTime;
    [SerializeField, Header("一度に何発のミサイルを吐くか")]
    public int missileCount;
    [SerializeField, Header("ミサイル発射音")]
    public AudioClip missileSE;
}
