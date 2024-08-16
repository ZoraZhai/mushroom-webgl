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
    //������ά�Խ�
    public string modelName;
    //���η���
    public string terrainType;

}
