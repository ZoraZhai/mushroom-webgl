using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BackgroundMusicController : MonoBehaviour
{
    public AudioSource mainTrack; // 主音轨
    public AudioSource[] otherTracks; // 其他音轨数组
    public AudioSource clickSound;//点击音效
    public AudioSource pickSound;//采摘音效

    private AudioSource currentTrack; // 当前播放的其他音轨

    private void Start()
    {
        PlayBGM();
    }
    public void PlayBGM()
    {
        // 开始播放主音轨
        mainTrack.Play();
        // 开始随机播放其他音轨
        StartCoroutine(PlayRandomTrack());
    }

    private IEnumerator PlayRandomTrack()
    {
        while (true)
        {
            // 随机选择一段音频播放
            int randomIndex = Random.Range(0, otherTracks.Length);
            currentTrack = otherTracks[randomIndex];
            currentTrack.Play();

            // 等待当前音轨播放完毕
            yield return new WaitForSeconds(currentTrack.clip.length);

            // 音频播放完毕后，等待下一轮
        }
    }

    //播放对应名称的音频
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
