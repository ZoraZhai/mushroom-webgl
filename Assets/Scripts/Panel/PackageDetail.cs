using Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PackageDetail : MonoBehaviour
{
    private Transform UIName;
    private Transform UIPoison;
    private Transform UIType;
    private Transform UIGround;
    private Transform UICase;
    private Transform UICamera;

    private PackageLocalItem packageLocalData;
    private PackageTableItem packageTableItem;
    // private PackagePanel uiParent; // ��� uiParent ����δʹ�ã������Ƴ�

    private GameObject modelInstance;
    private GameObject instantiatedModel;
    private void Awake()
    {
        InitUIName();
    }

    private void Start()
    {
        // ȷ���� Awake ֮�������ж��󶼳�ʼ������� Test
        //Test();
    }

    private void Test()
    {
        // ȷ�� GameManager ʵ�������� GetPackageLocalData ����������Ч����
        if (GameManager.Instance != null)
        {
            Refresh(GameManager.Instance.GetPackageLocalData()[1], null);
        }
        else
        {
            Debug.LogError("GameManager.Instance is not initialized.");
        }
    }

    private void InitUIName()
    {
        UIName = transform.Find("Name");
        UIPoison = transform.Find("Center/PoisonText");
        UIType = transform.Find("Center/TypeText");
        UIGround = transform.Find("Center/GroundText");
        UICase = transform.Find("Center/CaseText");
        UICamera = transform.Find("Render Camera");

        if (UICamera != null)
        {
            var cinemachineComponent = UICamera.GetComponent<CinemachineVirtualCamera>();
            if (cinemachineComponent == null)
            {
                Debug.LogError("UICamera does not have Cinemachine Virtual Camera component attached.");
            }
        }
        else
        {
            Debug.LogError("Render Camera not found.");
        }
    }

    public void Refresh(PackageLocalItem packageLocalData, PackagePanel uiParent)
    {
        Debug.Log("Refreshing detail for item with id: " + (packageLocalData != null ? packageLocalData.id.ToString() : "null"));
        if (packageLocalData == null)
        {
            Debug.LogError("PackageLocalItem is null.");
            return;
        }

        this.packageLocalData = packageLocalData;
        this.packageTableItem = GameManager.Instance.GetPackageItemById(packageLocalData.id);

        if (packageTableItem == null)
        {
            Debug.LogError("PackageTableItem is null when trying to refresh detail.");
            return;
        }

        // �� UI ������ı�
        UIName.GetComponent<TextMeshProUGUI>().text = packageTableItem.mushroomName;
        UIPoison.GetComponent<TextMeshProUGUI>().text = packageTableItem.poisonDescription;
        UIType.GetComponent<TextMeshProUGUI>().text = packageTableItem.typeDescription;
        UIGround.GetComponent<TextMeshProUGUI>().text = packageTableItem.groundDescription;
        UICase.GetComponent<TextMeshProUGUI>().text = packageTableItem.CaseDescription;

        // ģ�ͺ����������
        string prefabPath = "Prefab/" + packageTableItem.modelName;
        modelInstance = Resources.Load<GameObject>(prefabPath);
        //GameObject modelObject = GameObject.Find(packageTableItem.modelName);
        if (modelInstance != null)
        {
            instantiatedModel = Instantiate(modelInstance, transform.position, Quaternion.identity);
            UICamera.GetComponent<CinemachineVirtualCamera>().Follow = instantiatedModel.transform;
        }
        else
        {
            Debug.LogError($"Model with modelName {packageTableItem.modelName} not found.");
        }
    }

    public void DestroyModel()
    {
        // ֻ�е�ģ�ʹ���ʱ������
        Destroy(instantiatedModel);

    }
}