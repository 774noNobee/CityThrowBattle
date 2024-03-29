using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public enum SceneType {Title,Stage }
    public SceneType sceneType;
    [SerializeField]
    PlayerInput inputSys;
    [SerializeField]
    GameObject fadePanel;
    [SerializeField]
    Image fadeImage;
    [SerializeField]
    float fadeTime;
    [SerializeField]
    EventSystem eventSys;
    [SerializeField]
    GameObject[] buttons;
    [SerializeField]
    GameObject[] Panels;
    [SerializeField]
    AudioClip[] buttonSEs;
    IEnumerator coroutine;
    [SerializeField]
    GameObject lastSelectedButton,NowActivePanel;

    const float scaledRate = 1.25f;
    private void Start()
    {
        SoundSys.instance.PlayBGM(0);
        coroutine = null;
        eventSys.firstSelectedGameObject = buttons[0];
        lastSelectedButton = eventSys.firstSelectedGameObject;
        NowActivePanel = Panels[0];
        buttons[0].transform.localScale = new Vector3(scaledRate, scaledRate, scaledRate);
    }

    
    /// <summary>
    /// 選択しているボタンを大きく表示する
    /// </summary>
    public void OnNavigate()
    {
        GameObject button = eventSys.currentSelectedGameObject;
        if(button!=lastSelectedButton)
        {
            SoundSys.instance.PlaySE(buttonSEs[0]);
        }
        //ボタンのない場所をクリックしていた場合、表示されているパネルに応じたボタンを選んでいる状態にする
        if(button==null)
        {
            if (NowActivePanel == Panels[0])
            {
                eventSys.SetSelectedGameObject(buttons[0]);
            }else if (NowActivePanel == Panels[1])
            {
                eventSys.SetSelectedGameObject(buttons[2]);
            }else if (NowActivePanel = Panels[2])
            {
                eventSys.SetSelectedGameObject(buttons[3]);
            }
            else if(NowActivePanel == Panels[4])
            {
                eventSys.SetSelectedGameObject(buttons[7]);
            }
            else if((NowActivePanel == Panels[5]))
            {
                eventSys.SetSelectedGameObject(buttons[9]);
            }
            button = eventSys.currentSelectedGameObject;
        }
        lastSelectedButton =eventSys.currentSelectedGameObject;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (button == buttons[i])
            {
                buttons[i].transform.localScale = new Vector3(scaledRate, scaledRate, scaledRate);
            }
            else
            {
                buttons[i].transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
    /// <summary>
    /// 選択しているボタンを押す
    /// </summary>
    public void OnSubmit()
    {
        GameObject selectButton = eventSys.currentSelectedGameObject;
        if (selectButton != null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (selectButton.gameObject == buttons[i])
                {
                    ButtonsAction(i);
                }
            }
        }

    }
    /// <summary>
    /// ボタンごとの挙動
    /// </summary>
    /// <param name="ButtonNum">ボタン番号</param>
    public void ButtonsAction(int ButtonNum)
    {
        switch (ButtonNum)
        {
            case 0:
                //ステージ選択パネルの表示
                Panels[0].SetActive(false);
                Panels[1].SetActive(true);
                eventSys.SetSelectedGameObject(buttons[4]);
                SoundSys.instance.PlaySE(buttonSEs[0]);
                NowActivePanel = Panels[1];
                break;
            case 1: //ゲーム終了
                if (coroutine == null)
                {
                    coroutine = FadeOut(false,0);
                    StartCoroutine(FadeOut(false, 0));
                    SoundSys.instance.PlaySE(buttonSEs[2]);
                }
                break;
            case 2:
                //説明画面の表示
                Panels[1].SetActive(false);
                Panels[2].SetActive(true);
                eventSys.SetSelectedGameObject(buttons[3]);
                SoundSys.instance.PlaySE(buttonSEs[0]);
                NowActivePanel = Panels[2];
                break;
            case 3:
                //説明画面を非表示
                Panels[1].SetActive(true);
                Panels[2].SetActive(false);
                eventSys.SetSelectedGameObject(buttons[4]);
                SoundSys.instance.PlaySE(buttonSEs[2]);
                NowActivePanel = Panels[1];
                break;
            case 4:
                //ステージ1の画面表示
                Panels[1].SetActive(false);
                Panels[3].SetActive(true);
                Panels[4].SetActive(true);
                Panels[5].SetActive(false);
                eventSys.SetSelectedGameObject(buttons[7]);
                SoundSys.instance.PlaySE(buttonSEs[0]);
                NowActivePanel = Panels[4];

                break;
            case 5:
                //ステージ2の画面表示
                Panels[1].SetActive(false);
                Panels[3].SetActive(true);
                Panels[4].SetActive(false);
                Panels[5].SetActive(true);
                eventSys.SetSelectedGameObject(buttons[9]);
                SoundSys.instance.PlaySE(buttonSEs[0]);
                NowActivePanel = Panels[5];
                break;
            case 6:
                //タイトルへ戻る
                Panels[0].SetActive(true);
                Panels[1].SetActive(false);
                eventSys.SetSelectedGameObject(buttons[0]);
                SoundSys.instance.PlaySE(buttonSEs[2]);
                NowActivePanel = Panels[0];
                break;
            case 7:
                //ステージ1スタート
                if (coroutine == null)
                {
                    coroutine = FadeOut(true, 1);
                    StartCoroutine(FadeOut(true, 1));
                    SoundSys.instance.PlaySE(buttonSEs[1]);
                }
                break;
            case 8:
            case 10:
                //ステージ選択画面に戻る
                Panels[1].SetActive(true);
                Panels[3].SetActive(false); 
                Panels[4].SetActive(false);
                Panels[5].SetActive(false);
                eventSys.SetSelectedGameObject(buttons[4]);
                SoundSys.instance.PlaySE(buttonSEs[2]);
                NowActivePanel = Panels[1];
                break;
            case 9:
                //ステージ2スタート
                if (coroutine == null)
                {
                    coroutine = FadeOut(true, 2);
                    StartCoroutine(FadeOut(true, 2));
                    SoundSys.instance.PlaySE(buttonSEs[1]);
                }
                break;
        }
    }
    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="isGameStart">ゲームスタートするか否か</param>
    /// <param name="StageNum">ステージ番号</param>
    /// <returns></returns>
    IEnumerator FadeOut(bool isGameStart,int StageNum)
    {
        float fadeCount = 0;
        Color cl = new Color(0, 0, 0, 0);
        while (fadeCount < fadeTime)
        {
            fadeCount += Time.deltaTime;

            cl.a += Time.deltaTime;
            fadeImage.color = cl;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        switch (isGameStart)
        {
            case true:
                SceneManager.LoadScene(StageNum);
                break;
            case false:

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
                break;
        }

        yield break;
    }
}
