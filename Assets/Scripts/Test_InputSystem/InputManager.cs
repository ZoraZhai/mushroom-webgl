using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Action<Vector2, Vector2> pinchStarted;
    public Action<Vector2, Vector2> pinchChanged;
    public Action pinchEnd;

    private PlayerController playerController;//Input System的配置
    private Coroutine _pinchCoroutine;

    private void Awake()
    {
        playerController = new PlayerController();
    }

    private void OnEnable()
    {
        playerController.Enable();
    }
    private void OnDisable()
    {
        playerController.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        //设置监听
        playerController.PinchZoom.SecondaryTouchContact.started += _ => PinchStart();//当第二个触摸点接触时

        playerController.PinchZoom.SecondaryTouchContact.canceled += _ => PinchEnd();//当第二个触摸点被取消时（即用户停止了捏合手势）
        
    }


    private void PinchStart()
    {
        Debug.Log("第二个手指触摸");
        pinchStarted?.Invoke(playerController.PinchZoom.PrimaryFingerPosition.ReadValue<Vector2>(), playerController.PinchZoom.SecondaryFingerPosition.ReadValue<Vector2>());
        _pinchCoroutine = StartCoroutine(PinchDetection());

    }

    private void PinchEnd()
    {
        Debug.Log("第二个手指取消触摸");
        // 停止协程
        if (_pinchCoroutine != null)
        {
            StopCoroutine(_pinchCoroutine);
            _pinchCoroutine = null; // 将引用置空，确保之后可以检查是否协程正在运行
        }
    }

    IEnumerator PinchDetection()
    {
        while (true)
        {
            if (Input.touchCount == 2)
            {
                pinchChanged?.Invoke(playerController.PinchZoom.PrimaryFingerPosition.ReadValue<Vector2>(), playerController.PinchZoom.SecondaryFingerPosition.ReadValue<Vector2>());
            }

            yield return null;
        }
    }

}
