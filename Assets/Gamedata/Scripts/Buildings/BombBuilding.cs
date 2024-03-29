using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BombBuilding : MonoBehaviour
{
    public enum BombType { Building,Missile}
    public BombType bombType;
    [SerializeField]
    Collider col;

    const int bombForce=60,explosionRadius = 10, upwardsModifier = 15;
    void Start()
    {
        col = GetComponent<Collider>();
        if(bombType == BombType.Missile)
        {
            GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            StartCoroutine(ActiveTrigger());
        }
    }
    
    private void OnEnable()
    {
        GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        StartCoroutine(ActiveTrigger());
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 explodePos = other.transform.position;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) return;
        if (other.gameObject.CompareTag("GrabArea")) return;
        switch (bombType)
        {
            case BombType.Building:
                if (other == transform.parent.gameObject) return;
                if (transform.parent.GetComponent<BuildingStatus>().isbombered) return;
                if (other.gameObject.CompareTag("Enemy"))
                {
                    var EnemyScript = other.gameObject.GetComponent<EnemyMovement>();
                    EnemyScript.Damage(transform.parent.GetComponent<BuildingStatus>().status.power, true,gameObject.transform.parent.gameObject);
                    return;
                }

                if (rb != null)
                {
                    //衝突したオブジェクトがプレイヤーでも爆発する建物自身でもない場合
                    if (!rb.gameObject.CompareTag("Player") && rb.gameObject != transform.parent)
                    {
                        other.gameObject.GetComponent<BuildingStatus>().PuffGenerate(explodePos, rb.gameObject);
                        rb.useGravity = false;
                        rb.constraints = RigidbodyConstraints.None;
                        rb.velocity = Vector3.zero;
                        rb.AddExplosionForce(bombForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
                        rb.gameObject.layer = LayerMask.NameToLayer("BlowBuildings");
                        rb.useGravity = true;
                    }
                    if (rb.gameObject.CompareTag("Player"))
                    {
                        rb.gameObject.GetComponent<PlayercharaControll>().Damage();
                    }
                }
                break;
            case BombType.Missile:
                if (rb != null)
                {
                    //衝突したオブジェクトがプレイヤーでも敵でもない場合
                    if (!rb.gameObject.CompareTag("Player")&&!rb.gameObject.CompareTag("Enemy"))
                    {
                        rb.useGravity = false;
                        rb.constraints = RigidbodyConstraints.None;
                        rb.velocity = Vector3.zero;
                        rb.AddExplosionForce(bombForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
                        rb.gameObject.layer = LayerMask.NameToLayer("BlowBuildings");
                        SoundSys.instance.PlaySE(other.GetComponent<BuildingStatus>().status.blowSE);
                        rb.useGravity = true;
                    }
                    if (rb.gameObject.CompareTag("Player"))
                    {
                        rb.gameObject.GetComponent<PlayercharaControll>().Damage();
                    }
                }
                break;
        }
    }
    /// <summary>
    /// 爆風の攻撃判定を有効にする
    /// </summary>
    /// <returns></returns>
    IEnumerator ActiveTrigger()
    {
        col.enabled = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        col.enabled = false;
        yield return null;
    }

}
