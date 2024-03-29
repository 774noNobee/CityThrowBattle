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
    [SerializeField, Tooltip("プレイヤー")]
    GameObject player;
    [SerializeField,Header("ステージの壁")]
    GameObject[] stageWall;
    [SerializeField,Header("敵")]
    GameObject enemy;
    [SerializeField,Header("各種テキスト")]
    GameObject Clear, Failed, restartText;
    [SerializeField]
    GameObject countDownObj, startObj;
    [SerializeField, Header("建物のあった位置に出す煙")]
    GameObject[] smokes;
    [SerializeField, Header("地面と当たったら出てくるガレキ")]
    GameObject[] rubbles;
    [SerializeField, Header("地面に残るガレキ")]
    GameObject rubbleBlock;
    [SerializeField, Header("ポーズ画面")]
    GameObject pausePanel;
    [SerializeField, Header("ポーズ画面のボタン")]
    GameObject[] PauseButtons;
    [SerializeField, Header("最後に選択しているボタン")]
    GameObject lastSelectedButton;
    [SerializeField,Header("リザルトの背景")]
    GameObject resultBackGround;
    [SerializeField,Header("残ってる建物の数")]
    int remainBuilAmount;
    [SerializeField, Header("残ってる建物の数のマックス数")]
    int maxBuilAmount;
    [SerializeField,Header("残り建物用のテキスト")]
    TextMeshProUGUI remainBuilText;
    [SerializeField, Header("経過時間と残った建物結果表示用のテキスト")]
    TextMeshProUGUI resultText;
    [SerializeField, Header("カウントダウン用の音")]
    AudioClip[] countDownSEs;
    [SerializeField]
    AudioClip[] pauseSEs;
    [SerializeField,Header("バーチャルカメラ")]
    CinemachineVirtualCamera vCamera;
    [SerializeField,Header("カメラの注目度合い")]
    CinemachineTargetGroup targetGroup;
    [SerializeField]
    EventSystem eventSys;
    [SerializeField, Header("カウントダウンに使う数字の画像")]
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
                    //選択中のオブジェクトを大きくする
                    PauseButtons[i].transform.localScale = new Vector3(scaledRate, scaledRate, scaledRate);
                }
                else
                {
                    //選択していないオブジェクトを小さくする
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
    /// 残りの建物の割合を計算、表示する
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
        remainBuilText.text="　 :"+remainPercentage.ToString()+"%";
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
            //建物が無くなったら倒れる
            PlayercharaControll.instance.HP = 1;
            PlayercharaControll.instance.Damage();
            remainBuilIcon.sprite = remainBuilSprites[3];
            //Result(false);
            
        }
    }

    /// <summary>
    /// ポーズ画面の表示非表示を入れ替える
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
    /// スライダーの値を読み取り、音量変更メソッドを呼ぶ
    /// </summary>
    /// <param name="SoundType">BGMか効果音か</param>
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
    /// 結果表示
    /// </summary>
    /// <param name="isClear">敵を倒したかどうか</param>
    public void Result(bool isClear)
    {
        restartText.SetActive(true);
        resultBackGround.SetActive(true);
        switch(isClear)
        {
            case true:
                Clear.SetActive(true);
                resultText.gameObject.SetActive(true);
                resultText.text = "残った建物：" + Mathf.Floor(((float)remainBuilAmount / (float)maxBuilAmount) * 100) + "%\n撃退時間：" + (int)timer + "秒";
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
    /// 選択しているボタンを大きくする
    /// </summary>
    public void ButtonSelect()
    {
        GameObject button = eventSys.currentSelectedGameObject;
        if (button != lastSelectedButton)
        {
            SoundSys.instance.PlaySE(pauseSEs[0]);
        }
        //ボタンのない場所をクリックしていた場合、表示されているパネルに応じたボタンを選んでいる状態にする
        if (button == null)
        {
            eventSys.SetSelectedGameObject(PauseButtons[0]);
        }
        lastSelectedButton = eventSys.currentSelectedGameObject;

    }
    /// <summary>
    /// 選択しているボタンに応じた処理を行う
    /// </summary>
    public void ButtonAction()
    {
        GameObject selectButton= eventSys.currentSelectedGameObject;
        if (selectButton == PauseButtons[0])
        {
            //ポーズ画面を閉じて再開
            PauseSwitch();
        }else if(selectButton == PauseButtons[1])
        {
            //現ステージをやり直す
            
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
            //タイトルへ戻る
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
    /// 煙、炎の生成
    /// </summary>
    /// <param name="Pos">生成位置</param>
    public void SmokeSpawn(Vector3 Pos)
    {
        var Position = Pos;
        Position.y = 0;
        int smokesChoice = (int)Random.Range(0, 2);
        GameObject builSmoke = Instantiate(smokes[smokesChoice], Position, Quaternion.Euler(-90, 0, 0));

    }
    /// <summary>
    /// 瓦礫の生成
    /// </summary>
    /// <param name="rubblePos">生成位置</param>
    /// <param name="isrubbleBlock">塊で生成するか否か</param>
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
    /// カメラが敵に注目する
    /// </summary>
    /// <param name="isEnd">倒した後の爆発する段階かどうか</param>　
    public void  DefeatEnemyFeature(bool isEnd)
    {
        switch (isEnd)
        {
            case true:
                StartCoroutine(FeatureEnemyDeath());
                break;
            case false:
                //敵が気絶したとき
                StartCoroutine(FeatureEnemy());
                break;
        }
    }
    /// <summary>
    /// 敵を投げたらカメラが敵に注目する
    /// </summary>
    public void ThrowingEnemyFeature()
    {
        //倒した敵を投げ飛ばしたとき
        targetGroup.m_Targets[0].weight = 0;
    }
    /// <summary>
    /// カメラの注目を元に戻す
    /// </summary>
    public void RemoveFeatureCamera()
    {
        targetGroup.m_Targets[0].weight = playerCameraWeight;
    }
    /// <summary>
    /// 投げた建物や敵がステージの外壁をすり抜けるようにする
    /// </summary>
    public void WallLayerChange()
    {
        for(int i=0;i<stageWall.Length;i++)
        {
            stageWall[i].layer = LayerMask.NameToLayer("AfterDefeatWall");
        }
    }
    /// <summary>
    /// 開始時のカウントダウン
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
        //敵の行動開始
        enemy.GetComponent<EnemyMovement>().ChoiceAttack();
        startObj.SetActive(false);
        yield break;
    }
    /// <summary>
    /// 敵が気絶した際にカメラが敵に注目する
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
    /// 敵が爆発した際にカメラが敵に注目する
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
