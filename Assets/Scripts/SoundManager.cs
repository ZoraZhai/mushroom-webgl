using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource audioSource;
    private Dictionary<string, AudioClip> dictAudio;

    private void Awake()
    {
        instance = new SoundManager();
        audioSource = GetComponent<AudioSource>();
        dictAudio = new Dictionary<string, AudioClip>();
    }

    //加载音频，传入路径输出audioclip文件，音频文件路径在Resources文件夹下
    public AudioClip LoadAudio(string path)
    {
        return (AudioClip)Resources.Load(path);
    }

    //预加载音频到字典中
    private AudioClip GetAudio(string path)
    {
        Debug.Log("加载audioclip");
        if (!dictAudio.ContainsKey(path)) 
        {
            dictAudio[path] = LoadAudio(name);
        }
        return dictAudio[path];
    }
    //播放音效
    public void PlayBGM(string name)
    {
        Debug.Log("运行playbgm");
        audioSource.clip = GetAudio(name);
        
        this.audioSource.Play();
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }

    public void PlaySound(string path, float volume = 1f)
    {
        //叠加播放
        this.audioSource.PlayOneShot(GetAudio(path),volume);
        //audioSource.volume = volume;
    }


}
