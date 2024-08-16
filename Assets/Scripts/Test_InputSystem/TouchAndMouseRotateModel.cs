using UnityEngine;
using UnityEngine.InputSystem;

public class TouchAndMouseRotateModel : MonoBehaviour
{
    public float rotateSpeed = 100f;
    private Quaternion lastRotation;

    public PlayerController inputActions;


    private void OnEnable()
    {

        // 订阅Input Actions
        inputActions.RotationControls.TouchRotate.performed += OnTouchRotate;
        inputActions.RotationControls.MouseRotate.performed += OnMouseRotate;
    }

    private void OnDisable()
    {
        // 取消订阅Input Actions
        inputActions.RotationControls.TouchRotate.performed -= OnTouchRotate;
        inputActions.RotationControls.MouseRotate.performed -= OnMouseRotate;
    }

    private void OnTouchRotate(InputAction.CallbackContext context)
    {
        // 处理触摸输入
        if (context.phase == InputActionPhase.Performed)
        {
            var delta = context.ReadValue<Vector2>();
            // 根据触摸位置计算旋转角度
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            transform.Rotate(Vector3.up, angle * rotateSpeed * Time.deltaTime);
        }
    }

    private void OnMouseRotate(InputAction.CallbackContext context)
    {
        // 处理鼠标输入
        if (context.performed)
        {
            Debug.Log("鼠标旋转");
            // 鼠标左键按下时旋转模型
            var mouseDelta = context.ReadValue<Vector2>(); // 读取鼠标的位移
            float angle = Mathf.Atan2(mouseDelta.y, mouseDelta.x) * Mathf.Rad2Deg;
            transform.Rotate(Vector3.up, angle * rotateSpeed * Time.deltaTime);
        }
    }
}