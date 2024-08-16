using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PackagePanel : BasePanel
{
    public Transform UIScrollView; // 滚动视图的Transform
    public Transform UITabName; // 标签名的Transform
    public Transform UIMenuForest; // 菜单的Transform
    public Transform UILeftBtn; // 左按钮的Transform
    public Transform UIRightBtn; // 右按钮的Transform
    public Transform UIDetailPanel; // 详情面板的Transform

    private PackageTable packageTable; // 包数据表
    public List<PackageCell> packageCells; // 包单元列表
    private PackageCell PackageCellPrefab;
    private List<PackageLocalItem> pageItems;

    public string terraintype = "forest";//地形筛选
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
        // 初始化UI组件引用
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
        // 清理滚动容器中原本的物品
        RectTransform scrollContent = UIScrollView.GetComponent<ScrollRect>().content;
        foreach (Transform child in scrollContent)
        {
            Destroy(child.gameObject);
        }

        // 重新创建UI项
        foreach (var item in pageItems)
        {
            var packageCell = Instantiate(PackageCellPrefab, scrollContent);
            packageCell.Init(item, this);

        }
    }

    public void RefreshDetail()
    {
        if (string.IsNullOrEmpty(_chooseUid)) return;

        // 根据当前选中的 UID 获取对应的 PackageLocalItem
        PackageLocalItem localItem = GameManager.Instance.GetPackageLocalItemByUId(_chooseUid);
        if (localItem != null)
        {
            // 确保 Detail Panel 被激活
            GameObject detailPanelGameObject = UIDetailPanel.gameObject;
            detailPanelGameObject.SetActive(true);
            // 使用 localItem 中的数据刷新详情面板
            UIDetailPanel.GetComponent<PackageDetail>().Refresh(localItem, this);
        }
    }
}
