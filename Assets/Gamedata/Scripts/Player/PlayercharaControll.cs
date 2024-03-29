
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayercharaControll : MonoBehaviour
{
    public static PlayercharaControll instance;
    [Header("�ړ����x"), SerializeField]
    private float Movespeed;
    [SerializeField, Header("�^�C�g���ɖ߂���ʉ�")]
    public AudioClip resumeButtonSE;
    [SerializeField,Header("���������ʉ�")]
    AudioClip[] defeatSEs;
    [SerializeField, Header("��e���ʉ�")]
    AudioClip missSE;
    [SerializeField,Header("�L�����N�^�[�R���g���[���[")]
    CharacterController cController;
    [SerializeField]
    Animator animator;


    float originYtransform;
    [SerializeField]
    float stunTime;
    public int HP;
    public bool isGround;
    public bool isAlive;
    public bool isInvinsible,isStunning;
    Vector3 knockBackVel=Vector3.zero;
    Vector2 inputMove;
    private void Awake()
    {
        instance = this;
        cController = GetComponent<CharacterController>();
        isAlive = true;
    }
    private void Start()
    {
        originYtransform = transform.position.y;
        isInvinsible = false;
        isStunning = false;
        GameObject.Find("PlayerHPSlider").GetComponent<Slider>().maxValue = HP;
        GameObject.Find("PlayerHPSlider").GetComponent<Slider>().value = HP;
    }
    private void Update()
    {
        //�|����Ă�������͂𖳎�
        if (!isAlive) return;
        
        if(inputMove!=Vector2.zero)
        {
            //�����A�j���[�V�����𓮂���
            animator.SetBool("isMoving", true);
        }else
        {
            //�����A�j���[�V�������~�߂�
            animator.SetBool("isMoving", false);
        }
        Vector3 moveVelocity =
            new Vector3(inputMove.x * Movespeed, 0, inputMove.y * Movespeed);
        //�t���[���P�ʂ̈ړ��ʂɕϊ�
        var moveDelta = moveVelocity * Time.deltaTime;
        
        if(!isStunning)//�U�����󂯂�����̏ꍇ�͓����Ȃ��悤�ɂ���
        {
            cController.Move(moveDelta);
        }else if(knockBackVel!=Vector3.zero)//�U�����󂯂���m�b�N�o�b�N������
        {
            cController.Move(knockBackVel * Time.deltaTime);
        }
        
        //�߂荞��Ńv���C���[�������̂������
        transform.position=new Vector3(transform.position.x, originYtransform, transform.position.z);
        
        if(inputMove!=Vector2.zero&&knockBackVel==Vector3.zero)//���͂������
        {
            var targetAngleY = -Mathf.Atan2(inputMove.y, inputMove.x) * Mathf.Rad2Deg + 90;
            //�I�u�W�F�N�g����]������
            transform.rotation=Quaternion.Euler(0, targetAngleY, 0);
        }
    }

    public void OnMove(InputValue value)
    {
        //�ړ��̓��͂����m
        inputMove = value.Get<Vector2>();
    }

    public void OnPause()
    {
        //�J�E���g�_�E�����łȂ����G������Ƃ��������j���[��ʂ��J����悤�ɂ���
        if (!isAlive) return;
        if (!GameObject.FindGameObjectWithTag("Enemy")) return;
        MainGameSys.instance.PauseSwitch();
    }
    public void OnNavigate()
    {
        MainGameSys.instance.ButtonSelect();
    }
    public void OnSubmit()
    {
        MainGameSys.instance.ButtonAction();
    }

    
    public void Damage()
    {
        if (isInvinsible) return;
        if (!isAlive) return;
        HP--;
        GameObject.Find("PlayerHPSlider").GetComponent<Slider>().value = HP;
        InvinsibleActive();
        GetComponent<InvincibleTime>().InvincibleActivate();
        StartCoroutine(DamageStun());
        if(HP<=0)
        {
            animator.SetTrigger("DeathTrigger");
            isAlive = false;
            GameObject.Find("PlayerHPSlider").GetComponent<Slider>().value = 0;
            MainGameSys.instance.Result(false);
            for(int i=0;i<defeatSEs.Length; i++)
            {
                SoundSys.instance.PlaySE(defeatSEs[i]);
            }

        }else
        {
            isAlive = true;
            SoundSys.instance.PlaySE(missSE);
        }
    }

    void InvinsibleActive()
    {
        isInvinsible = true;
        Invoke("InvinsibleDisActive", 3f);
    }
    void InvinsibleDisActive()
    {
        isInvinsible = false;
    }

    IEnumerator DamageStun()
    {
        knockBackVel=(-transform.forward*30f);
        isStunning = true;
        yield return new WaitForSeconds(stunTime);
        knockBackVel = Vector3.zero;
        isStunning = false;
        yield break;
    }

    public void AttackAnim()
    {
        animator.SetTrigger("AttackTrigger");
    }
    
}
