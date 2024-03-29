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
    /// �I�����Ă���{�^����傫���\������
    /// </summary>
    public void OnNavigate()
    {
        GameObject button = eventSys.currentSelectedGameObject;
        if(button!=lastSelectedButton)
        {
            SoundSys.instance.PlaySE(buttonSEs[0]);
        }
        //�{�^���̂Ȃ��ꏊ���N���b�N���Ă����ꍇ�A�\������Ă���p�l���ɉ������{�^����I��ł����Ԃɂ���
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
    /// �I�����Ă���{�^��������
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
    /// �{�^�����Ƃ̋���
    /// </summary>
    /// <param name="ButtonNum">�{�^���ԍ�</param>
    public void ButtonsAction(int ButtonNum)
    {
        switch (ButtonNum)
        {
            case 0:
                //�X�e�[�W�I���p�l���̕\��
                Panels[0].SetActive(false);
                Panels[1].SetActive(true);
                eventSys.SetSelectedGameObject(buttons[4]);
                SoundSys.instance.PlaySE(buttonSEs[0]);
                NowActivePanel = Panels[1];
                break;
            case 1: //�Q�[���I��
                if (coroutine == null)
                {
                    coroutine = FadeOut(false,0);
                    StartCoroutine(FadeOut(false, 0));
                    SoundSys.instance.PlaySE(buttonSEs[2]);
                }
                break;
            case 2:
                //������ʂ̕\��
                Panels[1].SetActive(false);
                Panels[2].SetActive(true);
                eventSys.SetSelectedGameObject(buttons[3]);
                SoundSys.instance.PlaySE(buttonSEs[0]);
                NowActivePanel = Panels[2];
                break;
            case 3:
                //������ʂ��\��
                Panels[1].SetActive(true);
                Panels[2].SetActive(false);
                eventSys.SetSelectedGameObject(buttons[4]);
                SoundSys.instance.PlaySE(buttonSEs[2]);
                NowActivePanel = Panels[1];
                break;
            case 4:
                //�X�e�[�W1�̉�ʕ\��
                Panels[1].SetActive(false);
                Panels[3].SetActive(true);
                Panels[4].SetActive(true);
                Panels[5].SetActive(false);
                eventSys.SetSelectedGameObject(buttons[7]);
                SoundSys.instance.PlaySE(buttonSEs[0]);
                NowActivePanel = Panels[4];

                break;
            case 5:
                //�X�e�[�W2�̉�ʕ\��
                Panels[1].SetActive(false);
                Panels[3].SetActive(true);
                Panels[4].SetActive(false);
                Panels[5].SetActive(true);
                eventSys.SetSelectedGameObject(buttons[9]);
                SoundSys.instance.PlaySE(buttonSEs[0]);
                NowActivePanel = Panels[5];
                break;
            case 6:
                //�^�C�g���֖߂�
                Panels[0].SetActive(true);
                Panels[1].SetActive(false);
                eventSys.SetSelectedGameObject(buttons[0]);
                SoundSys.instance.PlaySE(buttonSEs[2]);
                NowActivePanel = Panels[0];
                break;
            case 7:
                //�X�e�[�W1�X�^�[�g
                if (coroutine == null)
                {
                    coroutine = FadeOut(true, 1);
                    StartCoroutine(FadeOut(true, 1));
                    SoundSys.instance.PlaySE(buttonSEs[1]);
                }
                break;
            case 8:
            case 10:
                //�X�e�[�W�I����ʂɖ߂�
                Panels[1].SetActive(true);
                Panels[3].SetActive(false); 
                Panels[4].SetActive(false);
                Panels[5].SetActive(false);
                eventSys.SetSelectedGameObject(buttons[4]);
                SoundSys.instance.PlaySE(buttonSEs[2]);
                NowActivePanel = Panels[1];
                break;
            case 9:
                //�X�e�[�W2�X�^�[�g
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
    /// �t�F�[�h�A�E�g
    /// </summary>
    /// <param name="isGameStart">�Q�[���X�^�[�g���邩�ۂ�</param>
    /// <param name="StageNum">�X�e�[�W�ԍ�</param>
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
