using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MainGameSys : MonoBehaviour
{
    public static MainGameSys instance;
    [SerializeField, Tooltip("�v���C���[")]
    GameObject player;
    [SerializeField,Header("�X�e�[�W�̕�")]
    GameObject[] stageWall;
    [SerializeField,Header("�G")]
    GameObject enemy;
    [SerializeField,Header("�e��e�L�X�g")]
    GameObject Clear, Failed, restartText;
    [SerializeField]
    GameObject countDownObj, startObj;
    [SerializeField, Header("�����̂������ʒu�ɏo����")]
    GameObject[] smokes;
    [SerializeField, Header("�n�ʂƓ���������o�Ă���K���L")]
    GameObject[] rubbles;
    [SerializeField, Header("�n�ʂɎc��K���L")]
    GameObject rubbleBlock;
    [SerializeField, Header("�|�[�Y���")]
    GameObject pausePanel;
    [SerializeField, Header("�|�[�Y��ʂ̃{�^��")]
    GameObject[] PauseButtons;
    [SerializeField, Header("�Ō�ɑI�����Ă���{�^��")]
    GameObject lastSelectedButton;
    [SerializeField,Header("���U���g�̔w�i")]
    GameObject resultBackGround;
    [SerializeField,Header("�c���Ă錚���̐�")]
    int remainBuilAmount;
    [SerializeField, Header("�c���Ă錚���̐��̃}�b�N�X��")]
    int maxBuilAmount;
    [SerializeField,Header("�c�茚���p�̃e�L�X�g")]
    TextMeshProUGUI remainBuilText;
    [SerializeField, Header("�o�ߎ��ԂƎc�����������ʕ\���p�̃e�L�X�g")]
    TextMeshProUGUI resultText;
    [SerializeField, Header("�J�E���g�_�E���p�̉�")]
    AudioClip[] countDownSEs;
    [SerializeField]
    AudioClip[] pauseSEs;
    [SerializeField,Header("�o�[�`�����J����")]
    CinemachineVirtualCamera vCamera;
    [SerializeField,Header("�J�����̒��ړx����")]
    CinemachineTargetGroup targetGroup;
    [SerializeField]
    EventSystem eventSys;
    [SerializeField, Header("�J�E���g�_�E���Ɏg�������̉摜")]
    Sprite[] countDownNumbers;
    [SerializeField]
    Sprite[] remainBuilSprites;
    [SerializeField]
    Image remainBuilIcon;
    [SerializeField]
    Image countDownImage;
    [SerializeField]
    Indicator indicator;

    float playerCameraWeight;
    float timer;
    public bool isRestart;

    const int rubbleAmount = 5;
    const int rubbleRandomPos = 5;
    const float featureWait = 2.5f;
    const float scaledRate = 1.25f;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        Clear.SetActive(false);
        Failed.SetActive(false);
        restartText.SetActive(false);
        player = GameObject.Find("Player");
        remainBuilAmount = GameObject.FindGameObjectsWithTag("Buildings").Length;
        resultBackGround.SetActive(false);
        maxBuilAmount = GameObject.FindGameObjectsWithTag("Buildings").Length;
        pausePanel.SetActive(false);
        StartCoroutine(CountDown());
        remainBuilIcon.sprite = remainBuilSprites[0];
        playerCameraWeight = targetGroup.m_Targets[0].weight;
        timer = 0;
        isRestart = false;
        player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
        player.GetComponent<PlayerInput>().actions.FindActionMap("UI").Disable();
    }
    private void Update()
    {
        if(pausePanel.activeSelf)
        {
            for(int i = 0; i < PauseButtons.Length; i++) 
            {
                if (PauseButtons[i]==eventSys.currentSelectedGameObject)
                {
                    //�I�𒆂̃I�u�W�F�N�g��傫������
                    PauseButtons[i].transform.localScale = new Vector3(scaledRate, scaledRate, scaledRate);
                }
                else
                {
                    //�I�����Ă��Ȃ��I�u�W�F�N�g������������
                    PauseButtons[i].transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }
        }
        if(PlayercharaControll.instance.isAlive)
        {
            timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// �c��̌����̊������v�Z�A�\������
    /// </summary>
    /// <param name="value"></param>
    public void RemainBuildings(int value)
    {
        if (enemy == null) return;
        if (!enemy.GetComponent<EnemyMovement>().LivingCheck()) return;
        remainBuilAmount-=value;
        remainBuilText.text = remainBuilAmount + "/" + maxBuilAmount;
        float remainPercentage=((float)remainBuilAmount/(float)maxBuilAmount)*100;
        remainPercentage=Mathf.Floor(remainPercentage);
        remainBuilText.text="�@ :"+remainPercentage.ToString()+"%";
        if(remainPercentage<50)
        {
            remainBuilText.color = Color.yellow;
            remainBuilIcon.sprite = remainBuilSprites[1];

        }
        if(remainPercentage<10)
        {
            remainBuilText.color = Color.red;
            remainBuilIcon.sprite = remainBuilSprites[2];
        }
        if(remainBuilAmount <= 0)
        {
            //�����������Ȃ�����|���
            PlayercharaControll.instance.HP = 1;
            PlayercharaControll.instance.Damage();
            remainBuilIcon.sprite = remainBuilSprites[3];
            //Result(false);
            
        }
    }

    /// <summary>
    /// �|�[�Y��ʂ̕\����\�������ւ���
    /// </summary>
    public void PauseSwitch()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
        switch(pausePanel.activeSelf)
        {
            case true:
                Time.timeScale = 0;
                player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
                player.GetComponent<PlayerInput>().actions.FindActionMap("UI").Enable();
                eventSys.SetSelectedGameObject(PauseButtons[0]);
                SoundSys.instance.PlaySE(pauseSEs[0]);
                break;
            case false:
                Time.timeScale = 1;
                player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
                player.GetComponent<PlayerInput>().actions.FindActionMap("UI").Disable();
                SoundSys.instance.PlaySE(pauseSEs[2]);
                break;
        }
    }
    /// <summary>
    /// �X���C�_�[�̒l��ǂݎ��A���ʕύX���\�b�h���Ă�
    /// </summary>
    /// <param name="SoundType">BGM�����ʉ���</param>
    public void PrepareSetVolume(string SoundType)
    {
        float volume = 0;
        switch(SoundType)
        {
            case "BGM":
                volume = PauseButtons[3].GetComponent<Slider>().value;
                SoundSys.instance.SetVolume(volume,SoundType);
                break;
            case "SE":
                volume = PauseButtons[4].GetComponent<Slider>().value;
                SoundSys.instance.SetVolume(volume, SoundType);
                break;
        }
    }
    /// <summary>
    /// ���ʕ\��
    /// </summary>
    /// <param name="isClear">�G��|�������ǂ���</param>
    public void Result(bool isClear)
    {
        restartText.SetActive(true);
        resultBackGround.SetActive(true);
        switch(isClear)
        {
            case true:
                Clear.SetActive(true);
                resultText.gameObject.SetActive(true);
                resultText.text = "�c���������F" + Mathf.Floor(((float)remainBuilAmount / (float)maxBuilAmount) * 100) + "%\n���ގ��ԁF" + (int)timer + "�b";
                break;
            case false:
                if (Clear.activeSelf) return;
                Failed.SetActive(true);
                PlayercharaControll.instance.isAlive = false;
                break;
        }
        if(!player.GetComponent<SceneReloader>())
        {
            player.AddComponent<SceneReloader>();
        }
        SoundSys.instance.BGMFade();
    }
    
    /// <summary>
    /// �I�����Ă���{�^����傫������
    /// </summary>
    public void ButtonSelect()
    {
        GameObject button = eventSys.currentSelectedGameObject;
        if (button != lastSelectedButton)
        {
            SoundSys.instance.PlaySE(pauseSEs[0]);
        }
        //�{�^���̂Ȃ��ꏊ���N���b�N���Ă����ꍇ�A�\������Ă���p�l���ɉ������{�^����I��ł����Ԃɂ���
        if (button == null)
        {
            eventSys.SetSelectedGameObject(PauseButtons[0]);
        }
        lastSelectedButton = eventSys.currentSelectedGameObject;

    }
    /// <summary>
    /// �I�����Ă���{�^���ɉ������������s��
    /// </summary>
    public void ButtonAction()
    {
        GameObject selectButton= eventSys.currentSelectedGameObject;
        if (selectButton == PauseButtons[0])
        {
            //�|�[�Y��ʂ���čĊJ
            PauseSwitch();
        }else if(selectButton == PauseButtons[1])
        {
            //���X�e�[�W����蒼��
            
            if (!player.GetComponent<SceneReloader>())
            {
                player.AddComponent<SceneReloader>();
                player.GetComponent<SceneReloader>().OnReload();
            }
            isRestart = true;
            PauseSwitch();
        }
        else if( selectButton == PauseButtons[2])
        {
            //�^�C�g���֖߂�
            if (!player.GetComponent<SceneReloader>())
            {
                player.AddComponent<SceneReloader>();
                player.GetComponent<SceneReloader>().OnReload();
            }
            isRestart = false;
            PauseSwitch();
        }
    }
    /// <summary>
    /// ���A���̐���
    /// </summary>
    /// <param name="Pos">�����ʒu</param>
    public void SmokeSpawn(Vector3 Pos)
    {
        var Position = Pos;
        Position.y = 0;
        int smokesChoice = (int)Random.Range(0, 2);
        GameObject builSmoke = Instantiate(smokes[smokesChoice], Position, Quaternion.Euler(-90, 0, 0));

    }
    /// <summary>
    /// ���I�̐���
    /// </summary>
    /// <param name="rubblePos">�����ʒu</param>
    /// <param name="isrubbleBlock">��Ő������邩�ۂ�</param>
    public void RubbleSpawn(Vector3 rubblePos,bool isrubbleBlock)
    {
        switch (isrubbleBlock)
        {
            case true:
                Vector3 rubbleBlockPos = rubblePos;
                rubbleBlockPos.y = 0;
                GameObject RubbleBlock = Instantiate(rubbleBlock, rubbleBlockPos, Quaternion.identity);
                RubbleBlock.transform.rotation = Quaternion.Euler(0, (int)Random.Range(0, 360), 0);
                break;
            case false:
                for (int i = 0; i < rubbleAmount; i++)
                {
                    GameObject rubble = Instantiate(rubbles[Random.Range(0, rubbles.Length - 1)]);
                    Vector3 pos = rubblePos;
                    pos.x += Random.Range(-rubbleRandomPos, rubbleRandomPos);
                    pos.y = 0.1f;
                    pos.z += Random.Range(-rubbleRandomPos, rubbleRandomPos);
                    rubble.transform.position = pos;
                }
                break;
        }
    }
    /// <summary>
    /// �J�������G�ɒ��ڂ���
    /// </summary>
    /// <param name="isEnd">�|������̔�������i�K���ǂ���</param>�@
    public void  DefeatEnemyFeature(bool isEnd)
    {
        switch (isEnd)
        {
            case true:
                StartCoroutine(FeatureEnemyDeath());
                break;
            case false:
                //�G���C�₵���Ƃ�
                StartCoroutine(FeatureEnemy());
                break;
        }
    }
    /// <summary>
    /// �G�𓊂�����J�������G�ɒ��ڂ���
    /// </summary>
    public void ThrowingEnemyFeature()
    {
        //�|�����G�𓊂���΂����Ƃ�
        targetGroup.m_Targets[0].weight = 0;
    }
    /// <summary>
    /// �J�����̒��ڂ����ɖ߂�
    /// </summary>
    public void RemoveFeatureCamera()
    {
        targetGroup.m_Targets[0].weight = playerCameraWeight;
    }
    /// <summary>
    /// ������������G���X�e�[�W�̊O�ǂ����蔲����悤�ɂ���
    /// </summary>
    public void WallLayerChange()
    {
        for(int i=0;i<stageWall.Length;i++)
        {
            stageWall[i].layer = LayerMask.NameToLayer("AfterDefeatWall");
        }
    }
    /// <summary>
    /// �J�n���̃J�E���g�_�E��
    /// </summary>
    /// <returns></returns>
    IEnumerator CountDown()
    {
        PlayercharaControll.instance.isAlive = false;
        yield return new WaitForSeconds(1f);
        countDownImage.sprite = countDownNumbers[2];
        SoundSys.instance.PlaySE(countDownSEs[0]);
        yield return new WaitForSeconds(1f);
        countDownImage.sprite = countDownNumbers[1];
        SoundSys.instance.PlaySE(countDownSEs[0]);
        yield return new WaitForSeconds(1f);
        countDownImage.sprite = countDownNumbers[0];
        SoundSys.instance.PlaySE(countDownSEs[0]);
        yield return new WaitForSeconds(1f);

        PlayercharaControll.instance.isAlive = true;
        countDownObj.SetActive(false);
        startObj.SetActive(true);
        SoundSys.instance.PlaySE(countDownSEs[1]);
        yield return new WaitForSeconds(1f);
        indicator.enabled = true;
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1:
                SoundSys.instance.PlayBGM(1);
                break;
            case 2:
                SoundSys.instance.PlayBGM(2);
                break;
        }
        //�G�̍s���J�n
        enemy.GetComponent<EnemyMovement>().ChoiceAttack();
        startObj.SetActive(false);
        yield break;
    }
    /// <summary>
    /// �G���C�₵���ۂɃJ�������G�ɒ��ڂ���
    /// </summary>
    /// <returns></returns>
    IEnumerator FeatureEnemy()
    {
        vCamera.LookAt = enemy.transform;
        yield return new WaitForSeconds(featureWait);
        vCamera.LookAt=targetGroup.transform;
        vCamera.Follow=targetGroup.transform;
        yield return null;
    }
    /// <summary>
    /// �G�����������ۂɃJ�������G�ɒ��ڂ���
    /// </summary>
    /// <returns></returns>
    IEnumerator FeatureEnemyDeath()
    {
        ThrowingEnemyFeature();
        yield return new WaitForSeconds(featureWait);
        vCamera.LookAt = targetGroup.transform;
        vCamera.Follow = targetGroup.transform;
        RemoveFeatureCamera();
        yield return null;
    }
}
