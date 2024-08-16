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
    private const float clickDurationThreshold = 0.2f; // ���ʱ����ֵ
    private Vector2 initialTouchPosition;
    private Vector2 finalTouchPosition;

    private CinemachineVirtualCamera mainCamera;
    private CinemachineFramingTransposer framingTransposer;

    private Vector3 originalScale;
    private Coroutine bounceCoroutine;
    //private bool isBouncing = false;
    private float scaleFactor = 1.1f; // ������������ű���
    private float duration = 0.2f; // ����ʱ��

    public static Outline SelectedOutline;
    private Outline outline;//���

    private InputPinchZoom pinchZoom;

    public GameObject panelPrefab; // UI PanelԤ�Ƽ�������
    private GameObject panelInstance; // UI Panelʵ��������
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

    /*private void OnClick(InputAction.CallbackContext context) // ��������Ģ��ʱ����(�ĳ�New Input System)
    {*/
    //if (!gameObject.activeSelf || !gameObject.activeInHierarchy)
    //{
    //return;
    //}


    //���߼��
    // �����λ���Ƿ��ڵ�ǰĢ������Ϸ������
    //Vector2 mousePosition = Input.mousePosition;
    //Vector2 mousePosition = Touchscreen.current.primaryTouch.position.ReadValue();// ��ȡ��Ļ�ϵ����λ��
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

        pointerDownTime = Time.time; // ��¼������ʼ��ʱ��
        initialTouchPosition = eventData.position;

        // ��ǰĢ����Ϊ�������Ģ��
       LastClickedMushroom = this;
        //SoundManager.instance.PlaySound(Globals.S_Click);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        isPointerDown = false;
        float elapsedTime = Time.time - pointerDownTime; // ���㴥������ʱ��
        finalTouchPosition = eventData.position;
        Debug.Log(Vector2.Distance(initialTouchPosition, finalTouchPosition));
        if (elapsedTime < clickDurationThreshold && Vector3.Distance(initialTouchPosition,finalTouchPosition)<0.05f)
        {
            // ����ʱ��϶��һ�������С����Ϊ���
            OnShortClick();
        }

    }

    //public void OnPointerClick(PointerEventData eventData)
    public void OnShortClick()
    {
        Debug.Log("�������"+transform.name);
        soundManager.ClickSound("Audio/�����Ч", 0.7f);

        // ���ʱ����ǰ Outline �ű�ʵ����ֵ����̬����
        SelectedOutline = GetComponent<Outline>();
        outline.enabled = true;//��߽ű�����
        DisableOtherOutlines();//������������Outline�ű�
        pinchZoom.isMushroomFocused = true;
        //pinchZoom.enabled=false;//���÷Ŵ�ű�

        FocusCameraToMushroom();

        //ֹͣû�����ĵ���Э��
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
            Debug.Log("ֹͣ����Э��");
            transform.localScale = originalScale;

        }
        if (levelManager.hasConfirmationPanel == true)//ԭ�ص�����������һ�ε��Ģ��ת�������ͷʱ������
        {
            bounceCoroutine =StartCoroutine(BounceEffect(this.gameObject));
        }

        //�����ε��������
        if (panelInstance == null && levelManager.hasConfirmationPanel == false)
        {
            panelInstance = Instantiate(panelPrefab, FindObjectOfType<Canvas>().transform, false);
            panelInstance.SetActive(true);
            levelManager.hasConfirmationPanel = true;

            // ����UI Panel�İ�ť�߼���
            pickButton = panelInstance.transform.Find("PickButton").GetComponent<Button>();
            cancelButton = panelInstance.transform.Find("CancelButton").GetComponent<Button>();

            pickButton.onClick.AddListener(PickMushroom);
            cancelButton.onClick.AddListener(Cancel);
        }
        
    }


    public void FocusCameraToMushroom()
    {
        /*//��Э��ƽ����������л�����
        GameObject mushroom = this.gameObject;//�������Ģ��
        if (mushroom != null)
        {
            //StartCoroutine(SwitchFollowSlowMotion(mushroom.transform.position));
        }*/

        mainCamera.Follow=transform;
        framingTransposer.m_CameraDistance = levelManager.zoomInDistance;//Camera Distance ��
        framingTransposer.m_TrackedObjectOffset.y = levelManager.mushroomY;//��������߶�
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

    public void PickMushroom() // �� "��ժ" ��ť�ĵ���¼��е���
    {
        // ʹ��LastClickedMushroom��ȡ�������Ģ����Ϣ
        MushroomClicker clickedMushroom = LastClickedMushroom;
        MushroomInformation mushroomInformation = clickedMushroom.transform.GetComponent<MushroomInformation>();
        Animator mushroomAnimator = LastClickedMushroom.GetComponent<Animator>();
        //itemOnWorld itemOnWorld = clickedMushroom.transform.GetComponent<itemOnWorld>();

        // ʵ��Ģ������ժ�����Ŷ������Ƴ�Ģ��
        Destroy(panelInstance);
        levelManager.hasConfirmationPanel = false;
        //��������StartCoroutine(PickupAnimation());
        mushroomAnimator.enabled = true;
        
        soundManager.ClickSound("Audio/��ť�����Ч");


        //�ӳ���ʾui
        StartCoroutine(ShowUIPanel(mushroomInformation));
        //infoText.text = null;//�����ʧ

        pinchZoom.isMushroomFocused = false;

        //itemOnWorld.AddNewItem();//��ӵ�ͼ������
    }
    IEnumerator ShowUIPanel(MushroomInformation mushroomInformation)
    {
        yield return new WaitForSeconds(0.2f);
        soundManager.ClickSound("Audio/�ɼ�ֲ��",1.5f);
        if (mushroomInformation.isPoisonous)
        {
            //��Ļ������ɫ��Ч
            levelManager.FlashEffect();
            yield return new WaitForSeconds(1.5f);
            // gameoverUI��ʾ
            levelManager.Gameover(mushroomInformation.mushroomName, mushroomInformation.description,mushroomInformation.spriteToLoad);
            soundManager.ClickSound("Audio/���嵹��2",0.8f);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            levelManager.correctAnswersCount++;
            levelManager.currentQuestionIndex++;
            soundManager.ClickSound("Audio/�ɹ���Ч",1f);

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
    public void Cancel() // �� "ȡ��" ��ť�ĵ���¼��е���
    {
        Destroy(panelInstance);
        levelManager.hasConfirmationPanel = false;
        outline.enabled = false;
        DisableOtherOutlines();
        pinchZoom.enabled = true;//�ָ��Ŵ�ű�

        //framingTransposer.m_CameraDistance = levelManager.zoomOutDistance;
        //framingTransposer.m_TrackedObjectOffset.y = levelManager.targetY;

        levelManager.zoomOutDistance = mushroomInformation.zoomOutDistance;
        StartCoroutine(MoveCameraToDistanceAndOffset(levelManager.zoomOutDistance,0.2f));

        pinchZoom.isMushroomFocused = false;

        pickButton.onClick.RemoveListener(PickMushroom);
        cancelButton.onClick.RemoveListener(Cancel);
        soundManager.ClickSound("Audio/��ť�����Ч");
    }

    public void LoadLevel() 
    {
        levelManager.LoadLevel();
        Destroy(nextlevelpanelInstance);
    }
   
    private IEnumerator PickupAnimation()
    {
        yield return ScaleOverTime(new Vector3(transform.localScale.x, 0.5f * transform.localScale.y, transform.localScale.z), 0.5f);// ѹ������
        yield return ScaleOverTime(transform.localScale, 0.2f);// ������ԭʼ��С
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5f);// �ȴ�һ��ʱ��
        Destroy(gameObject); // ���Ģ������
        Debug.Log("Ģ�������");
        
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

        framingTransposer.m_TrackedObjectOffset.y = targetY; // ȷ������ֵ׼ȷ
    }


    private IEnumerator BounceEffect(GameObject target)
    {
        
        // ��¼ԭʼ��С
        originalScale = transform.localScale;
        // ����Ŀ���С
        Vector3 targetScale = originalScale * scaleFactor;

        // ��¼��Ч��ʼʱ��
        float startTime = Time.time;
        //while (Time.time - startTime <= duration)
        while(Vector3.Distance(transform.localScale,targetScale)>0.01f )
        {
            // ���㵱ǰ��Ч�ѳ���ʱ��ı���
            float t = (Time.time - startTime) / duration;
            // ��ԭʼ��С��Ŀ���С֮���ֵ��ʵ������
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            // �ȴ���һ֡
            yield return null;
        }

        // ����Ч
        //startTime = Time.time;
        while (Vector3.Distance(targetScale, transform.localScale ) > 0.01f)
        {
            float t = (Time.time - startTime) / duration;
            // ��Ŀ���С��ԭʼ��С֮���ֵ��ʵ������
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        // ȷ����Ч����ʱ��С�ָ�ԭ��
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
