using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class EnemyMovement : MonoBehaviour
{
    public EnemyStatus enemyStatus;
    Rigidbody rb;
    [SerializeField]
    GameObject player,deathEffect,deathText;
    [SerializeField]
    protected Animator animator;
    AnimatorStateInfo stateInfo;
    [SerializeField]
    AudioClip deathSE;
    [SerializeField]
    AudioClip downSE;
    [SerializeField]
    AudioClip chaseSE;
    [SerializeField]
    AudioClip chargeSE;
    [SerializeField]
    AudioClip breathSE;
    [SerializeField]
    GameObject[] firePoints;
    [SerializeField]
    float chaseTime;
    [SerializeField]
    float chaseTimeCount;
    [SerializeField]
    float chargeTime;
    [SerializeField]
    public float invinsibleTime;
    [SerializeField]
    bool NowCoroutinning;
    public enum AttackType {Chase,Charge,Breath }
    public AttackType ATKType;
    public enum BreathType { Fire,Missile,Other}
    [SerializeField]
    BreathType breathType;
    bool isLookPlayer,alreadyDefeat;
    IEnumerator[] Coroutines;
    [SerializeField]
    int beforeCoroutineNum;
    [SerializeField]
    GameObject fireSpotParent;
    [SerializeField]
    GameObject knockBackCol;

    const float deathInterval = 3f;
    void Start()
    {
        enemyStatus = GetComponent<EnemyStatus>();
        enemyStatus.status.Type = Status.statusType.Normal;
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        SetLookPlayer(false);
        alreadyDefeat = false;
        chaseTimeCount = 0;
        SetNowCoroutinning(false);
        Coroutines = new IEnumerator[10];
        Coroutines[0] = null;
        Coroutines[1] = Chase();
        Coroutines[2] = Charge();
        Coroutines[3] = Breath();
        GameObject.Find("EnemyHPSlider").GetComponent<Slider>().maxValue = enemyStatus.status.HP;
        GameObject.Find("EnemyHPSlider").GetComponent<Slider>().value = enemyStatus.status.HP;
        DeathTextSet(false);
        //FireSpots���擾���A���̎q�I�u�W�F�N�g��FirePoints�Ɋi�[
        fireSpotParent = GameObject.Find("FireSpots").gameObject;
        if( fireSpotParent != null )
        {
            for(int i=0;i<fireSpotParent.transform.childCount;i++)
            {
                if (i >= firePoints.Length) return;
                firePoints[i] = fireSpotParent.transform.GetChild(i).gameObject;
            }
        }
    }

    void Update()
    {
        
        if (isLookPlayer)
        {
            //�v���C���[�̕���������
            transform.LookAt(new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z));
        }
        if(LivingCheck()==false)//�|������
        {
            for(int i = 0;i < Coroutines.Length; i++)
            {
                if (Coroutines[i]!=null)
                StopCoroutine(Coroutines[i]);
            }
            SetLookPlayer(false);
        }
        deathText.transform.rotation = Quaternion.Euler(deathText.transform.rotation.x, 360-transform.root.rotation.y, deathText.transform.rotation.z);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //�v���C���[�Ƀ_���[�W
            PlayercharaControll.instance.Damage();
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("StageWall")&&LivingCheck())
        {
            if (beforeCoroutineNum == 2) //�ːi���������璆�~������
            {
                if (enemyStatus.status.isInvinsivle) return;
                SetNowCoroutinning(false);
                enemyStatus.status.moveSpeed = enemyStatus.status.originMoveSpeed;
                enemyStatus.status.Type = Status.statusType.Normal;
                StopCoroutine(Coroutines[beforeCoroutineNum]);
                SetbeforeCoroutineNum(0);
                animator.SetTrigger("IdleTrigger");
                ChoiceAttack();
            }
        }
    }

    /// <summary>
    /// �G�̍s���𒊑I���ċN��������
    /// </summary>
    public void ChoiceAttack()
    {
        if (NowCoroutinning !=false) return;
        if (LivingCheck() == false) return;
        if (enemyStatus.status.Type == Status.statusType.Attack) return;
        knockBackCol.SetActive(false);
        for (int i = 1; i < Coroutines.Length; i++)
        {
            Coroutines[i] = null;
        }
        Coroutines[1] = Chase();
        Coroutines[2] = Charge();
        Coroutines[3] = Breath();

        enemyStatus.status.Type = Status.statusType.Attack;
        ATKType = (AttackType)Random.Range(0, 3);
        NowCoroutinning =true;
        //null���߂��邱�Ƃɂ���ē����������R���[�`����j��
        
            
        switch (ATKType)
        {
            case AttackType.Chase:
                SetbeforeCoroutineNum(1);
                animator.SetInteger("AttackType", 0);
                animator.SetTrigger("AttackTrigger");
                break;

            case AttackType.Charge:
                SetbeforeCoroutineNum(2);
                break;

            case AttackType.Breath:
                SetbeforeCoroutineNum(3);
                break;
        }
        StartCoroutine(Coroutines[beforeCoroutineNum]);
    }
    /// <summary>
    /// �v���C���[��ǐՂ���
    /// </summary>
    /// <returns></returns>
    IEnumerator Chase()//�v���C���[��ǐՂ���
    {
        
        SoundSys.instance.PlaySE(chaseSE);
        enemyStatus.status.moveSpeed = enemyStatus.status.originMoveSpeed;
        chaseTimeCount = 0;
        SetLookPlayer(true);
        while (chaseTimeCount<chaseTime)
        {
            transform.Translate(Vector3.forward * enemyStatus.status.moveSpeed * Time.deltaTime);
            chaseTimeCount += Time.deltaTime;
            yield return null;
        }
        //LookPlayer(false);
        SetLookPlayer(false);
        SetNowCoroutinning(false);
        enemyStatus.status.Type = Status.statusType.Normal;
        SetbeforeCoroutineNum(0);
        animator.SetTrigger("IdleTrigger");
        ChoiceAttack();
        yield break;
    }
    /// <summary>
    /// �v���C���[�Ɍ������ēːi����
    /// </summary>
    /// <returns></returns>
    IEnumerator Charge()//���b���v���C���[�̕������������A���߂������ֈ��b���ːi����
    {
        enemyStatus.status.moveSpeed = enemyStatus.status.chargeSpeed;
        isLookPlayer = true;
        //animator.SetTrigger("IdleTrigger");
        yield return new WaitForSeconds(3.0f);
        
        float chargeTimeCount = 0;
        animator.SetInteger("AttackType", 1);
        animator.SetTrigger("AttackTrigger");
        SoundSys.instance.PlaySE(chargeSE);
        while (chargeTimeCount<chargeTime)
        {
            
            SetLookPlayer(false);
            transform.Translate(Vector3.forward * enemyStatus.status.moveSpeed*Time.deltaTime);
            chargeTimeCount += Time.deltaTime;
            yield return null;
        }

        SetNowCoroutinning(false);
        enemyStatus.status.moveSpeed = enemyStatus.status.originMoveSpeed;
        enemyStatus.status.Type = Status.statusType.Normal;
        SetbeforeCoroutineNum(0);
        animator.SetTrigger("IdleTrigger");
        ChoiceAttack();
        yield break;
    }
    /// <summary>
    /// �G���甭�˕������������
    /// </summary>
    /// <returns></returns>
    IEnumerator Breath()
    {
        SetLookPlayer(true);
        switch (breathType)
        {
            case BreathType.Fire:
                //�΂̋ʂ��~�点��
                yield return new WaitForSeconds(3.0f);
                SetLookPlayer(false);

                animator.SetInteger("AttackType", 2);
                animator.SetTrigger("AttackTrigger");
                for (int i = 0; i < enemyStatus.status.fireCount; i++)
                {
                    yield return new WaitForSeconds(0.5f);
                    int randomx = Random.Range(-10, 10) * 3;
                    int randomY = Random.Range(-10, 10) * 3;
                    firePoints[i].transform.position = new Vector3(transform.position.x + randomx, firePoints[i].transform.position.y, transform.position.z + randomY);
                    firePoints[i].SetActive(true);
                }
                break;
            case BreathType.Missile:
                //�~�T�C���𔭎˂���
                MissileAction missile = GetComponent<MissileAction>();
                yield return new WaitForSeconds(missile.waitMissileTime);

                animator.SetInteger("AttackType", 2);
                animator.SetTrigger("AttackTrigger");
                for (int i = 0; i < missile.missileCount; i++)
                {
                    yield return new WaitForSeconds(missile.betweenMissileTime);
                    GameObject launchMissile = Instantiate(missile.missile, (transform.position + transform.forward * 2), transform.rotation);
                    launchMissile.transform.Rotate(0, 180, 0);
                    SoundSys.instance.PlaySE(breathSE);
                }
                break;
            case BreathType.Other:
                
                break;
        }
        SetNowCoroutinning(false);
        enemyStatus.status.Type = Status.statusType.Normal;
        beforeCoroutineNum = 0;
        animator.SetTrigger("IdleTrigger");
        ChoiceAttack();
        yield break;
    }

   
    /// <summary>
    /// �G�������Ă邩�ǂ���
    /// </summary>
    /// <returns>�����Ă���true</returns>
    public bool LivingCheck()
    {
        return enemyStatus.status.Type!=Status.statusType.Defeat;

    }
    public void Damage(int Damage,bool isKnockBack,GameObject StraightBuilding)
    {
        if (enemyStatus.status.isInvinsivle) return;
        enemyStatus.status.HP -= Damage;
        GameObject.Find("EnemyHPSlider").GetComponent<Slider>().value = enemyStatus.status.HP;
        //�m�b�N�o�b�N������
        if(isKnockBack)
        {
            if(StraightBuilding!=null)
            {
                Vector3 colldePos = StraightBuilding.transform.position;
                colldePos.y = transform.position.y;
                transform.LookAt(colldePos);
            }
            knockBackCol.SetActive(true);
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.AddForce(-transform.forward * enemyStatus.status.knockBackPower, ForceMode.VelocityChange);
            
        }
        if (enemyStatus.status.HP > 0)
        {

            if(Coroutines!= null)
            {
                StopAllCoroutines();
            }
            //���G���Ԓ��̓_��
            GetComponent<InvincibleTime>().InvincibleActivate();
            enemyStatus.status.isInvinsivle = true;
            SetLookPlayer(false);
            Invoke("AttackRestart", invinsibleTime);
        }
        else
        {
            SoundSys.instance.PlaySE(downSE);
            MainGameSys.instance.DefeatEnemyFeature(false);
            DeathTextSet(true);
            knockBackCol.SetActive(true);
            enemyStatus.status.Type = Status.statusType.Defeat;
            animator.SetTrigger("DeathTrigger");
        }

    }
    /// <summary>
    /// ���ԍ���Death���\�b�h���Ă�
    /// </summary>
    public void Defeat()
    {
        if (!alreadyDefeat)
        {
            Invoke("Death", deathInterval);
            alreadyDefeat = true;
        }

    }
    /// <summary>
    /// �G����������
    /// </summary>
    void Death()
    {
        MainGameSys.instance.DefeatEnemyFeature(true);
        SoundSys.instance.PlaySE(deathSE);
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /// <summary>
    /// ��e��ɍU�����ĊJ����
    /// </summary>
    void AttackRestart()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        SetLookPlayer(true);
        knockBackCol.SetActive(false);
        if (Coroutines[beforeCoroutineNum] != null)
        {
            if (Coroutines[beforeCoroutineNum] != null)
                StartCoroutine(Coroutines[beforeCoroutineNum]);
        }
        //else
        //{
        //    ChoiceAttack();
        //}
    }

    /// <summary>
    /// �G���C�₵�Ă���Ƃ��̃e�L�X�g�\��
    /// </summary>
    /// <param name="active"></param>
    public void DeathTextSet(bool active)
    {
        deathText.SetActive(active);
    }
    /// <summary>
    /// �C�₵���G��͂񂾎��̃e�L�X�g�\��
    /// </summary>
    public void BlowTextSet()
    {
        deathText.GetComponent<TextMeshPro>().text = "������΂��I�I";
    }
    /// <summary>
    /// �v���C���[�̕�������
    /// </summary>
    /// <param name="islooking">�v���C���[�̕����������ǂ���</param>
    public void SetLookPlayer(bool islooking)
    {
        isLookPlayer=islooking;
    }
    /// <summary>
    /// ���ݓG���s�������̃t���O��ݒ肷��
    /// </summary>
    /// <param name="isCoroutining">�s������</param>
    public void SetNowCoroutinning(bool isCoroutining)
    {
        NowCoroutinning = isCoroutining;
    }
    /// <summary>
    /// �N�����Ă���U���̔ԍ���ݒ肷��
    /// </summary>
    /// <param name="num">�U���ԍ�</param>
    public void SetbeforeCoroutineNum(int num)
    {
        beforeCoroutineNum = num;
    }
}
