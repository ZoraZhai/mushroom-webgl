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
        /*if (!isHoldDetected || (Time.time - holdStartTime) < 0.2f) //长按时无法点击，同时点击时间很短
        { */
            // 获取触摸位置
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            // 将触摸位置转换为世界空间中的射线
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            // 进行射线投射
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log($"Hit {hit.collider.gameObject.name}");
                // 在这里添加更多的处理逻辑，例如对被射中的对象做些什么
/*            }*/
        }
    }

    /*private void OnHoldStarted(InputAction.CallbackContext context)
    {
        holdStartTime = Time.time;
        Debug.Log("尝试长按");
        isHoldDetected = true;
    }
    private void OnHoldEnded(InputAction.CallbackContext context)
    {
        if ((Time.time - holdStartTime) >= 0.5f) // 假定大于等于0.5秒的按压为长按
        {
            isHoldDetected = true;
            Debug.Log("长按确认");
        }
        else
        {
            isHoldDetected = false;
            Debug.Log("长按取消");
        }

    }*/

}
