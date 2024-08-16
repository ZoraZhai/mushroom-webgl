using UnityEngine;
using System.Collections.Generic;

public class PackageLocalData
{
    private static PackageLocalData _instance;
    //µÿ–Œ∑÷“≥
    public Dictionary<string, List<PackageLocalItem>> terrainTypePages;

    public static PackageLocalData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PackageLocalData();
            }
            return _instance;
        }
    }


    public List<PackageLocalItem> items;

    public void SavePackage()
    {
        Debug.Log("Saving package data...");
        string inventoryJson = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PackageLocalData", inventoryJson);
        PlayerPrefs.Save();
        Debug.Log("Package data saved.");
    }

    public List<PackageLocalItem> LoadPackage()
    {
        Debug.Log("Loading package data...");
        if (items != null)
        {
            return items;
        }

        string inventoryJson = PlayerPrefs.GetString("PackageLocalData");
        if (string.IsNullOrEmpty(inventoryJson))
        {
            Debug.LogError("No saved data found.");
            items = new List<PackageLocalItem>();
            return items;
        }

        Debug.Log("Attempting to load JSON data: " + inventoryJson);
        PackageLocalData packageLocalData = JsonUtility.FromJson<PackageLocalData>(inventoryJson);
        if (packageLocalData == null || packageLocalData.items == null)
        {
            Debug.LogError("Failed to parse JSON data.");
            items = new List<PackageLocalItem>();
            return items;
        }

        items = packageLocalData.items;
        Debug.Log("Package data loaded successfully.");
        return items;
    }
}



[System.Serializable]
public class PackageLocalItem
{
    public string uid;
    public int id;
    public int num;
    public bool isNew;

    public override string ToString()
    {
        return string.Format("[id]:{0}", id);
    }
}
