using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

[Serializable]
public struct buildingStatus
{
    [Header("���� ���g��菬������͐������ł�")]
    public float mass;
    [Header("�������ł�����mass")]
    public float blowingMass;
    [Header("�Ԃ����Ƃ��̃_���[�W")]
    public int power;
    [Header("���ł����x")]
    public float speed;
    [Header("�Ԃ����Đ�����ԑ��x")]
    public float blowingSpeed;
    [Header("�n�ʂɂ��Ă��牽�b�ŏ����邩")]
    public float fadeOutTime;
    [Header("�G�ɂԂ����̉�")]
    public AudioClip[] hitSE;
    [Header("�G�ɐ�����΂��ꂽ���̉�")]
    public AudioClip blowSE;
    public enum ThrowType {Parabola,Straight }
    public ThrowType throwType;
    public bool NowGrabbing;
    public bool horizontalGrabFlag;
    /// <summary>
    /// Normal.�ʏ�̌����Ă���
    /// Linear.������ɔ�сA�����̏��������̂𐁂���΂��ꍇ
    /// Bomber.��������A���e�_�Ŕ�������
    /// Decoration �ڐG���邾���Ő�����Ԃ���
    /// </summary>
    public enum BuildingType { Normal,Boaling,Bomber,Decoration}
    public BuildingType type;
    public enum BlowType {Normal,toBill,toEnemy,toDecoration }
    public BlowType blowType;

    //���̒��̕��������łȂ���
    public enum DecorationType { inCity,outCity}
    public DecorationType decorationType;



}

public class BuildingStatus : MonoBehaviour
{
    public buildingStatus status;
    public Rigidbody rb;
    public GameObject colldierChecker;
    [SerializeField, Header("�n�ʂ��痣���ۂ̃G�t�F�N�g")]
    GameObject puffEffect;
    [SerializeField, Header("������ۂ̃G�t�F�N�g")]
    GameObject destroyPuffEffect;
    public bool isbombered=false;
    [Header("���X�̎���")]
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
        originMass = rb.mass;//���X�̎��ʂ�ۑ�
        status.NowGrabbing = false;
        status.blowingMass = 1;//���ݒ�
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
                    //�t�F�[�h�A�E�g�̊J�n
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
                    case "Decorations"://�؂�M���ȂǂɏՓ�
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
                    case "BlowBuildings"://�f�t�H���g���������ł�I�u�W�F�N�g
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
    /// ���ʂ̕ύX
    /// </summary>
    /// <param name="toOrigin">���̎��ʂɂ��邩�ۂ�</param>
    /// <param name="target">���ʂ�ς���Ώ�</param>
    public void MassChange(bool toOrigin,GameObject target)
    {
        switch (toOrigin)
        {
            case true:
                if(target.GetComponent<Rigidbody>()!=null)
                {
                    //���̎���
                    target.GetComponent<Rigidbody>().mass = originMass;
                }
                rb.mass = originMass;
                break;
            case false:
                rb.mass = status.blowingMass ;
                //�y������
                if (target.GetComponent<Rigidbody>() != null)
                {
                    target.GetComponent<Rigidbody>().mass = status.blowingMass;
                }
                break;
        }

    }

    /// <summary>
    /// �������������Ԃ������������傫�����ʂ��ǂ������肷��
    /// </summary>
    /// <param name="ThrowBuild">����������</param>
    /// <param name="HitBuild">�Ԃ���������</param>
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
    /// �����𐁂���΂�
    /// </summary>
    /// <param name="CollideObject">��΂��I�u�W�F�N�g</param>
    /// <param name="BlowPosition">�Ԃ������ʒu</param>
    /// <param name="BlowType">�ǂ���΂���</param>
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
            case buildingStatus.BlowType.toBill://�r���ƂԂ���
                MassChange(false, CollideObject);
                MainGameSys.instance.RubbleSpawn(transform.position, false);
                MainGameSys.instance.SmokeSpawn(CollideObject.transform.position);
                CollideObject.layer = LayerMask.NameToLayer("BlowBuildings");
                colliderRb.AddExplosionForce(collideStatus.blowingSpeed * blowRateToBill, BlowPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
                break;
            case buildingStatus.BlowType.toEnemy://�G�ƂԂ���
                MassChange(false, CollideObject);
                CollideObject.layer = LayerMask.NameToLayer("HitBuildings");//�n�ʂɐG���܂œG�ɐG��Ȃ��ʂ̃��C���[�ɕς���
                colliderRb.velocity = Vector3.zero;
                colliderRb.AddExplosionForce(collideStatus.blowingSpeed * blowRateToEnemy, BlowPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
                break;
            case buildingStatus.BlowType.toDecoration://�؂�M���ȂǂƂԂ���
                CollideObject.layer = LayerMask.NameToLayer("BlowDecorations");
                collideCol.isTrigger = false;
                colliderRb.AddExplosionForce(collideStatus.blowingSpeed * blowRateToBill, BlowPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
                colliderRb.useGravity = true;
                break;
        }
    }
    /// <summary>
    /// �����蔻��̗L����
    /// </summary>
    /// <param name="Building"></param>
    public void CollisionActive(GameObject Building)
    {
        Collider Col=Building.GetComponent<Collider>();
        Col.isTrigger = false;
    }

    

    
    /// <summary>
    /// �����ɂȂ�}�e���A���ɕύX����
    /// </summary>
    /// <param name="ToTransparent">�����ɂȂ�}�e���A���ɕύX���邩�ǂ���</param>
    /// <param name="builMaterial">�ύX��̃}�e���A��</param>
    public void Materialchange(bool ToTransparent)
    {
        var mRenderer = GetComponent<MeshRenderer>();
        switch (ToTransparent) 
        {
            case true:
                //�������ɂ���
                mRenderer.material = materials[1];
                mRenderer.material.SetColor("_Color", new Color(1, 1, 1, 0.75f));
                break;

            case false:
                //���ɖ߂�
                mRenderer.material = materials[0];
                mRenderer.material.SetColor("_Color", new Color(1, 1, 1, 1));
                break;
        }
    }
    /// <summary>
    /// ���X�ɓ����ɂȂ�̂��~�߂�
    /// </summary>
    public void StopFade()
    {
        StopCoroutine(coroutine);
        coroutine = null;
        coroutine = FadeOutBuildings(status.fadeOutTime);
    }

    /// <summary>
    /// �͂񂾂�n�ʂ��痣���ۂɃG�t�F�N�g�𐶐�����
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
    /// ���X�Ɍ����𓧖��ɂ���
    /// </summary>
    /// <param name="FadeTime">���S�ɓ����ɂȂ�܂ł̎���</param>
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
