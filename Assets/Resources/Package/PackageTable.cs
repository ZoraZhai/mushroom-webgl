using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="PackageTable",menuName ="Mushroom/PackageTable")]
public class PackageTable: ScriptableObject
{
    public List<PackageTableItem> DataList = new List<PackageTableItem>();
}
[System.Serializable]
public class PackageTableItem
{
    public int id;
    public int difficultyLevel;
    public bool isPoisonous;
    public string mushroomName;
    public string description;

    public string poisonDescription;
    public string typeDescription;
    [TextArea]
    public string groundDescription;
    [TextArea]
    public string CaseDescription;
    //public Sprite mushroomImageSprite;
    public string imagePath;
    //用于三维对焦
    public string modelName;
    //地形分类
    public string terrainType;

}
