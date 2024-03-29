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
    /// ���ʉ���炷
    /// </summary>
    /// <param name="clip">�炷���ʉ��N���b�v</param>
    public void PlaySE(AudioClip clip)
    {
        //AudioSource.PlayClipAtPoint(clip, pos);
        seSource.PlayOneShot(clip);
    }

    /// <summary>
    /// BGM��炷
    /// </summary>
    /// <param name="bgmNum">BGM�̔ԍ�</param>
    public void PlayBGM(int bgmNum)
    {
        bgmSource.clip = bgmClips[bgmNum];
        bgmSource.Play();
    }

    /// <summary>
    /// �󂯎�����X���C�_�[�̒l�ɍ��킹�ĉ��ʂ��I���I�t�؂�ւ���B
    /// </summary>
    /// <param name="volume">�X���C�_�[�̒l</param>
    /// <param name="soundType">BGM�����ʉ���</param>
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
    /// BGM�̉��ʂ����X�ɉ�����
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
