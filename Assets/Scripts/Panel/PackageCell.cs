using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PackageCell : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image itemImage;
    public PackagePanel uiParent;

    private PackageTableItem packageTableItem;
    private PackageLocalItem packageLocalItem;



    public void Init(PackageLocalItem item, PackagePanel parent)
    {
        packageLocalItem = item;
        packageTableItem = GameManager.Instance.GetPackageItemById(packageLocalItem.id); 
        uiParent = parent;

        if (packageLocalItem != null)
        {
            nameText.text = packageTableItem.mushroomName;
            descriptionText.text = packageTableItem.description;
            itemImage.sprite = Resources.Load<Sprite>(packageTableItem.imagePath);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 将当前 PackageCell 的 packageTableItem 的 id 传递给 PackagePanel
        //uiParent.chooseUID = GetUidByPackageTableItem(packageTableItem);
        this.uiParent.chooseUID = this.packageLocalItem.uid; 
    }


    //根据 packageTableItem 获取对应的 PackageLocalItem 的 uid
    private string GetUidByPackageTableItem(PackageTableItem item)
    {
        PackageLocalItem localItem = GameManager.Instance.GetPackageLocalItemByUId(item.id.ToString());
        return localItem != null ? localItem.uid : string.Empty;
    }

}