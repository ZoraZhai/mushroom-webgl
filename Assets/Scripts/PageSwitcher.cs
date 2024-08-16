using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PageSwitcher : MonoBehaviour
{
    public GameObject loadingScreen; // 加载中UI界面
    public Slider loadingBar; // 加载进度条
    public Text progressText;

    public void SwitchToPage(string pageName)
    {
        StartCoroutine(LoadPageAsync(pageName));
    }

    IEnumerator LoadPageAsync(string pageName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(pageName);

        loadingScreen.SetActive(true); // 显示加载中UI界面

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 获取加载进度（范围：0-1）
            loadingBar.value = progress; // 更新加载进度条
            progressText.text = Mathf.RoundToInt(progress * 100) + "%"; // 更新加载进度数字

            yield return null;
        }
    }
}
