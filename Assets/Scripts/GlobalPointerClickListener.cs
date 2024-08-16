using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlobalPointerClickListener : MonoBehaviour
{
    void Start()
    {
        // 获取或创建EventTrigger组件
        EventTrigger trigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

        // 创建一个新的条目来监听PointerClick事件
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback = new EventTrigger.TriggerEvent();

        // 绑定回调到OnPointerClickOutside方法
        entry.callback.AddListener((data) => OnPointerClickOutside((PointerEventData)data));

        // 将条目添加到EventTrigger组件
        trigger.triggers.Add(entry);
    }

    public void OnPointerClickOutside(PointerEventData eventData)
    {
        Debug.Log("进入  OnPointerClickOutside 函数");
        // 检查PointerEventData是否指向UI元素（或者点的不是tag为mushroom的物体），如果没有，则执行取消操作
        if (!EventSystem.current.IsPointerOverGameObject(eventData.pointerId) && !eventData.pointerCurrentRaycast.gameObject.CompareTag("mushroom"))
        {
            MushroomClicker.LastClickedMushroom.Cancel(); // LastClickedMushroom是全局可访问的
            Debug.Log("取消点击");
        }
    }
}
