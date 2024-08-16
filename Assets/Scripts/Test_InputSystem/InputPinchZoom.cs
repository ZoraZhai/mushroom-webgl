using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputPinchZoom : MonoBehaviour
{
    private CinemachineVirtualCamera mainCamera;
    private CinemachineFramingTransposer framingTransposer;
    public float zoomSpeed = 1f; // �����ٶ�
    
    private Vector2 touch0StartPos, touch1StartPos; // ��ʼ������λ��
    private float initialDistance; // ��ʼ��ָ���
    public float currentCameraDistance;
    private float velocity = 0f;
    public Image hintImage;
    private int hintCount = 0;
    public bool isLocked = false;

    public bool isMushroomFocused = false;
    public static bool isZooming = false;
    [SerializeField] float minZ = 0.41f; // ��С����
    [SerializeField] float maxZ = 3f; // ��󽹾�


    void Awake()
    {
        // ��ȡCinemachine Frametransposer���
        GameObject cameraObject = GameObject.FindWithTag("MainCamera");
        if (cameraObject != null)
        {
            mainCamera = cameraObject.GetComponent<CinemachineVirtualCamera>();
            framingTransposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

    }

    void Update()
    {
        // �������������
        // ʹ��Touchscreen.current.touches����ȡ��ǰ�����д�����
        var touches = Touchscreen.current.touches;

        // ��ʼ��������
        int activeTouches = 0;

        // ����touches���飬���㴦�ڻ״̬�Ĵ���������
        foreach (var touch in touches)
        {
            // Check if the touch is in progress
            if (touch.isInProgress)
            {
                activeTouches++;
            }
        }
        if (touches[0].isInProgress)
        {
            StartCoroutine(EnableZoom());
        }

        // ����Ƿ�ͬʱ����������
        if (activeTouches >= 2 && !isZooming)
        {
            // ��ʼ����
            isZooming = true;
            //��ʼ����¼���Ų���
            touch0StartPos = touches[0].startPosition.ReadValue();
            touch1StartPos = touches[1].startPosition.ReadValue();
            initialDistance = Vector2.Distance(touch0StartPos, touch1StartPos);
            currentCameraDistance = framingTransposer.m_CameraDistance;
        }

        if (!touches[1].isInProgress && isZooming)
        {
            StartCoroutine(DisableZoom());

        }


        if (isZooming && !isMushroomFocused)
        {
            // ��������
            var touch0Pos = touches[0].position.ReadValue();
            var touch1Pos = touches[1].position.ReadValue(); ;
            var currentDistance = Vector2.Distance(touch0Pos, touch1Pos);
            var scaleFactor = initialDistance / currentDistance;//�������ű���

            velocity = currentCameraDistance * scaleFactor * zoomSpeed;//���ݱ����������
            velocity = Mathf.Clamp(velocity, minZ, maxZ);//���ƾ����ڼ�ֵ��Χ��
            framingTransposer.m_CameraDistance = velocity;//set�����Cinemachine��distance

            
            if (framingTransposer.m_CameraDistance == minZ && currentDistance > initialDistance)//���ڷŴ����
            {
                if (hintCount <2 )
                {
                    StartCoroutine(HintAppear());
                    hintCount++;
                    isLocked = true;
                }
                
            }
        }



        }

        IEnumerator HintAppear()
    {
        yield return new WaitForSeconds(0.5f);
        hintImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        hintImage.gameObject.SetActive(false);
        
    }
    IEnumerator EnableZoom()
    {
        yield return new WaitForSecondsRealtime(0.02f);
    }
    IEnumerator DisableZoom()
    {
        yield return new WaitForSecondsRealtime(0.1f);//�ӳ�0.1f�˳�����״̬
        isZooming = false;
    }
}



