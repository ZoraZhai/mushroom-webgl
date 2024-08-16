using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Cinemachine;


public class QuizManager : MonoBehaviour
{
    public List<QuestionData> questions;
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public GameObject reviveUIPanel;
    public Button reviveButton;

    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;

    private LevelManager1 levelManager;

    void Start()
    {
        ShowQuestion();
        // 获取LevelManager物体上的LevelManager组件
        levelManager = FindObjectOfType<LevelManager1>();
    }

    private void ShowQuestion()
    {
        Debug.Log("刷新题目");
        questionText.text = questions[currentQuestionIndex].question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i; // 保存当前选项的索引
            optionButtons[i].onClick.RemoveAllListeners(); // 移除之前的监听器
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index)); // 添加新的监听器
            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = questions[currentQuestionIndex].options[i];
        }
    }

    private void OnOptionSelected(int optionIndex)
    {
        string selectedOption = optionIndex.ToString();
        CheckAnswer(selectedOption);
    }

    public void CheckAnswer(string selectedOption)
    {
        if (selectedOption == questions[currentQuestionIndex].correctOption)
        {
            correctAnswers++;
            Debug.Log("答对了");
            currentQuestionIndex++;
            if (currentQuestionIndex < questions.Count)
            {
                ShowQuestion();
            }
            

        
            if (currentQuestionIndex == questions.Count)
            {
                if (correctAnswers == questions.Count)
                { // 复活
                    Debug.Log("全部答对，可以复活！");
                    levelManager.hasRevived = true;
                    //重新加载关卡
                    levelManager.currentLevel -= 1;
                    reviveUIPanel.SetActive(false);
                    reviveButton.gameObject.SetActive(false);

                    levelManager.LoadLevel();
                    Debug.Log("复活加载关卡");
                }
            }
            

        }
        else
        {
            // 跳转到失败页面并禁用复活机会
            Debug.Log("回答错误，游戏失败！");
            levelManager.hasRevived = true;
            reviveButton.gameObject.SetActive(false);

            reviveUIPanel.SetActive(false);

        }
    }
}
