using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PageSwitcher : MonoBehaviour
{
    public GameObject loadingScreen; // ������UI����
    public Slider loadingBar; // ���ؽ�����
    public Text progressText;

    public void SwitchToPage(string pageName)
    {
        StartCoroutine(LoadPageAsync(pageName));
    }

    IEnumerator LoadPageAsync(string pageName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(pageName);

        loadingScreen.SetActive(true); // ��ʾ������UI����

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // ��ȡ���ؽ��ȣ���Χ��0-1��
            loadingBar.value = progress; // ���¼��ؽ�����
            progressText.text = Mathf.RoundToInt(progress * 100) + "%"; // ���¼��ؽ�������

            yield return null;
        }
    }
}
