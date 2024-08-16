using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
//using UnityEngine.UIElements;

/*
[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public int numberOfBoxes;
}
*/
public class LevelManager : MonoBehaviour
{
    [Header("关卡信息")]
    public List<Vector3> mushroomPlaceholder;
    public int currentLevel = 0; // 当前关卡索引
    public int totalLevel = 3;//总共关卡数目
    public int numberOfBoxes;//关卡对应几个盒子(后期是否可以用数据结构管理?)
    //public float radius = 0.1f;//随机生成盒子位置的半径

    public GameObject center;
    public TextMeshProUGUI levelText; // UI Text元素的引用
    //public TextMeshProUGUI infoText;

    [Header("加载打包资源")]
    private AssetBundle currentLevelBundle;
    private AssetBundle nextLevelBundle;
    //private string assetBundleBaseURL = "E:/Mushroom/Assets/StreamingAssets/"; // AssetBundlePC端基本URL
    //private string assetBundleBaseURL = Application.streamingAssetsPath + "/";//AssetBundle 安卓端URL
    public string assetBundleBaseURL = "E:/AssetBundles/StandaloneWindows/";
    private List<GameObject> currenLevelMushrooms = new List<GameObject>();
    private List<GameObject> nextLevelMushrooms = new List<GameObject>();
    public GameObject boxPrefab;

    [Header("UI")]
    public Image infoImage;
    public int currentQuestionIndex = 0;
    public int correctAnswersCount = 0;
    public GameObject nextLevelUIPrefab;
    public GameObject gameOverUIPrefab;
    public GameObject revieveUIPrefab;
    public Button reviveButton;
    public GameObject endingUIPanel;
    public Image image;
    public Image gameOverImage;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI mushroomInfoText;
    public TextMeshProUGUI gameOverNameText;
    public TextMeshProUGUI gameOverInfoText;

    private CinemachineVirtualCamera mainCamera;
    private CinemachineFramingTransposer framingTransposer;

    [Header("相机焦距")]
    public float zoomOutDistance = 1.6f;
    public float zoomInDistance = 0.41f;

    public float switchDuration = 0.5f;//点击不同蘑菇之间的切换协程速度
    public float dampingSpeed = 0.5f;
    public float targetY = 0.14f;//相机Y轴偏移
    public float mushroomY = 0.11f;

    [Header("关卡状态")]
    public bool hasRevived = false;
    public bool hasConfirmationPanel = false;

    [Header("晕倒特效")]
    public VolumeComponent chromaticAberration;
    public float duration = 1.0f; // 动画持续时间
    void Start()
    {

        GameObject cameraObject = GameObject.FindWithTag("MainCamera");
        mainCamera = cameraObject.GetComponent<CinemachineVirtualCamera>();
        framingTransposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        InputPinchZoom pinchZoom = mainCamera.GetComponent<InputPinchZoom>();
        if (pinchZoom != null)
        {
            pinchZoom.enabled = true;
        }
        LoadLevel();
    }



    public void LoadLevel()
    {
        //LevelData level = levels[levelIndex];
        //ClearLevel();//清空关卡
        //CreateBoxes(numberOfBoxes);//新建空盒子

        nextLevelUIPrefab.SetActive(false);//关闭蘑菇信息卡
        gameOverUIPrefab.SetActive(false);
        revieveUIPrefab.SetActive(false);
        endingUIPanel.SetActive(false);

        //初始化视角
        framingTransposer.m_CameraDistance = zoomOutDistance;
        mainCamera.Follow = center.transform;
        framingTransposer.m_TrackedObjectOffset.y = zoomInDistance;

        currentLevel += 1;
        UpdateLevelText();
        DisplayNextQuestion();

        //加载asset bundle
        //StartCoroutine(LoadCurrentLevelAssetBundle("level1_bundle.unity3d"));
    }
    private void UpdateLevelText()
    {
        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        levelText.text = "Level: " + currentLevel;
    }

