using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchRaycaster : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction click;
    private InputAction hold;
    //private bool isHoldDetected = false;
    private float holdStartTime;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        click = playerInput.actions["Click"];
        hold = playerInput.actions["Hold"];
    }
    private void OnEnable()
    {
        //hold.started += OnHoldStarted;
        //hold.canceled += OnHoldEnded;
        click.performed += OnClickPerformed;


    }

    private void OnDisable()
    {
        //hold.performed -= OnHoldStarted;
        //hold.canceled -= OnHoldEnded;
        click.performed -= OnClickPerformed;

    }


    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        /*if (!isHoldDetected || (Time.time - holdStartTime) < 0.2f) //����ʱ�޷������ͬʱ���ʱ��ܶ�
        { */
            // ��ȡ����λ��
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            // ������λ��ת��Ϊ����ռ��е�����
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            // ��������Ͷ��
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"Hit {hit.collider.gameObject.name}");
                // ��������Ӹ���Ĵ����߼�������Ա����еĶ�����Щʲô
/*            }*/
        }
    }

    /*private void OnHoldStarted(InputAction.CallbackContext context)
    {
        holdStartTime = Time.time;
        Debug.Log("���Գ���");
        isHoldDetected = true;
    }
    private void OnHoldEnded(InputAction.CallbackContext context)
    {
        if ((Time.time - holdStartTime) >= 0.5f) // �ٶ����ڵ���0.5��İ�ѹΪ����
        {
            isHoldDetected = true;
            Debug.Log("����ȷ��");
        }
        else
        {
            isHoldDetected = false;
            Debug.Log("����ȡ��");
        }

    }*/

}
