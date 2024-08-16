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
    public float zoomSpeed = 1f; // 缓冲速度
    
    private Vector2 touch0StartPos, touch1StartPos; // 开始触摸的位置
    private float initialDistance; // 初始两指间距
    public float currentCameraDistance;
    private float velocity = 0f;
    public Image hintImage;
    private int hintCount = 0;
    public bool isLocked = false;

    public bool isMushroomFocused = false;
    public static bool isZooming = false;
    [SerializeField] float minZ = 0.41f; // 最小焦距
    [SerializeField] float maxZ = 3f; // 最大焦距


    void Awake()
    {
        // 获取Cinemachine Frametransposer组件
        GameObject cameraObject = GameObject.FindWithTag("MainCamera");
        if (cameraObject != null)
        {
            mainCamera = cameraObject.GetComponent<CinemachineVirtualCamera>();
            framingTransposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

    }

    void Update()
    {
        // 检测两个触摸点
        // 使用Touchscreen.current.touches来获取当前的所有触摸点
        var touches = Touchscreen.current.touches;

        // 初始化计数器
        int activeTouches = 0;

        // 遍历touches数组，计算处于活动状态的触摸点数量
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

        // 检查是否同时有两个触点
        if (activeTouches >= 2 && !isZooming)
        {
            // 开始缩放
            isZooming = true;
            //初始化记录缩放参数
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
            // 正在缩放
            var touch0Pos = touches[0].position.ReadValue();
            var touch1Pos = touches[1].position.ReadValue(); ;
            var currentDistance = Vector2.Distance(touch0Pos, touch1Pos);
            var scaleFactor = initialDistance / currentDistance;//计算缩放比例

            velocity = currentCameraDistance * scaleFactor * zoomSpeed;//依据比例计算距离
            velocity = Mathf.Clamp(velocity, minZ, maxZ);//限制距离在极值范围内
            framingTransposer.m_CameraDistance = velocity;//set距离给Cinemachine的distance

            
            if (framingTransposer.m_CameraDistance == minZ && currentDistance > initialDistance)//正在放大到最大
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
        yield return new WaitForSecondsRealtime(0.1f);//延迟0.1f退出缩放状态
        isZooming = false;
    }
}



