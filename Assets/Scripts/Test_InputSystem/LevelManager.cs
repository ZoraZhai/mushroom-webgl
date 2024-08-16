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
    [Header("�ؿ���Ϣ")]
    public List<Vector3> mushroomPlaceholder;
    public int currentLevel = 0; // ��ǰ�ؿ�����
    public int totalLevel = 3;//�ܹ��ؿ���Ŀ
    public int numberOfBoxes;//�ؿ���Ӧ��������(�����Ƿ���������ݽṹ����?)
    //public float radius = 0.1f;//������ɺ���λ�õİ뾶

    public GameObject center;
    public TextMeshProUGUI levelText; // UI TextԪ�ص�����
    //public TextMeshProUGUI infoText;

    [Header("���ش����Դ")]
    private AssetBundle currentLevelBundle;
    private AssetBundle nextLevelBundle;
    //private string assetBundleBaseURL = "E:/Mushroom/Assets/StreamingAssets/"; // AssetBundlePC�˻���URL
    //private string assetBundleBaseURL = Application.streamingAssetsPath + "/";//AssetBundle ��׿��URL
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

    [Header("�������")]
    public float zoomOutDistance = 1.6f;
    public float zoomInDistance = 0.41f;

    public float switchDuration = 0.5f;//�����ͬĢ��֮����л�Э���ٶ�
    public float dampingSpeed = 0.5f;
    public float targetY = 0.14f;//���Y��ƫ��
    public float mushroomY = 0.11f;

    [Header("�ؿ�״̬")]
    public bool hasRevived = false;
    public bool hasConfirmationPanel = false;

    [Header("�ε���Ч")]
    public VolumeComponent chromaticAberration;
    public float duration = 1.0f; // ��������ʱ��
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
        //ClearLevel();//��չؿ�
        //CreateBoxes(numberOfBoxes);//�½��պ���

        nextLevelUIPrefab.SetActive(false);//�ر�Ģ����Ϣ��
        gameOverUIPrefab.SetActive(false);
        revieveUIPrefab.SetActive(false);
        endingUIPanel.SetActive(false);

        //��ʼ���ӽ�
        framingTransposer.m_CameraDistance = zoomOutDistance;
        mainCamera.Follow = center.transform;
        framingTransposer.m_TrackedObjectOffset.y = zoomInDistance;

        currentLevel += 1;
        UpdateLevelText();
        DisplayNextQuestion();

        //����asset bundle
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
            //infoText.text = "���ժΨһ�ĺ�Ģ��";
            infoImage.gameObject.SetActive(true);
            Debug.Log("��ʾͼƬ");
        }
        else
        {
            // �ѻش��������⣬��������
            if (correctAnswersCount == totalLevel)
            {
                //ͨ�سɹ�
                Debug.Log( "̫���ˣ�ͨ�سɹ�");
                //��ʾ�ɾͿ�
                endingUIPanel.SetActive(true);


            }
        }
    }
    private void ClearLevel()
    {
        

        // ����Ģ��
        foreach (var mushroom in GameObject.FindGameObjectsWithTag("mushroom"))
        {
            Destroy(mushroom);
        }
        //����cube
        foreach (var cube in GameObject.FindGameObjectsWithTag("Cube"))
        {
            Destroy(cube);
        }
    }

    private void CreateBoxes(int count)
    {
        //�������λ�����겻�ظ�������������Ŀ��boxprefab
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
            /*//ʹ��streamingassets����
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                currentLevelBundle = DownloadHandlerAssetBundle.GetContent(www);*/

            
            Debug.Log("�ɹ����ر��ؿ�AssetBundle");

            if (currentLevelBundle != null)
            {
                GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");

                //�����µ�Ģ��ģ��
                var mushroomPrefabs = currentLevelBundle.LoadAllAssets<GameObject>();
                int goodmushroomCount = 0;
                int totalmushroomCount = 0;
                // ɸѡ��ͬ�Ѷȵ�Ģ��Prefabs
                foreach (var prefab in mushroomPrefabs)
                {
                    MushroomInformation difficultyComponent = prefab.GetComponent<MushroomInformation>();
                    if (difficultyComponent != null)
                    {
                        //�����Ѷ�(�������Ƶ�������֮���)
                        //if (difficultyComponent != null && difficultyComponent.difficultyLevel == 2)

                        //ֻ����һ����Ģ��
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
                        GameObject newMushroom = Instantiate(mushroomToInstantiate, cube.transform.position, cube.transform.rotation); // ʵ����Prefab
                        currenLevelMushrooms.RemoveAt(randomIndex); // �Ƴ��Ѿ�ʹ�õ�Ģ��Prefab�������ظ�
                        Destroy(cube);

                        currenLevelMushrooms.Clear();

                    }
                }
                //�ͷ���Դ
                currentLevelBundle.Unload(false);
            }
            else
            {
                Debug.LogError("���ص�AssetBundle�ǿյ�");
            }
            
            //������һ�ؿ���AssetBundle
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
    //Ģ����Ϣ��
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

    //ɫ���
    public void ChromaticAberrationController()
    {
        
    }
}


