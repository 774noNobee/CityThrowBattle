using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

[Serializable]
public struct buildingStatus
{
    [Header("質量 自身より小さいやつは吹っ飛んでく")]
    public float mass;
    [Header("吹っ飛んでく時のmass")]
    public float blowingMass;
    [Header("ぶつけたときのダメージ")]
    public int power;
    [Header("飛んでく速度")]
    public float speed;
    [Header("ぶつかって吹っ飛ぶ速度")]
    public float blowingSpeed;
    [Header("地面についてから何秒で消えるか")]
    public float fadeOutTime;
    [Header("敵にぶつけたの音")]
    public AudioClip[] hitSE;
    [Header("敵に吹っ飛ばされた時の音")]
    public AudioClip blowSE;
    public enum ThrowType {Parabola,Straight }
    public ThrowType throwType;
    public bool NowGrabbing;
    public bool horizontalGrabFlag;
    /// <summary>
    /// Normal.通常の建ってる状態
    /// Linear.直線状に飛び、道中の小さいものを吹っ飛ばす場合
    /// Bomber.放物線状、着弾点で爆発する
    /// Decoration 接触するだけで吹っ飛ぶもの
    /// </summary>
    public enum BuildingType { Normal,Boaling,Bomber,Decoration}
    public BuildingType type;
    public enum BlowType {Normal,toBill,toEnemy,toDecoration }
    public BlowType blowType;

    //町の中の物かそうでないか
    public enum DecorationType { inCity,outCity}
    public DecorationType decorationType;



}

public class BuildingStatus : MonoBehaviour
{
    public buildingStatus status;
    public Rigidbody rb;
    public GameObject colldierChecker;
    [SerializeField, Header("地面から離れる際のエフェクト")]
    GameObject puffEffect;
    [SerializeField, Header("消える際のエフェクト")]
    GameObject destroyPuffEffect;
    public bool isbombered=false;
    [Header("元々の質量")]
    public float originMass;
    [SerializeField]
    Material[] materials;
    int groundCollisionCount = 0;
    IEnumerator coroutine;

