using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewQuestionData", menuName = "Question Data", order = 1)]
public class QuestionData : ScriptableObject
{
    public string question;
    public List<string> options;
    public string correctOption;

    public QuestionData(string question, List<string> options)
    {
        this.question = question;
        this.options = options;
    }
}
