using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputPinch : MonoBehaviour
{
    //PlayerAction inputActions; // �������붯���ʲ�
    InputAction touchAction;
   
    private Vector2 touch0StartPos, touch1StartPos;
    private float initialDistance;
    public static bool isZooming;
    [SerializeField] float zoomSpeed =30f;
    float velocity=0f;
    [SerializeField] float minZ = -500f;
    [SerializeField] float maxZ = -300f;

    void Awake()
    {
        //inputActions = new PlayerAction();

    }

    void OnEnable()
    {

        //inputActions.UI.Enable();
       
    }

    void OnDisable()
    {
       //inputActions.UI.Disable();
    }

    void Update()
    {
        // �������������
        // ʹ��Touchscreen.current.touches����ȡ��ǰ�����д�����
        var touches = Touchscreen.current.touches;

        // ��ʼ��������
        int activeTouches = 0;

        // ����touches���飬���㴦�ڻ״̬�Ĵ���������
        foreach (var touch in touches)
        {
            // Check if the touch is in progress
            if (touch.isInProgress)
            {
                activeTouches++;
            }
        }

        // ����Ƿ�ͬʱ����������
        if (activeTouches == 2)
        {
            Debug.Log("��ǰ��Ļ�����������㡣");
            isZooming = true;
            // ������ִ������Ҫ���߼�
        } else {
            isZooming = false;
        }

        print($"touchPoints:  {activeTouches}");
        
    
 

      
        if (isZooming)
        {
            var touch0Pos = touches[0].position.ReadValue();
            var touch1Pos = touches[1].position.ReadValue(); ;


                // ��ʼ����
                touch0StartPos = touches[0].startPosition.ReadValue();
                touch1StartPos = touches[1].startPosition.ReadValue();
                initialDistance = Vector2.Distance(touch0StartPos, touch1StartPos);


                // ��������
                var currentDistance = Vector2.Distance(touch0Pos, touch1Pos);
                var scaleFactor = currentDistance / initialDistance;
                if(scaleFactor<1) scaleFactor = -initialDistance/currentDistance;
            // �����������ӵ���UI��С�������Ұ��
            // ���磺transform.localScale = originalScale * scaleFactor;
            velocity = Camera.main.transform.position.z + scaleFactor * Time.deltaTime* zoomSpeed;
                velocity = Mathf.Clamp(velocity, minZ, maxZ);
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, velocity);
             //   Debug.Log($"Scale Factor: {scaleFactor}");
          
        }

    }
}
