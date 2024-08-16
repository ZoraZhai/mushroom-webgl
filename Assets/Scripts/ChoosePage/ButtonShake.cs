using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFloatingHint : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float moveRange = 5f;
    public float waitTime = 1f;

    
    public List<GameObject> gameObjects;
    private bool[] isFloating = new bool[3];

    void Start()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            isFloating[i] = i != 2; // 默认所有按钮浮动，除了第三个
            Vector3 originalPos = gameObjects[i].transform.position;
            StartCoroutine(FloatingHint(gameObjects[i], originalPos, isFloating[i]));
        }
    }


    void Update()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (Input.GetButtonDown($"Level{i+1}Button")) // 按钮的点击事件????
            {
                isFloating[i] = !isFloating[i]; // 切换浮动状态
            }
        }
    }

    IEnumerator FloatingHint(GameObject img,Vector3 originalPos, bool shouldFloat)
    {
        

        while (shouldFloat)
        {
            float timer = 0f;
            // 上升阶段
            while (timer < Mathf.PI)
            {
                timer += Time.deltaTime / waitTime;
                float y = Mathf.Sin(timer) * moveRange;
                img.transform.position = originalPos + new Vector3(0, y, 0);
                yield return null;
            }

            // 下降阶段
            while (timer < Mathf.PI * 2)
            {
                timer += Time.deltaTime / waitTime;
                float y = Mathf.Sin(timer) * moveRange;
                img.transform.position = originalPos + new Vector3(0, y, 0);
                yield return null;
            }

            timer = 0f;
        }
    }
}
