using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    private Color selectedColor = new Color(164f / 255f, 144f / 255f, 133f / 255f);
    private Color defaultColor = new Color(255f / 255f, 255f / 255f, 255f / 255f);

    private Button button;
    private Button[] buttons;

    void Start()
    {
        //GetComponent<Image>().color = defaultColor;
        
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeButtonColor);
    }

    void ChangeButtonColor()
    {
        GetComponent<Image>().color = selectedColor;

        ChangeColor[] buttons = FindObjectsOfType<ChangeColor>();
        foreach (ChangeColor button in buttons)
        {
            if (button != this)
            {
                button.GetComponent<Image>().color = defaultColor;
            }
        }
    }
}
