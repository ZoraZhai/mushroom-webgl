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

    private PlayerController playerController;//Input System������
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
        //���ü���
        playerController.PinchZoom.SecondaryTouchContact.started += _ => PinchStart();//���ڶ���������Ӵ�ʱ

        playerController.PinchZoom.SecondaryTouchContact.canceled += _ => PinchEnd();//���ڶ��������㱻ȡ��ʱ�����û�ֹͣ��������ƣ�
        
    }


    private void PinchStart()
    {
        Debug.Log("�ڶ�����ָ����");
        pinchStarted?.Invoke(playerController.PinchZoom.PrimaryFingerPosition.ReadValue<Vector2>(), playerController.PinchZoom.SecondaryFingerPosition.ReadValue<Vector2>());
        _pinchCoroutine = StartCoroutine(PinchDetection());

    }

    private void PinchEnd()
    {
        Debug.Log("�ڶ�����ָȡ������");
        // ֹͣЭ��
        if (_pinchCoroutine != null)
        {
            StopCoroutine(_pinchCoroutine);
            _pinchCoroutine = null; // �������ÿգ�ȷ��֮����Լ���Ƿ�Э����������
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
