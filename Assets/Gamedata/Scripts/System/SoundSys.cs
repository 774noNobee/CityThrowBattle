using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSys : MonoBehaviour
{
    public static SoundSys instance;
    [SerializeField]
    public AudioSource seSource;
    [SerializeField]
    private AudioSource bgmSource;

    [SerializeField]
    private AudioClip[] bgmClips;
    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// 効果音を鳴らす
    /// </summary>
    /// <param name="clip">鳴らす効果音クリップ</param>
    public void PlaySE(AudioClip clip)
    {
        //AudioSource.PlayClipAtPoint(clip, pos);
        seSource.PlayOneShot(clip);
    }

    /// <summary>
    /// BGMを鳴らす
    /// </summary>
    /// <param name="bgmNum">BGMの番号</param>
    public void PlayBGM(int bgmNum)
    {
        bgmSource.clip = bgmClips[bgmNum];
        bgmSource.Play();
    }

    /// <summary>
    /// 受け取ったスライダーの値に合わせて音量をオンオフ切り替える。
    /// </summary>
    /// <param name="volume">スライダーの値</param>
    /// <param name="soundType">BGMか効果音か</param>
    public void SetVolume(float volume, string soundType)
    {
        switch(soundType)
        {
            case "BGM":
                bgmSource.volume = volume;
                break;
            case "SE":
                seSource.volume = volume;
                break;
        }
    }

    public void BGMFade()
    {
        StartCoroutine(BGMFadeOut());
    }

    /// <summary>
    /// BGMの音量を徐々に下げる
    /// </summary>
    /// <returns></returns>
    IEnumerator BGMFadeOut()
    {
        while(bgmSource.volume > 0)
        {
            bgmSource.volume -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return null;
    }
}
