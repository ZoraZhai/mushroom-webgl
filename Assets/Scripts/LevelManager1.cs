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

    //�������ʵ����Ģ��
    public List<GameObject> mushroomPrefab;

    [Header("�ؿ���Ϣ")]
    public List<Vector3> mushroomPlaceholder;
    public int currentLevel = 0; // ��ǰ�ؿ�����
    public int totalLevel = 3;//�ܹ��ؿ���Ŀ
    public int numberOfBoxes = 5;//�ؿ���Ӧ��������(�����Ƿ���������ݽṹ����?)
    //public float radius = 0.1f;//������ɺ���λ�õİ뾶

    public GameObject center;
    public TextMeshProUGUI levelText; // UI TextԪ�ص�����
    //public TextMeshProUGUI infoText;

    [Header("���ش����Դ")]
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


    [Header("�������")]
    public float zoomOutDistance = 1f;
    public float zoomInDistance = 0.41f;

    public float switchDuration = 0.7f;//�����ͬĢ��֮����л�Э���ٶ�
    public float dampingSpeed = 0.3f;
    public float targetY = 0.14f;//���Y��ƫ��
    public float mushroomY = 0.11f;

    [Header("�ؿ�״̬")]
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
        ClearLevel();//��չؿ�
        CreateBoxes(numberOfBoxes);//�½��պ��ӣ����б�ǩCube��
        MushroomLoad();

        nextLevelUIPrefab.SetActive(false);//�ر�Ģ����Ϣ��
        gameOverUIPrefab.SetActive(false);
        revieveUIPrefab.SetActive(false);
        endingUIPanel.SetActive(false);

        flashMaterial.SetFloat("_FullScreenIntensity", 0);

        if (flashCoroutine != null) // ���Э���������У�ֹͣЭ��
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        //��ʼ���ӽ�
        framingTransposer.m_CameraDistance = zoomOutDistance;
        mainCamera.Follow = center.transform;
        framingTransposer.m_TrackedObjectOffset.y = zoomInDistance;

        currentLevel += 1;
        UpdateLevelText();
        DisplayNextQuestion();

        //����Ģ����Դ(ʹ��addressable)
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
            //infoText.text = "���ժΨһ�ĺ�Ģ��";
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
    void LoadMushroomsModel(int levelIndex)
    {
        //���ݵڼ����첽�����زģ�δд��
        //��һ��
        /*if (levelIndex == 0)
        {*/
        mushroomHandle = Addressables.LoadAssetsAsync<GameObject>(mushroomLabel[levelIndex-1], null);
        mushroomHandle.Completed += OnMushroomsLoaded;
        //mushroomNextLevelHandle = Addressables.LoadAssetsAsync<GameObject>(mushroomLabel[levelIndex + 1], null);
        Debug.Log("������һ����Դ");
        //}
        //�м��
        /*else if (levelIndex + 1 <= totalLevel)
        {
            
        }*/
        //���һ��
    }

    //apk���԰�
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
            //goodMushrooms.Remove(selectedGoodMushroom);  // �Ƴ���ѡ�ĺ�Ģ��(�����ڲ�ͬ�ؿ����ֲ��ظ�Ģ��ʱʹ��)

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
                Debug.Log("û�л�Ģ������");
            }
        }
        else
        {
            Debug.LogError("û�к�Ģ������");
        }
        //�滻�պ��ӵ�λ��,ʵ����ÿһ��Ģ��ģ��
        ReplaceMushroom();

    }
    private void OnMushroomsLoaded(AsyncOperationHandle<IList<GameObject>> handle)
    {
        List<GameObject> goodMushrooms = new List<GameObject>();
        List<GameObject> badMushrooms = new List<GameObject>();
        cubes = GameObject.FindGameObjectsWithTag("Cube");

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("�ҵ���Դ��");

            //ѡ��Ģ��������������һ���Ǻ�Ģ�������
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
                //goodMushrooms.Remove(selectedGoodMushroom);  // �Ƴ���ѡ�ĺ�Ģ��(�����ڲ�ͬ�ؿ����ֲ��ظ�Ģ��ʱʹ��)

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
                    Debug.Log("û�л�Ģ������");
                }
            }
            else
            {
                Debug.LogError("û�к�Ģ������");
            }
        }
        else
        {
            Debug.LogError("Ģ��ģ�ͼ���ʧ��: " + handle.OperationException.ToString());
        }
        

        //�滻�պ��ӵ�λ��,ʵ����ÿһ��Ģ��ģ��
        ReplaceMushroom();

        //��ɺ��ͷ���Դ
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
                    Instantiate(mushroomToInstantiate, cube.transform.position, cube.transform.rotation); // ʵ����Prefab
                    Debug.Log("��ʼʵ����Ģ����Դ");
                    currenLevelMushrooms.RemoveAt(randomIndex); // �Ƴ��Ѿ�ʹ�õ�Ģ��Prefab�������ظ�
                    Destroy(cube); 
                }
            }

        }
        else
        {
            Debug.Log("cubes�����ǿյ�");
        }
        Debug.Log("�������");
        currenLevelMushrooms.Clear();//����б�
    }

    public void ReleaseMushroomResources()
    {
        if (mushroomHandle.IsValid())
        {
            Addressables.Release(mushroomHandle); // �ͷŵ�ǰ���ص�Ģ����Դ
            Debug.Log("Ģ����Դ���ͷţ�");
        }
    }




    //Ģ����Ϣ��
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
        // ��鶯���Ƿ��Ѿ����ѭ��
        while (currentLoopCount < 5)
        {
            // ���Ŷ���
            flashAnimator.Play("FlashEffect");
            currentLoopCount++;
        }
            flashEffect.SetActive(false);
    }
   /* IEnumerator FlashCoroutine()
    {
        float flashDuration = 3f;
        float currentTime = 0f;
        float intensityChange = 0.5f;//�ٶ�*����

        while (currentTime < flashDuration)
        {
            // ���ӻ����ǿ�ȣ�ȡ���ڵ�ǰ�׶�
            if (isIncreasing)
            {
                intensity += Time.deltaTime * intensityChange;
            }
            else
            {
                intensity -= Time.deltaTime * intensityChange;
            }

            // ����Shader�е�_FullScreenIntensityֵ
            flashMaterial.SetFloat("_FullScreenIntensity", Mathf.Clamp(intensity, 0f, 0.25f));

            // �л�״̬
            if (intensity >= 0.25f || intensity <= 0f)
            {
                isIncreasing = !isIncreasing;
                intensity = Mathf.Clamp(intensity, 0f, 0.25f);
            }

            currentTime += Time.deltaTime;
            yield return null; // ÿ֡��ִ��
        }

        intensity = 0f; // ����ʱ�ָ�����ʼֵ
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


