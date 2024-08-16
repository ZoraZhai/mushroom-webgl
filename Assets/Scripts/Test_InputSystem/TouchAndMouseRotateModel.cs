using UnityEngine;
using UnityEngine.InputSystem;

public class TouchAndMouseRotateModel : MonoBehaviour
{
    public float rotateSpeed = 100f;
    private Quaternion lastRotation;

    public PlayerController inputActions;


    private void OnEnable()
    {

        // ����Input Actions
        inputActions.RotationControls.TouchRotate.performed += OnTouchRotate;
        inputActions.RotationControls.MouseRotate.performed += OnMouseRotate;
    }

    private void OnDisable()
    {
        // ȡ������Input Actions
        inputActions.RotationControls.TouchRotate.performed -= OnTouchRotate;
        inputActions.RotationControls.MouseRotate.performed -= OnMouseRotate;
    }

    private void OnTouchRotate(InputAction.CallbackContext context)
    {
        // ����������
        if (context.phase == InputActionPhase.Performed)
        {
            var delta = context.ReadValue<Vector2>();
            // ���ݴ���λ�ü�����ת�Ƕ�
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            transform.Rotate(Vector3.up, angle * rotateSpeed * Time.deltaTime);
        }
    }

    private void OnMouseRotate(InputAction.CallbackContext context)
    {
        // �����������
        if (context.performed)
        {
            Debug.Log("�����ת");
            // ����������ʱ��תģ��
            var mouseDelta = context.ReadValue<Vector2>(); // ��ȡ����λ��
            float angle = Mathf.Atan2(mouseDelta.y, mouseDelta.x) * Mathf.Rad2Deg;
            transform.Rotate(Vector3.up, angle * rotateSpeed * Time.deltaTime);
        }
    }
}