using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlobalPointerClickListener : MonoBehaviour
{
    void Start()
    {
        // ��ȡ�򴴽�EventTrigger���
        EventTrigger trigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

        // ����һ���µ���Ŀ������PointerClick�¼�
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback = new EventTrigger.TriggerEvent();

        // �󶨻ص���OnPointerClickOutside����
        entry.callback.AddListener((data) => OnPointerClickOutside((PointerEventData)data));

        // ����Ŀ��ӵ�EventTrigger���
        trigger.triggers.Add(entry);
    }

    public void OnPointerClickOutside(PointerEventData eventData)
    {
        Debug.Log("����  OnPointerClickOutside ����");
        // ���PointerEventData�Ƿ�ָ��UIԪ�أ����ߵ�Ĳ���tagΪmushroom�����壩�����û�У���ִ��ȡ������
        if (!EventSystem.current.IsPointerOverGameObject(eventData.pointerId) && !eventData.pointerCurrentRaycast.gameObject.CompareTag("mushroom"))
        {
            MushroomClicker.LastClickedMushroom.Cancel(); // LastClickedMushroom��ȫ�ֿɷ��ʵ�
            Debug.Log("ȡ�����");
        }
    }
}
