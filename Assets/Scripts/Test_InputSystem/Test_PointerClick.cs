using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Test_IPointerClick : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
    //IBeginDragHandler, IEndDragHandler
{
    //private bool isDragging;

    /*public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        Debug.Log("结束滑动");
    }*/
    private bool isPointerDown = false;
    private float pointerDownTime = 0f;
    private const float clickDurationThreshold = 0.2f; // 点击时间阈值

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTime = Time.time; // 记录触摸开始的时间
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        float elapsedTime = Time.time - pointerDownTime; // 计算触摸持续时间

        if (elapsedTime < clickDurationThreshold)
        {
            // 触摸时间较短，视为点击
            OnShortClick();
        }
    }

    private void OnShortClick()
    {
        // 处理点击逻辑
        Debug.Log("Short click detected.");
    }

}
