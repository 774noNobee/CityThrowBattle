
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayercharaControll : MonoBehaviour
{
    public static PlayercharaControll instance;
    [Header("移動速度"), SerializeField]
    private float Movespeed;
    [SerializeField, Header("タイトルに戻る効果音")]
    public AudioClip resumeButtonSE;
    [SerializeField,Header("負けた効果音")]
    AudioClip[] defeatSEs;
    [SerializeField, Header("被弾効果音")]
    AudioClip missSE;
    [SerializeField,Header("キャラクターコントローラー")]
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
        //倒されていたら入力を無視
        if (!isAlive) return;
        
        if(inputMove!=Vector2.zero)
        {
            //歩きアニメーションを動かす
            animator.SetBool("isMoving", true);
        }else
        {
            //歩きアニメーションを止める
            animator.SetBool("isMoving", false);
        }
        Vector3 moveVelocity =
            new Vector3(inputMove.x * Movespeed, 0, inputMove.y * Movespeed);
        //フレーム単位の移動量に変換
        var moveDelta = moveVelocity * Time.deltaTime;
        
        if(!isStunning)//攻撃を受けた直後の場合は動けないようにする
        {
            cController.Move(moveDelta);
        }else if(knockBackVel!=Vector3.zero)//攻撃を受けたらノックバックさせる
        {
            cController.Move(knockBackVel * Time.deltaTime);
        }
        
        //めり込んでプレイヤーが浮くのを避ける
        transform.position=new Vector3(transform.position.x, originYtransform, transform.position.z);
        
        if(inputMove!=Vector2.zero&&knockBackVel==Vector3.zero)//入力がある間
        {
            var targetAngleY = -Mathf.Atan2(inputMove.y, inputMove.x) * Mathf.Rad2Deg + 90;
            //オブジェクトを回転させる
            transform.rotation=Quaternion.Euler(0, targetAngleY, 0);
        }
    }

    public void OnMove(InputValue value)
    {
        //移動の入力を検知
        inputMove = value.Get<Vector2>();
    }

    public void OnPause()
    {
        //カウントダウン中でないかつ敵がいるときだけメニュー画面を開けるようにする
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
