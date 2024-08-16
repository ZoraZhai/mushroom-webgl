using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private PackageTable packageTable;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public PackageTable GetPackageTable()
    {
        if (packageTable == null)
        {
            packageTable = Resources.Load<PackageTable>("TableData/PackageTable");
        }
        return packageTable;
    }

    public List<PackageLocalItem> GetPackageLocalData()
    {
        return PackageLocalData.Instance.LoadPackage();
    }
    //通过id 获取PackageTable 数据列表中的对应条目
    public PackageTableItem GetPackageItemById(int id)
    {
        List<PackageTableItem> packageDataList = GetPackageTable().DataList;
        foreach (PackageTableItem item in packageDataList)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;
    }
    //通过传入的 uid 获取 PackageLocalData 数据列表中的对应条目
    public PackageLocalItem GetPackageLocalItemByUId(string uid)
    {
        List<PackageLocalItem> packageDataList = GetPackageLocalData();
        foreach (PackageLocalItem item in packageDataList)
        {
            if (item.uid == uid)
            {
                return item;
            }
        }
        return null;
    }


    public List<PackageLocalItem> GetPackageLocalItems()
    {
        // 获取所有数据
        List<PackageLocalItem> allItems = GetPackageLocalData();
        return allItems;
    }

    // 根据地形类型过滤数据
    /*public List<PackageLocalItem> GetPackageLocalItemsByTerrainType(string terrainType)
    {
        // 获取所有数据
        List<PackageLocalItem> allItems = GetPackageLocalData();

        // 筛选特定地形类型的物品
        var filteredItems = allItems.Where(item =>
        string.Equals(GetTerrainTypeByItemId(item.id), terrainType)
        ).ToList();
        return filteredItems;
    }
    public string GetTerrainTypeByItemId(int itemId)
    {
        PackageTableItem item = GetPackageItemById(itemId);
        return item != null ? item.terrainType : string.Empty;
    }
    */
}
