using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PackagePanel : BasePanel
{
    public Transform UIScrollView; // ������ͼ��Transform
    public Transform UITabName; // ��ǩ����Transform
    public Transform UIMenuForest; // �˵���Transform
    public Transform UILeftBtn; // ��ť��Transform
    public Transform UIRightBtn; // �Ұ�ť��Transform
    public Transform UIDetailPanel; // ��������Transform

    private PackageTable packageTable; // �����ݱ�
    public List<PackageCell> packageCells; // ����Ԫ�б�
    private PackageCell PackageCellPrefab;
    private List<PackageLocalItem> pageItems;

    public string terraintype = "forest";//����ɸѡ
    private string _chooseUid;
    public string chooseUID
    {
        get { return _chooseUid; }
        set
        {
            _chooseUid = value;
            RefreshDetail();
            //Debug.Log("New chooseUID is set to: " + _chooseUid);
        }
    }

    private void Awake()
    {
        // ��ʼ��UI�������
        UIDetailPanel = transform.Find("DetailPanel");
        UIScrollView = transform.Find("ListPanel/Scroll View");
    }

    private void Start()
    {
        LoadPackageItems();
        RefreshUI();
    }

    private void LoadPackageItems()
    {
        //pageItems = GameManager.Instance.GetPackageLocalItemsByTerrainType(terrainType);
        pageItems = GameManager.Instance.GetPackageLocalItems();
        PackageCellPrefab = Resources.Load<PackageCell>("Package/PackageUIItem");

    }

    private void RefreshUI()
    {
        // �������������ԭ������Ʒ
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        foreach (Transform child in scrollContent)
        {
            Destroy(child.gameObject);
        }

        // ���´���UI��
        foreach (var item in pageItems)
        {
            var packageCell = Instantiate(PackageCellPrefab, scrollContent);
            packageCell.Init(item, this);

        }
    }

    public void RefreshDetail()
    {
        if (string.IsNullOrEmpty(_chooseUid)) return;

        // ���ݵ�ǰѡ�е� UID ��ȡ��Ӧ�� PackageLocalItem
        PackageLocalItem localItem = GameManager.Instance.GetPackageLocalItemByUId(_chooseUid);
        if (localItem != null)
        {
            // ȷ�� Detail Panel ������
            GameObject detailPanelGameObject = UIDetailPanel.gameObject;
            detailPanelGameObject.SetActive(true);
            // ʹ�� localItem �е�����ˢ���������
            UIDetailPanel.GetComponent<PackageDetail>().Refresh(localItem, this);
        }
    }
}
