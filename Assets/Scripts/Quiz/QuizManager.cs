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
        // ��ȡLevelManager�����ϵ�LevelManager���
        levelManager = FindObjectOfType<LevelManager1>();
    }

    private void ShowQuestion()
    {
        Debug.Log("ˢ����Ŀ");
        questionText.text = questions[currentQuestionIndex].question;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i; // ���浱ǰѡ�������
            optionButtons[i].onClick.RemoveAllListeners(); // �Ƴ�֮ǰ�ļ�����
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index)); // ����µļ�����
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
            Debug.Log("�����");
            currentQuestionIndex++;
            if (currentQuestionIndex < questions.Count)
            {
                ShowQuestion();
            }
            

        
            if (currentQuestionIndex == questions.Count)
            {
                if (correctAnswers == questions.Count)
                { // ����
                    Debug.Log("ȫ����ԣ����Ը��");
                    levelManager.hasRevived = true;
                    //���¼��عؿ�
                    levelManager.currentLevel -= 1;
                    reviveUIPanel.SetActive(false);
                    reviveButton.gameObject.SetActive(false);

                    levelManager.LoadLevel();
                    Debug.Log("������عؿ�");
                }
            }
            

        }
        else
        {
            // ��ת��ʧ��ҳ�沢���ø������
            Debug.Log("�ش������Ϸʧ�ܣ�");
            levelManager.hasRevived = true;
            reviveButton.gameObject.SetActive(false);

            reviveUIPanel.SetActive(false);

        }
    }
}
