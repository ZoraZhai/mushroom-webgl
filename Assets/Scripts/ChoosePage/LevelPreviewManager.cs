using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPreviewManager : MonoBehaviour
{
    public GameObject[] levelPreviews; // 这是一个公开的数组，可以在Unity的Inspector面板中添加预览图片
    //public Image previewImage;
    //public Button levelButton;

    private int currentLevel = 0;

    public void OnLevelButtonClick(int level)
    {
        currentLevel = level;
        UpdatePreview(); 
    }

    private void UpdatePreview()
    {
        if (currentLevel >= 0 && currentLevel < levelPreviews.Length)
        {
            foreach (GameObject preview in levelPreviews)
            {
                preview.SetActive(false);
            }
            levelPreviews[currentLevel].SetActive(true);
        }
        else
        {
            Debug.LogError("Invalid level index");
        }
    }
}