    public void DisplayNextQuestion()
    {
        if (currentQuestionIndex < totalLevel)
        {
            //infoText = GameObject.Find("InfoText").GetComponent<TextMeshProUGUI>();
            //infoText.text = "请采摘唯一的好蘑菇";
            infoImage.gameObject.SetActive(true);
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
    
    IEnumerator LoadCurrentLevelAssetBundle(string currentLevelAssetBundleName)
    {
        string url = assetBundleBaseURL + currentLevelAssetBundleName;

        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(url);
        yield return bundleRequest;
        currentLevelBundle = bundleRequest.assetBundle;
        if (currentLevelBundle != null)
        { 
            /*//使用streamingassets加载
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                currentLevelBundle = DownloadHandlerAssetBundle.GetContent(www);*/

            
            Debug.Log("成功加载本关卡AssetBundle");

            if (currentLevelBundle != null)
            {
                GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");

                //加载新的蘑菇模型
                var mushroomPrefabs = currentLevelBundle.LoadAllAssets<GameObject>();
                int goodmushroomCount = 0;
                int totalmushroomCount = 0;
                // 筛选不同难度的蘑菇Prefabs
                foreach (var prefab in mushroomPrefabs)
                {
                    MushroomInformation difficultyComponent = prefab.GetComponent<MushroomInformation>();
                    if (difficultyComponent != null)
                    {
                        //控制难度(后续控制地形属性之类的)
                        //if (difficultyComponent != null && difficultyComponent.difficultyLevel == 2)

                        //只加载一个好蘑菇
                        while (totalmushroomCount <= numberOfBoxes)
                        {
                            if (goodmushroomCount == 0 && difficultyComponent.isPoisonous == false)
                            {
                                currenLevelMushrooms.Add(prefab);
                                goodmushroomCount++;
                                totalmushroomCount++;
                            }
                            else if (goodmushroomCount == 1 && difficultyComponent.isPoisonous == true)
                            {
                                currenLevelMushrooms.Add(prefab);
                                totalmushroomCount++;
                            }
                        }
                    }
                }

                foreach (GameObject cube in cubes)
                {
                    if (currenLevelMushrooms.Count > 0)
                    {
                        int randomIndex = Random.Range(0, currenLevelMushrooms.Count);
                        GameObject mushroomToInstantiate = currenLevelMushrooms[randomIndex];
                        GameObject newMushroom = Instantiate(mushroomToInstantiate, cube.transform.position, cube.transform.rotation); // 实例化Prefab
                        currenLevelMushrooms.RemoveAt(randomIndex); // 移除已经使用的蘑菇Prefab，避免重复
                        Destroy(cube);

                        currenLevelMushrooms.Clear();

                    }
                }
                //释放资源
                currentLevelBundle.Unload(false);
            }
            else
            {
                Debug.LogError("本关的AssetBundle是空的");
            }
            
            //加载下一关卡的AssetBundle
            //StartCoroutine(LoadNextLevelAssetBundle("level1_bundle.unity3d"));
        }

        else
        {
            Debug.LogError("Failed to load next level AssetBundle!");
        }
    }

    
    IEnumerator LoadNextLevelAssetBundle(string nextLevelName)
    {
        nextLevelBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + nextLevelName);
        yield return null;
    }
    private void UnloadCurrentLevelAssets()
    {
        if (currentLevelBundle != null)
        {
            currentLevelBundle.Unload(true);
        }
        if (nextLevelBundle != null)
        {
            nextLevelBundle.Unload(true);
        }

    }
    //蘑菇信息卡
    public void DisplayNextLevel(string name, string description, Sprite spriteToLoad)
    {
        nextLevelUIPrefab.SetActive(true);
        nameText.text = name;
        mushroomInfoText.text = description;
        image.sprite = spriteToLoad;
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

    //色像差
    public void ChromaticAberrationController()
    {
        
    }
}


