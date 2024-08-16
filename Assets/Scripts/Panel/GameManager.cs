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
    //ͨ��id ��ȡPackageTable �����б��еĶ�Ӧ��Ŀ
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
    //ͨ������� uid ��ȡ PackageLocalData �����б��еĶ�Ӧ��Ŀ
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
        // ��ȡ��������
        List<PackageLocalItem> allItems = GetPackageLocalData();
        return allItems;
    }

    // ���ݵ������͹�������
    /*public List<PackageLocalItem> GetPackageLocalItemsByTerrainType(string terrainType)
    {
        // ��ȡ��������
        List<PackageLocalItem> allItems = GetPackageLocalData();

        // ɸѡ�ض��������͵���Ʒ
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