    const int blowRateToBill = 6;
    const int blowRateToEnemy = 2;
    const int explosionRadius = 10, upwardsModifier = 15;
    void Start()
    {
        rb=GetComponent<Rigidbody>();
        originMass = rb.mass;//元々の質量を保存
        status.NowGrabbing = false;
        status.blowingMass = 1;//仮設定
        if(status.type!=buildingStatus.BuildingType.Decoration&&colldierChecker!=null)
        {
            colldierChecker.SetActive(false);
        }
        status.fadeOutTime = 6;
        coroutine = FadeOutBuildings(status.fadeOutTime);
        if(status.decorationType==buildingStatus.DecorationType.outCity)
        {
            Materialchange(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(LayerMask.LayerToName(gameObject.layer)=="ThrowBuildings")
        {

        }else
        {
            if (status.type != buildingStatus.BuildingType.Bomber&&status.type!=buildingStatus.BuildingType.Decoration)
            {
                colldierChecker.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (LayerMask.LayerToName(collision.gameObject.layer))
        {
            
            case "Ground":
                gameObject.GetComponent<Collider>().isTrigger = false;
                
                if (rb.constraints==RigidbodyConstraints.None)
                {
                    //フェードアウトの開始
                    StartCoroutine(coroutine);
                    if(status.type==buildingStatus.BuildingType.Decoration)
                    {
                        return;
                    }
                    if (gameObject.layer != LayerMask.NameToLayer("BlowDecorations"))
                    {
                        MainGameSys.instance.RubbleSpawn(transform.position, true);
                        MainGameSys.instance.RubbleSpawn(transform.position,false);
                    }
                    if (gameObject.layer != LayerMask.NameToLayer("BlowDecorations") && gameObject.layer != LayerMask.NameToLayer("HitBuildings"))
                    {
                        gameObject.layer = LayerMask.NameToLayer("BlowBuildings");
                    }
                }
                break;
            case "Enemy":
                switch (LayerMask.LayerToName(gameObject.layer))
                {
                    case "ThrowBuildings":
                        var EnemyScript = collision.gameObject.GetComponent<EnemyMovement>();
                        if (status.throwType == buildingStatus.ThrowType.Straight)
                        {
                            EnemyScript.Damage(status.power, true,gameObject);
                            
                        }
                        else
                        {
                            EnemyScript.Damage(status.power, false,null);
                        }
                            int seNum = (int)UnityEngine.Random.Range(0, status.hitSE.Length);
                            SoundSys.instance.PlaySE(status.hitSE[seNum]);

                        BuildingBlow(gameObject, collision.transform.position, buildingStatus.BlowType.toEnemy);
                        break;
                    case "Default":
                    case "BlowBuilding":
                        BuildingBlow(gameObject, collision.transform.position, buildingStatus.BlowType.toEnemy);
                        MainGameSys.instance.SmokeSpawn(transform.position); 
                        MainGameSys.instance.RubbleSpawn(transform.position, true);
                        MainGameSys.instance.RubbleSpawn(transform.position, false);
                        SoundSys.instance.PlaySE(status.blowSE);
                        break;
                    case "Decorations"://木や信号などに衝突
                        BuildingBlow(gameObject, collision.transform.position, buildingStatus.BlowType.toDecoration);
                        SoundSys.instance.PlaySE(status.blowSE);
                        break;
                }
                break;
            case "Decorations":
                BuildingBlow(collision.gameObject, collision.transform.position, buildingStatus.BlowType.toDecoration);

                break;
            
        }
        if (status.type == buildingStatus.BuildingType.Bomber)
        {
            if (groundCollisionCount > 0)
            {
                GetComponent<BombAreaSpawner>().BombSpawn(gameObject.transform);
            }
            groundCollisionCount++;

        }
        if (collision.gameObject.layer==LayerMask.NameToLayer("Ground")&&gameObject.layer==LayerMask.NameToLayer("ThrowBuildings"))
        {
            gameObject.layer = LayerMask.NameToLayer("BlowBuildings");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == colldierChecker) return;
        switch (LayerMask.LayerToName(other.gameObject.layer))
        {
            case "Player":
                if(status.type==buildingStatus.BuildingType.Decoration)
                {
                    BuildingBlow(gameObject, other.transform.position, buildingStatus.BlowType.toDecoration);
                    gameObject.layer = LayerMask.NameToLayer("BlowDecorations");
                    SoundSys.instance.PlaySE(status.blowSE);
                    return;
                }
                if (PlayercharaControll.instance.isStunning)
                {
                    BuildingBlow(gameObject, other.transform.position, buildingStatus.BlowType.toEnemy);
                    SoundSys.instance.PlaySE(status.blowSE);
                    MainGameSys.instance.SmokeSpawn(transform.position);
                    MainGameSys.instance.RubbleSpawn(transform.position, true);
                    MainGameSys.instance.RubbleSpawn(transform.position, false);
                }

                break;

            case "Ground":
                gameObject.GetComponent<Collider>().isTrigger = false;
                MainGameSys.instance.RubbleSpawn(transform.position, true);
                MainGameSys.instance.RubbleSpawn(transform.position, false);
                break;
            case "ThrowBuildings":

                break;
            case "BlowBuildings":

                break;
            case "Enemy":
                switch (LayerMask.LayerToName(gameObject.layer))
                {
                    case "ThrowBuildings":
                        var EnemyScript = other.GetComponent<EnemyMovement>();
                        if(status.throwType==buildingStatus.ThrowType.Straight)
                        {
                            EnemyScript.Damage(status.power,true,gameObject);
                        }else
                        {
                            EnemyScript.Damage(status.power,false,null);
                        }
                            int seNum = (int)UnityEngine.Random.Range(0, status.hitSE.Length);
                            SoundSys.instance.PlaySE(status.hitSE[seNum]);
                        BuildingBlow(gameObject, other.transform.position, buildingStatus.BlowType.toEnemy);
                        break;
                    case "Default":
                    case "BlowBuildings"://デフォルトか吹っ飛んでるオブジェクト
                        BuildingBlow(gameObject, other.transform.position, buildingStatus.BlowType.toEnemy);
                        MainGameSys.instance.SmokeSpawn(transform.position);
                        MainGameSys.instance.RubbleSpawn(transform.position, true);
                        MainGameSys.instance.RubbleSpawn(transform.position, false);
                        SoundSys.instance.PlaySE(status.blowSE);
                        break;

                }

                break;
            case "StageWall":
                gameObject.GetComponent<Collider>().isTrigger = false;
                break;
            case "EnemyFireBall":
                BuildingBlow(gameObject, other.transform.position, buildingStatus.BlowType.toEnemy);
                MainGameSys.instance.SmokeSpawn(transform.position);
                MainGameSys.instance.RubbleSpawn(transform.position, true);
                MainGameSys.instance.RubbleSpawn(transform.position, false);
                SoundSys.instance.PlaySE(status.blowSE);
                break;
            case "Decorations":
                BuildingBlow(other.gameObject, other.transform.position, buildingStatus.BlowType.toDecoration);
                if(other.gameObject.GetComponent<BuildingStatus>().status.decorationType==buildingStatus.DecorationType.outCity)
                {
                    MainGameSys.instance.SmokeSpawn(transform.position);
                    MainGameSys.instance.RubbleSpawn(transform.position, true);
                    MainGameSys.instance.RubbleSpawn(transform.position, false);
                }
                SoundSys.instance.PlaySE(status.blowSE);
                break;
            case "KnockBackCollider":
                if(status.type==buildingStatus.BuildingType.Decoration)
                {
                    if(status.decorationType==buildingStatus.DecorationType.outCity)
                    {
                        BuildingBlow(gameObject, transform.position, buildingStatus.BlowType.toDecoration);
                        MainGameSys.instance.SmokeSpawn(transform.position);
                        MainGameSys.instance.RubbleSpawn(transform.position, true);
                        MainGameSys.instance.RubbleSpawn(transform.position, false);
                    }
                    else
                    {
                        BuildingBlow(gameObject, other.transform.position, buildingStatus.BlowType.toDecoration);
                    }
                    rb.useGravity = true;
                }
                else
                {
                    BuildingBlow(gameObject, other.transform.position, buildingStatus.BlowType.toEnemy);
                    MainGameSys.instance.RubbleSpawn(transform.position, false);
                }
                SoundSys.instance.PlaySE(status.blowSE);
                break;
        }

        if(other.CompareTag("Enemy"))
        {
            SoundSys.instance.PlaySE(status.blowSE);
        }
    }

    /// <summary>
    /// 質量の変更
    /// </summary>
    /// <param name="toOrigin">元の質量にするか否か</param>
    /// <param name="target">質量を変える対象</param>
    public void MassChange(bool toOrigin,GameObject target)
    {
        switch (toOrigin)
        {
            case true:
                if(target.GetComponent<Rigidbody>()!=null)
                {
                    //元の質量
                    target.GetComponent<Rigidbody>().mass = originMass;
                }
                rb.mass = originMass;
                break;
            case false:
                rb.mass = status.blowingMass ;
                //軽くする
                if (target.GetComponent<Rigidbody>() != null)
                {
                    target.GetComponent<Rigidbody>().mass = status.blowingMass;
                }
                break;
        }

    }

    /// <summary>
    /// 投げた建物がぶつかった建物より大きい質量かどうか判定する
    /// </summary>
    /// <param name="ThrowBuild">投げた建物</param>
    /// <param name="HitBuild">ぶつかった建物</param>
    /// <returns></returns>
    public bool MassCheck(GameObject ThrowBuild, GameObject HitBuild)
    {
        if (ThrowBuild == null || HitBuild == null) return false;
        var ThrowStatus=ThrowBuild.GetComponent<BuildingStatus>().status;
        var HitStatus=HitBuild.GetComponent<BuildingStatus>().status;
        if(ThrowStatus.mass>HitStatus.mass)
        {
            return true;
        }else
        return false;
    }

    
    /// <summary>
    /// 建物を吹っ飛ばす
    /// </summary>
    /// <param name="CollideObject">飛ばすオブジェクト</param>
    /// <param name="BlowPosition">ぶつかった位置</param>
    /// <param name="BlowType">どう飛ばすか</param>
    public void BuildingBlow(GameObject CollideObject,Vector3 BlowPosition, buildingStatus.BlowType BlowType)
    {
        if(BlowType!=buildingStatus.BlowType.toEnemy&&BlowType!=buildingStatus.BlowType.toDecoration)
        {
            PuffGenerate(CollideObject.transform.position, CollideObject);
        }

        var collideStatus = CollideObject.GetComponent<BuildingStatus>().status;
        var collideCol=CollideObject.GetComponent<Collider>();
        var colliderRb=CollideObject.GetComponent<Rigidbody>();
        colliderRb.constraints = RigidbodyConstraints.None;
        
        switch(BlowType) 
        {
            case buildingStatus.BlowType.toBill://ビルとぶつかる
                MassChange(false, CollideObject);
                MainGameSys.instance.RubbleSpawn(transform.position, false);
                MainGameSys.instance.SmokeSpawn(CollideObject.transform.position);
                CollideObject.layer = LayerMask.NameToLayer("BlowBuildings");
                colliderRb.AddExplosionForce(collideStatus.blowingSpeed * blowRateToBill, BlowPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
                break;
            case buildingStatus.BlowType.toEnemy://敵とぶつかる
                MassChange(false, CollideObject);
                CollideObject.layer = LayerMask.NameToLayer("HitBuildings");//地面に触れるまで敵に触れない別のレイヤーに変える
                colliderRb.velocity = Vector3.zero;
                colliderRb.AddExplosionForce(collideStatus.blowingSpeed * blowRateToEnemy, BlowPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
                break;
            case buildingStatus.BlowType.toDecoration://木や信号などとぶつかる
                CollideObject.layer = LayerMask.NameToLayer("BlowDecorations");
                collideCol.isTrigger = false;
                colliderRb.AddExplosionForce(collideStatus.blowingSpeed * blowRateToBill, BlowPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
                colliderRb.useGravity = true;
                break;
        }
    }
    /// <summary>
    /// 当たり判定の有効化
    /// </summary>
    /// <param name="Building"></param>
    public void CollisionActive(GameObject Building)
    {
        Collider Col=Building.GetComponent<Collider>();
        Col.isTrigger = false;
    }

    

    
    /// <summary>
    /// 透明になるマテリアルに変更する
    /// </summary>
    /// <param name="ToTransparent">透明になるマテリアルに変更するかどうか</param>
    /// <param name="builMaterial">変更先のマテリアル</param>
    public void Materialchange(bool ToTransparent)
    {
        var mRenderer = GetComponent<MeshRenderer>();
        switch (ToTransparent) 
        {
            case true:
                //半透明にする
                mRenderer.material = materials[1];
                mRenderer.material.SetColor("_Color", new Color(1, 1, 1, 0.75f));
                break;

            case false:
                //元に戻す
                mRenderer.material = materials[0];
                mRenderer.material.SetColor("_Color", new Color(1, 1, 1, 1));
                break;
        }
    }
    /// <summary>
    /// 徐々に透明になるのを止める
    /// </summary>
    public void StopFade()
    {
        StopCoroutine(coroutine);
        coroutine = null;
        coroutine = FadeOutBuildings(status.fadeOutTime);
    }

    /// <summary>
    /// 掴んだり地面から離れる際にエフェクトを生成する
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="targetBuilding"></param>
    public void PuffGenerate(Vector3 tr,GameObject targetBuilding)
    {
        var targetStatus=targetBuilding.GetComponent<BuildingStatus>();
        if (targetStatus.puffEffect != null)
        {
            Instantiate(targetStatus.puffEffect, tr, Quaternion.identity);
            targetStatus.puffEffect = null;
        }

    }
    /// <summary>
    /// 徐々に建物を透明にする
    /// </summary>
    /// <param name="FadeTime">完全に透明になるまでの時間</param>
    /// <returns></returns>
    IEnumerator FadeOutBuildings(float FadeTime)
    {
        Materialchange(true);
        gameObject.layer = LayerMask.NameToLayer("HitBuildings");
        var material=GetComponent<Renderer>().material;
        material.SetColor("_Color", new Color(1, 1, 1, 1));
        float fadeAmount = 1 / (FadeTime)*Time.deltaTime;
        float fadeTimeCount = 0;
        while (material.color.a>0)
        {
            fadeTimeCount += fadeAmount;
            if(material.color.a-fadeAmount>=0)
            {
                material.SetColor("_Color", new Color(1, 1, 1, material.color.a - fadeAmount));
            }else
            {
                material.SetColor("_Color", new Color(1, 1, 1, 0));
            }
            yield return null;
        }
        if (status.type!=buildingStatus.BuildingType.Decoration)
        {
            MainGameSys.instance.RemainBuildings(1);
            Instantiate(destroyPuffEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject);
        yield break;
    }
}
