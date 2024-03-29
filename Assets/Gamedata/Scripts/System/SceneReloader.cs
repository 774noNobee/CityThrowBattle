using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TitleManager;

public class SceneReloader : MonoBehaviour
{
    [SerializeField]
    PlayerInput inputSys;
    [SerializeField]
    Image fadeImage;
    IEnumerator coroutine;
    const float fadeTime = 1.5f;
    
    private void Awake()
    {
        coroutine = null;
        fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
    }

    public void OnReload()
    {
        if (coroutine == null)
        {
            AudioClip clip = GetComponent<PlayercharaControll>().resumeButtonSE;
            SoundSys.instance.PlaySE(clip);
            coroutine = FadeOut();
            StartCoroutine(FadeOut());
        }
    }
    IEnumerator FadeOut()
    {
        float fadeCount = 0;
        Color cr = new Color(0, 0, 0, 0);
        while (fadeCount < fadeTime)
        {
            fadeCount += Time.deltaTime;

            cr.a += Time.deltaTime;
            fadeImage.color = cr;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        switch (MainGameSys.instance.isRestart)
        {
            case true:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case false:
                SceneManager.LoadScene(0);
                break;
        }


        yield break;
    }
}
