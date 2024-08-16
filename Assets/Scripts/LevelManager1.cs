using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

/*
[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int numberOfBoxes;
}
*/
public class LevelManager1 : MonoBehaviour
{

    //程序包内实例化蘑菇
    public List<GameObject> mushroomPrefab;

    [Header("关卡信息")]
    public List<Vector3> mushroomPlaceholder;
    public int currentLevel = 0; // 当前关卡索引
    public int totalLevel = 3;//总共关卡数目
    public int numberOfBoxes = 5;//关卡对应几个盒子(后期是否可以用数据结构管理?)
    //public float radius = 0.1f;//随机生成盒子位置的半径

    public GameObject center;
    public TextMeshProUGUI levelText; // UI Text元素的引用
    //public TextMeshProUGUI infoText;

    [Header("加载打包资源")]
    public string[] mushroomLabel;

    private List<GameObject> currenLevelMushrooms = new List<GameObject>();
    private AsyncOperationHandle<IList<GameObject>> mushroomHandle;
    private AsyncOperationHandle<IList<GameObject>> mushroomNextLevelHandle;
    private GameObject[] cubes;
    public GameObject boxPrefab;

    [Header("UI")]
    private Coroutine flashCoroutine;
    public float intensity = 0f;
    public Material flashMaterial;
    private bool isIncreasing = true;

    public int currentQuestionIndex = 0;
    public int correctAnswersCount = 0;
    public GameObject nextLevelUIPrefab;
    public GameObject gameOverUIPrefab;
    public GameObject revieveUIPrefab;
    public Button reviveButton;
    public GameObject endingUIPanel;
    public Image nextLevelImage;
    public Image gameOverImage;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI mushroomInfoText;
    public TextMeshProUGUI gameOverNameText;
    public TextMeshProUGUI gameOverInfoText;

    private CinemachineVirtualCamera mainCamera;
    private CinemachineFramingTransposer framingTransposer;


    [Header("相机焦距")]
    public float zoomOutDistance = 1f;
    public float zoomInDistance = 0.41f;

    public float switchDuration = 0.7f;//点击不同蘑菇之间的切换协程速度
    public float dampingSpeed = 0.3f;
    public float targetY = 0.14f;//相机Y轴偏移
    public float mushroomY = 0.11f;

    [Header("关卡状态")]
    public bool hasRevived = false;
    public bool hasConfirmationPanel = false;

    public GameObject flashEffect;
    private Animator flashAnimator;
    private int currentLoopCount = 0;
    
    void Start()
    {
        GameObject cameraObject = GameObject.FindWithTag("MainCamera");
        mainCamera = cameraObject.GetComponent<CinemachineVirtualCamera>();
        framingTransposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        InputPinchZoom pinchZoom = mainCamera.GetComponent<InputPinchZoom>();
        flashAnimator = flashEffect.GetComponent<Animator>();
        if (pinchZoom != null)
        {
            pinchZoom.enabled = true;
        }
        //SoundManager.instance.PlayBGM(Globals.BGM2);
        LoadLevel();
    }

    public void OnSceneObjectClick(PointerEventData eventData)
    {
        if (!EventSystem.current.IsPointerOverGameObject(eventData.pointerId) && !eventData.pointerCurrentRaycast.gameObject.CompareTag("mushroom"))
        {
            mainCamera.Follow = center.transform;
        }
    }

    public void LoadLevel()
    {
        //LevelData level = levels[levelIndex];
        ClearLevel();//清空关卡
        CreateBoxes(numberOfBoxes);//新建空盒子（带有标签Cube）
        MushroomLoad();

        nextLevelUIPrefab.SetActive(false);//关闭蘑菇信息卡
        gameOverUIPrefab.SetActive(false);
        revieveUIPrefab.SetActive(false);
        endingUIPanel.SetActive(false);

        flashMaterial.SetFloat("_FullScreenIntensity", 0);

        if (flashCoroutine != null) // 如果协程正在运行，停止协程
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        //初始化视角
        framingTransposer.m_CameraDistance = zoomOutDistance;
        mainCamera.Follow = center.transform;
        framingTransposer.m_TrackedObjectOffset.y = zoomInDistance;

        currentLevel += 1;
        UpdateLevelText();
        DisplayNextQuestion();

        //加载蘑菇资源(使用addressable)
        //LoadMushroomsModel(currentLevel);

    }
    private void UpdateLevelText()
    {
        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        levelText.text = currentLevel.ToString();
    }

