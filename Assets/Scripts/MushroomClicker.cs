using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;

public class MushroomClicker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
//IPointerClickHandler,
{
    //private PlayerController inputActions;
    public static MushroomClicker LastClickedMushroom;

    private LevelManager1 levelManager;
    private BackgroundMusicController soundManager;
    private MushroomInformation mushroomInformation;
    private string mushroomID;

    private float zoomOutDistance;

    private bool isPointerDown = false;
    private float pointerDownTime = 0f;
    private const float clickDurationThreshold = 0.2f; // 点击时间阈值
    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;

    private CinemachineVirtualCamera mainCamera;
    private CinemachineFramingTransposer framingTransposer;

    private Vector3 originalScale;
    private Coroutine bounceCoroutine;
    //private bool isBouncing = false;
    private float scaleFactor = 1.1f; // 点击弹动的缩放倍数
    private float duration = 0.2f; // 动画时长

    public static Outline SelectedOutline;
    private Outline outline;//描边

    private InputPinchZoom pinchZoom;

    public GameObject panelPrefab; // UI Panel预制件的引用
    private GameObject panelInstance; // UI Panel实例的引用
    private GameObject nextlevelpanelPrefab;
    private GameObject nextlevelpanelInstance;
    private Button pickButton;
    private Button cancelButton;

    /*public void Awake()
    {
        inputActions = new PlayerController();
        inputActions.Player.Click.performed += OnClick;

        inputActions.Enable();
    }



    private void OnDisable()
    {
        inputActions.Player .Click.performed -= OnClick;
        inputActions.Disable();
    }*/

    void Start()
    {
        GameObject cameraObject = GameObject.FindWithTag("MainCamera");
        if (cameraObject != null)
        {
            mainCamera = cameraObject.GetComponent<CinemachineVirtualCamera>();
            framingTransposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            pinchZoom = mainCamera.GetComponent<InputPinchZoom>();
        }
        else
        {
            Debug.LogWarning("Main camera not found.");
        }

        outline = GetComponent<Outline>();

        GameObject ScriptManager = GameObject.Find("LevelManager");
        levelManager = ScriptManager.GetComponent<LevelManager1>();
        soundManager = ScriptManager.GetComponent<BackgroundMusicController>();

        //infoText = GameObject.Find("InfoText").GetComponent<TextMeshProUGUI>();

    }

    /*private void OnClick(InputAction.CallbackContext context) // 当点击这个蘑菇时调用(改成New Input System)
    {*/
    //if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
    //{
    //return;
    //}


    //射线检测
    // 检查点击位置是否在当前蘑菇的游戏对象上
    //Vector2 mousePosition = Input.mousePosition;
    //Vector2 mousePosition = Touchscreen.current.primaryTouch.position.ReadValue();// 获取屏幕上的鼠标位置
    //Vector2 mousePosition = context.ReadValue<Vector2>();
    /*if (Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out RaycastHit hit))
    {
        if (hit.collider.gameObject == this.gameObject)
        {

           */

    public void OnPointerDown(PointerEventData eventData)
    {
        mushroomInformation = transform.GetComponent<MushroomInformation>();
        isPointerDown = true;

        pointerDownTime = Time.time; // 记录触摸开始的时间
        initialTouchPosition = eventData.position;

        // 当前蘑菇成为最后点击的蘑菇
       LastClickedMushroom = this;
        //SoundManager.instance.PlaySound(Globals.S_Click);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        isPointerDown = false;
        float elapsedTime = Time.time - pointerDownTime; // 计算触摸持续时间
        finalTouchPosition = eventData.position;
        Debug.Log(Vector2.Distance(initialTouchPosition, finalTouchPosition));
        if (elapsedTime < clickDurationThreshold && Vector3.Distance(initialTouchPosition,finalTouchPosition)<0.05f)
        {
            // 触摸时间较短且滑动距离小，视为点击
            OnShortClick();
        }

    }

    //public void OnPointerClick(PointerEventData eventData)
    public void OnShortClick()
    {
        Debug.Log("点击到了"+transform.name);
        soundManager.ClickSound("Audio/点击音效", 0.7f);

        // 点击时将当前 Outline 脚本实例赋值给静态变量
        SelectedOutline = GetComponent<Outline>();
        outline.enabled = true;//描边脚本激活
        DisableOtherOutlines();//禁用其他物体Outline脚本
        pinchZoom.isMushroomFocused = true;
        //pinchZoom.enabled=false;//禁用放大脚本

        FocusCameraToMushroom();

        //停止没结束的弹动协程
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
            Debug.Log("停止弹动协程");
            transform.localScale = originalScale;

        }
        if (levelManager.hasConfirmationPanel == true)//原地弹动动画（第一次点击蘑菇转换相机镜头时不弹）
        {
            bounceCoroutine =StartCoroutine(BounceEffect(this.gameObject));
        }

        //避免多次点击出问题
        if (panelInstance == null && levelManager.hasConfirmationPanel == false)
        {
            panelInstance = Instantiate(panelPrefab, FindObjectOfType<Canvas>().transform, false);
            panelInstance.SetActive(true);
            levelManager.hasConfirmationPanel = true;

            // 设置UI Panel的按钮逻辑等
            pickButton = panelInstance.transform.Find("PickButton").GetComponent<Button>();
            cancelButton = panelInstance.transform.Find("CancelButton").GetComponent<Button>();

            pickButton.onClick.AddListener(PickMushroom);
            cancelButton.onClick.AddListener(Cancel);
        }
        
    }


    public void FocusCameraToMushroom()
    {
        /*//用协程平滑过渡相机切换对象
        GameObject mushroom = this.gameObject;//被点击的蘑菇
        if (mushroom != null)
        {
            //StartCoroutine(SwitchFollowSlowMotion(mushroom.transform.position));
        }*/

        mainCamera.Follow=transform;
        framingTransposer.m_CameraDistance = levelManager.zoomInDistance;//Camera Distance 变
        framingTransposer.m_TrackedObjectOffset.y = levelManager.mushroomY;//调整相机高度
    }



    public void DisableOtherOutlines()
    {
        GameObject[] mushrooms = GameObject.FindGameObjectsWithTag("mushroom");
        foreach (GameObject mushroom in mushrooms)
        {
            if(mushroom != gameObject)
            {
                Outline outline = mushroom.GetComponent<Outline>();
                outline.enabled = false;

            }
        }
    }

    public void PickMushroom() // 在 "采摘" 按钮的点击事件中调用
    {
        // 使用LastClickedMushroom获取最后点击的蘑菇信息
        MushroomClicker clickedMushroom = LastClickedMushroom;
        MushroomInformation mushroomInformation = clickedMushroom.transform.GetComponent<MushroomInformation>();
        Animator mushroomAnimator = LastClickedMushroom.GetComponent<Animator>();
        //itemOnWorld itemOnWorld = clickedMushroom.transform.GetComponent<itemOnWorld>();

        // 实现蘑菇被采摘，播放动画、移除蘑菇
        Destroy(panelInstance);
        levelManager.hasConfirmationPanel = false;
        //弹动动画StartCoroutine(PickupAnimation());
        mushroomAnimator.enabled = true;
        
        soundManager.ClickSound("Audio/按钮点击音效");


        //延迟显示ui
        StartCoroutine(ShowUIPanel(mushroomInformation));
        //infoText.text = null;//题干消失

        pinchZoom.isMushroomFocused = false;

        //itemOnWorld.AddNewItem();//添加到图鉴背包
    }
    IEnumerator ShowUIPanel(MushroomInformation mushroomInformation)
    {
        yield return new WaitForSeconds(0.2f);
        soundManager.ClickSound("Audio/采集植物",1.5f);
        if (mushroomInformation.isPoisonous)
        {
            //屏幕闪动红色特效
            levelManager.FlashEffect();
            yield return new WaitForSeconds(1.5f);
            // gameoverUI显示
            levelManager.Gameover(mushroomInformation.mushroomName, mushroomInformation.description,mushroomInformation.spriteToLoad);
            soundManager.ClickSound("Audio/身体倒地2",0.8f);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            levelManager.correctAnswersCount++;
            levelManager.currentQuestionIndex++;
            soundManager.ClickSound("Audio/成功音效",1f);

            if (levelManager.currentLevel < levelManager.totalLevel)
            {
                if (levelManager != null)
                {
                    levelManager.DisplayNextLevel(mushroomInformation.mushroomName, mushroomInformation.description, mushroomInformation.spriteToLoad);
                }
            }
            else
            {
                levelManager.DisplayNextQuestion();
            }
        }
    }
    public void Cancel() // 在 "取消" 按钮的点击事件中调用
    {
        Destroy(panelInstance);
        levelManager.hasConfirmationPanel = false;
        outline.enabled = false;
        DisableOtherOutlines();
        pinchZoom.enabled = true;//恢复放大脚本

        //framingTransposer.m_CameraDistance = levelManager.zoomOutDistance;
        //framingTransposer.m_TrackedObjectOffset.y = levelManager.targetY;

        levelManager.zoomOutDistance = mushroomInformation.zoomOutDistance;
        StartCoroutine(MoveCameraToDistanceAndOffset(levelManager.zoomOutDistance,0.2f));

        pinchZoom.isMushroomFocused = false;

        pickButton.onClick.RemoveListener(PickMushroom);
        cancelButton.onClick.RemoveListener(Cancel);
        soundManager.ClickSound("Audio/按钮点击音效");
    }

    public void LoadLevel() 
    {
        levelManager.LoadLevel();
        Destroy(nextlevelpanelInstance);
    }
   
    private IEnumerator PickupAnimation()
    {
        yield return ScaleOverTime(new Vector3(transform.localScale.x, 0.5f * transform.localScale.y, transform.localScale.z), 0.5f);// 压缩动画
        yield return ScaleOverTime(transform.localScale, 0.2f);// 弹动回原始大小
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5f);// 等待一段时间
        Destroy(gameObject); // 清除蘑菇对象
        Debug.Log("蘑菇被清除");
        
    }
    private IEnumerator ScaleOverTime(Vector3 targetScale, float duration)
    {
        Vector3 originalScale = transform.localScale;
        float time = 0;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, (time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
    }
    private IEnumerator MoveCameraToDistanceAndOffset(float distance, float duration)
    {
        float currentDistance = framingTransposer.m_CameraDistance;
        float currentOffsetY = framingTransposer.m_TrackedObjectOffset.y;
        float elapsedTime = 0f;

        while (elapsedTime<duration )
        {
            float t = elapsedTime / duration;

            currentDistance = UnityEngine.Mathf.Lerp(currentDistance, distance, t * levelManager.dampingSpeed);
            currentOffsetY = UnityEngine.Mathf.Lerp(currentOffsetY, levelManager.targetY, t * levelManager.dampingSpeed);

            framingTransposer.m_CameraDistance = currentDistance;
            framingTransposer.m_TrackedObjectOffset.y = currentOffsetY;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        framingTransposer.m_CameraDistance = distance;
        framingTransposer.m_TrackedObjectOffset.y = levelManager.targetY;
    }


    private IEnumerator MoveCameraToDistance(float distance)
    {
        float currentDistance = framingTransposer.m_CameraDistance;
        float elapsedTime = 0f;

        while (Mathf.Abs(currentDistance - distance) > 0.01f)
        {
            currentDistance = UnityEngine.Mathf.Lerp(currentDistance, distance, elapsedTime*levelManager.dampingSpeed);
            framingTransposer.m_CameraDistance = currentDistance;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        framingTransposer.m_CameraDistance = distance;
        

    }
    private IEnumerator ChangeTrackedObjectOffsetYOverTime(float targetY, float duration)
    {
        float elapsedTime = 0f;
        float startY = framingTransposer.m_TrackedObjectOffset.y;

        while (elapsedTime < duration)
        {
            framingTransposer.m_TrackedObjectOffset.y = Mathf.Lerp(startY, targetY, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        framingTransposer.m_TrackedObjectOffset.y = targetY; // 确保最终值准确
    }


    private IEnumerator BounceEffect(GameObject target)
    {
        
        // 记录原始大小
        originalScale = transform.localScale;
        // 计算目标大小
        Vector3 targetScale = originalScale * scaleFactor;

        // 记录动效开始时间
        float startTime = Time.time;
        //while (Time.time - startTime <= duration)
        while(Vector3.Distance(transform.localScale,targetScale)>0.01f )
        {
            // 计算当前动效已持续时间的比例
            float t = (Time.time - startTime) / duration;
            // 在原始大小和目标大小之间插值，实现缩放
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            // 等待下一帧
            yield return null;
        }

        // 反向动效
        //startTime = Time.time;
        while (Vector3.Distance(targetScale, transform.localScale ) > 0.01f)
        {
            float t = (Time.time - startTime) / duration;
            // 在目标大小和原始大小之间插值，实现缩放
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        // 确保动效结束时大小恢复原样
        transform.localScale = originalScale;

        yield return new WaitForSeconds(0.5f); 
        //
    }

    private IEnumerator SwitchFollowSlowMotion(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Transform originalFollowTarget = mainCamera.Follow;
        float originalCameraDistance = framingTransposer.m_CameraDistance;
        float originalTrackedObjectOffsetY = framingTransposer.m_TrackedObjectOffset.y;

        while (elapsedTime < levelManager.switchDuration)
        {
            mainCamera.Follow.position = Vector3.Lerp(originalFollowTarget.position, targetPosition, elapsedTime / duration);
            framingTransposer.m_CameraDistance = Mathf.Lerp(originalCameraDistance, levelManager.zoomInDistance, elapsedTime / duration);
            framingTransposer.m_TrackedObjectOffset.y = Mathf.Lerp(originalTrackedObjectOffsetY, levelManager.mushroomY, elapsedTime / duration);

            elapsedTime += Time.deltaTime / levelManager.switchDuration;
            yield return null;
        }
        mainCamera.transform.position = targetPosition;
        framingTransposer.m_CameraDistance = levelManager.zoomInDistance;
        framingTransposer.m_TrackedObjectOffset.y = levelManager.mushroomY;
    }
}
