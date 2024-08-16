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
        Debug.Log("��������");
    }*/
    private bool isPointerDown = false;
    private float pointerDownTime = 0f;
    private const float clickDurationThreshold = 0.2f; // ���ʱ����ֵ

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTime = Time.time; // ��¼������ʼ��ʱ��
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        float elapsedTime = Time.time - pointerDownTime; // ���㴥������ʱ��

        if (elapsedTime < clickDurationThreshold)
        {
            // ����ʱ��϶̣���Ϊ���
            OnShortClick();
        }
    }

    private void OnShortClick()
    {
        // �������߼�
        Debug.Log("Short click detected.");
    }

}
