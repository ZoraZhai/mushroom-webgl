using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BackgroundMusicController : MonoBehaviour
{
    public AudioSource mainTrack; // ������
    public AudioSource[] otherTracks; // ������������
    public AudioSource clickSound;//�����Ч
    public AudioSource pickSound;//��ժ��Ч

    private AudioSource currentTrack; // ��ǰ���ŵ���������

    private void Start()
    {
        PlayBGM();
    }
    public void PlayBGM()
    {
        // ��ʼ����������
        mainTrack.Play();
        // ��ʼ���������������
        StartCoroutine(PlayRandomTrack());
    }

    private IEnumerator PlayRandomTrack()
    {
        while (true)
        {
            // ���ѡ��һ����Ƶ����
            int randomIndex = Random.Range(0, otherTracks.Length);
            currentTrack = otherTracks[randomIndex];
            currentTrack.Play();

            // �ȴ���ǰ���첥�����
            yield return new WaitForSeconds(currentTrack.clip.length);

            // ��Ƶ������Ϻ󣬵ȴ���һ��
        }
    }

    //���Ŷ�Ӧ���Ƶ���Ƶ
    public void ClickSound(string soundName, float volume =1f)
    {
        AudioClip clip = Resources.Load<AudioClip>(soundName);
        if (clip != null)
        {
            clickSound.clip = clip;
            clickSound.volume = volume;
            clickSound.PlayOneShot(clip);
            
        }
        else
        {
            Debug.LogError($"Failed to load sound: {soundName}");
        }
    }


    public void StopPlay()
    {
        mainTrack.Stop();
        currentTrack.Stop();
        StopCoroutine("PlayRandomTrack");
        clickSound.Stop();
    }
}
