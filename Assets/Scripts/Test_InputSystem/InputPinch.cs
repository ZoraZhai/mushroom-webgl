using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputPinch : MonoBehaviour
{
    //PlayerAction inputActions; // 引用输入动作资产
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
        // 检测两个触摸点
        // 使用Touchscreen.current.touches来获取当前的所有触摸点
        var touches = Touchscreen.current.touches;

        // 初始化计数器
        int activeTouches = 0;

        // 遍历touches数组，计算处于活动状态的触摸点数量
        foreach (var touch in touches)
        {
            // Check if the touch is in progress
            if (touch.isInProgress)
            {
                activeTouches++;
            }
        }

        // 检查是否同时有两个触点
        if (activeTouches == 2)
        {
            Debug.Log("当前屏幕上有两个触点。");
            isZooming = true;
            // 在这里执行你需要的逻辑
        } else {
            isZooming = false;
        }

        print($"touchPoints:  {activeTouches}");
        
    
 

      
        if (isZooming)
        {
            var touch0Pos = touches[0].position.ReadValue();
            var touch1Pos = touches[1].position.ReadValue(); ;


                // 开始缩放
                touch0StartPos = touches[0].startPosition.ReadValue();
                touch1StartPos = touches[1].startPosition.ReadValue();
                initialDistance = Vector2.Distance(touch0StartPos, touch1StartPos);


                // 正在缩放
                var currentDistance = Vector2.Distance(touch0Pos, touch1Pos);
                var scaleFactor = currentDistance / initialDistance;
                if(scaleFactor<1) scaleFactor = -initialDistance/currentDistance;
            // 根据缩放因子调整UI大小或相机视野等
            // 例如：transform.localScale = originalScale * scaleFactor;
            velocity = Camera.main.transform.position.z + scaleFactor * Time.deltaTime* zoomSpeed;
                velocity = Mathf.Clamp(velocity, minZ, maxZ);
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, velocity);
             //   Debug.Log($"Scale Factor: {scaleFactor}");
          
        }

    }
}