    public void DisplayNextQuestion()
    {
        if (currentQuestionIndex < totalLevel)
        {
            //infoText = GameObject.Find("InfoText").GetComponent<TextMeshProUGUI>();
            //infoText.text = "请采摘唯一的好蘑菇";
            Debug.Log("显示图片");
        }
        else
        {
            // 已回答所有问题，结算问题
            if (correctAnswersCount == totalLevel)
            {
                //通关成功
                Debug.Log( "太棒了，通关成功");
                //显示成就卡
                endingUIPanel.SetActive(true);


            }
        }
    }
    private void ClearLevel()
    {
        // 清理蘑菇
        foreach (var mushroom in GameObject.FindGameObjectsWithTag("mushroom"))
        {
            Destroy(mushroom);
        }
        //清理cube
        foreach (var cube in GameObject.FindGameObjectsWithTag("Cube"))
        {
            Destroy(cube);
        }
    }

    private void CreateBoxes(int count)
    {
        //在输入的位置坐标不重复地生成设置数目的boxprefab
        List<Vector3> availablePosition = new List<Vector3>(mushroomPlaceholder);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availablePosition.Count);
            Vector3 spawnPosition = availablePosition[randomIndex];
            Instantiate(boxPrefab, spawnPosition, Quaternion.identity);
            availablePosition.RemoveAt(randomIndex);
        }
        
    }
    void LoadMushroomsModel(int levelIndex)
    {
        //根据第几关异步加载素材（未写）
        //第一关
        /*if (levelIndex == 0)
        {*/
        mushroomHandle = Addressables.LoadAssetsAsync<GameObject>(mushroomLabel[levelIndex-1], null);
        mushroomHandle.Completed += OnMushroomsLoaded;
        //mushroomNextLevelHandle = Addressables.LoadAssetsAsync<GameObject>(mushroomLabel[levelIndex + 1], null);
        Debug.Log("加载下一关资源");
        //}
        //中间关
        /*else if (levelIndex + 1 <= totalLevel)
        {
            
        }*/
        //最后一关
    }

    //apk测试版
    private void MushroomLoad()
    {
        List<GameObject> goodMushrooms = new List<GameObject>();
        List<GameObject> badMushrooms = new List<GameObject>();
        cubes = GameObject.FindGameObjectsWithTag("Cube");

        foreach (GameObject mushroom in mushroomPrefab)
        {
            MushroomInformation difficultyComponent = mushroom.GetComponent<MushroomInformation>();
            if (difficultyComponent != null)
            {
                if (difficultyComponent.isPoisonous == false)
                {
                    goodMushrooms.Add(mushroom);
                }
                else
                {
                    badMushrooms.Add(mushroom);
                }
            }
        }
        if (goodMushrooms.Count > 0)
        {
            GameObject selectedGoodMushroom = goodMushrooms[Random.Range(0, goodMushrooms.Count)];
            currenLevelMushrooms.Add(selectedGoodMushroom);
            //goodMushrooms.Remove(selectedGoodMushroom);  // 移除已选的好蘑菇(可以在不同关卡出现不重复蘑菇时使用)

            if (badMushrooms.Count > 0)
            {
                while (currenLevelMushrooms.Count < cubes.Length && badMushrooms.Count > 0)
                {
                    GameObject selectedBadMushroom = badMushrooms[Random.Range(0, badMushrooms.Count)];
                    currenLevelMushrooms.Add(selectedBadMushroom);
                    badMushrooms.Remove(selectedBadMushroom);
                }
            }
            else
            {
                Debug.Log("没有坏蘑菇可用");
            }
        }
        else
        {
            Debug.LogError("没有好蘑菇可用");
        }
        //替换空盒子的位置,实例化每一个蘑菇模型
        ReplaceMushroom();

    }
    private void OnMushroomsLoaded(AsyncOperationHandle<IList<GameObject>> handle)
    {
        List<GameObject> goodMushrooms = new List<GameObject>();
        List<GameObject> badMushrooms = new List<GameObject>();
        cubes = GameObject.FindGameObjectsWithTag("Cube");

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("找到资源包");

            //选出蘑菇，条件：其中一个是好蘑菇，随机
            foreach (GameObject mushroom in handle.Result)
            {
                MushroomInformation difficultyComponent = mushroom.GetComponent<MushroomInformation>();
                if (difficultyComponent != null)
                {
                    if (difficultyComponent.isPoisonous == false)
                    {
                        goodMushrooms.Add(mushroom);
                    }
                    else
                    {
                        badMushrooms.Add(mushroom);
                    }
                }
            }
            if (goodMushrooms.Count > 0)
            {
                GameObject selectedGoodMushroom = goodMushrooms[Random.Range(0, goodMushrooms.Count)];
                currenLevelMushrooms.Add(selectedGoodMushroom);
                //goodMushrooms.Remove(selectedGoodMushroom);  // 移除已选的好蘑菇(可以在不同关卡出现不重复蘑菇时使用)

                if (badMushrooms.Count > 0)
                {
                    while (currenLevelMushrooms.Count < cubes.Length)
                    {
                        GameObject selectedBadMushroom = badMushrooms[Random.Range(0, badMushrooms.Count)];
                        currenLevelMushrooms.Add(selectedBadMushroom);
                        badMushrooms.Remove(selectedBadMushroom);
                    }
                }
                else
                {
                    Debug.Log("没有坏蘑菇可用");
                }
            }
            else
            {
                Debug.LogError("没有好蘑菇可用");
            }
        }
        else
        {
            Debug.LogError("蘑菇模型加载失败: " + handle.OperationException.ToString());
        }
        

        //替换空盒子的位置,实例化每一个蘑菇模型
        ReplaceMushroom();

        //完成后释放资源
        ReleaseMushroomResources();

    }

    private void ReplaceMushroom()
    {

        if (cubes != null)
        {
            foreach (GameObject cube in cubes)
            {
                if (cube != null && currenLevelMushrooms.Count > 0)
                {
                    int randomIndex = Random.Range(0, currenLevelMushrooms.Count);
                    GameObject mushroomToInstantiate = currenLevelMushrooms[randomIndex];
                    Instantiate(mushroomToInstantiate, cube.transform.position, cube.transform.rotation); // 实例化Prefab
                    Debug.Log("开始实例化蘑菇资源");
                    currenLevelMushrooms.RemoveAt(randomIndex); // 移除已经使用的蘑菇Prefab，避免重复
                    Destroy(cube); 
                }
            }

        }
        else
        {
            Debug.Log("cubes数组是空的");
        }
        Debug.Log("加载完成");
        currenLevelMushrooms.Clear();//清空列表
    }

    public void ReleaseMushroomResources()
    {
        if (mushroomHandle.IsValid())
        {
            Addressables.Release(mushroomHandle); // 释放当前加载的蘑菇资源
            Debug.Log("蘑菇资源已释放！");
        }
    }




    //蘑菇信息卡
    public void DisplayNextLevel(string name, string description, Sprite spriteToLoad)
    {
        
        nameText.text = name;
        mushroomInfoText.text = description;
        nextLevelImage.sprite = spriteToLoad;
        nextLevelUIPrefab.SetActive(true);
    }
    public void Gameover(string name, string description, Sprite spriteToLoad)
    {
        gameOverNameText.text = name;
        gameOverInfoText.text = description;
        gameOverImage.sprite = spriteToLoad;
        gameOverUIPrefab.SetActive(true);
        if (!hasRevived)
        {
            reviveButton.gameObject.SetActive(true);
        }
        else
        {
            reviveButton.gameObject.SetActive(false);
        }
    }
    public void FlashEffect()
    {
        //flashCoroutine = StartCoroutine(FlashCoroutine());
        flashEffect.SetActive(true);
        // 检查动画是否已经完成循环
        while (currentLoopCount < 5)
        {
            // 播放动画
            flashAnimator.Play("FlashEffect");
            currentLoopCount++;
        }
            flashEffect.SetActive(false);
    }
   /* IEnumerator FlashCoroutine()
    {
        float flashDuration = 3f;
        float currentTime = 0f;
        float intensityChange = 0.5f;//速度*倍数

        while (currentTime < flashDuration)
        {
            // 增加或减少强度，取决于当前阶段
            if (isIncreasing)
            {
                intensity += Time.deltaTime * intensityChange;
            }
            else
            {
                intensity -= Time.deltaTime * intensityChange;
            }

            // 设置Shader中的_FullScreenIntensity值
            flashMaterial.SetFloat("_FullScreenIntensity", Mathf.Clamp(intensity, 0f, 0.25f));

            // 切换状态
            if (intensity >= 0.25f || intensity <= 0f)
            {
                isIncreasing = !isIncreasing;
                intensity = Mathf.Clamp(intensity, 0f, 0.25f);
            }

            currentTime += Time.deltaTime;
            yield return null; // 每帧都执行
        }

        intensity = 0f; // 结束时恢复到初始值
        flashMaterial.SetFloat("_FullScreenIntensity", intensity);
    }*/
    private void BackToHome()
        {
            SceneManager.LoadScene("Start");
        }
        private void Replay()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Revive()
        {
            if (hasRevived == false)
            {
                revieveUIPrefab.SetActive(true);
                hasRevived = true;
            }
        }

        public void FocusToCenter()
        {
            mainCamera.Follow = center.transform;
        }
    }


