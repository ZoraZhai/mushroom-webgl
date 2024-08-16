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

    //������Ƶ������·�����audioclip�ļ�����Ƶ�ļ�·����Resources�ļ�����
    public AudioClip LoadAudio(string path)
    {
        return (AudioClip)Resources.Load(path);
    }

    //Ԥ������Ƶ���ֵ���
    private AudioClip GetAudio(string path)
    {
        Debug.Log("����audioclip");
        if (!dictAudio.ContainsKey(path)) 
        {
            dictAudio[path] = LoadAudio(name);
        }
        return dictAudio[path];
    }
    //������Ч
    public void PlayBGM(string name)
    {
        Debug.Log("����playbgm");
        audioSource.clip = GetAudio(name);
        
        this.audioSource.Play();
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }

    public void PlaySound(string path, float volume = 1f)
    {
        //���Ӳ���
        this.audioSource.PlayOneShot(GetAudio(path),volume);
        //audioSource.volume = volume;
    }


}
