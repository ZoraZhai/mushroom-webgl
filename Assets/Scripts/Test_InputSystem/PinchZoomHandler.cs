using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PinchZoomHandler : MonoBehaviour
{
    private PlayerController inputActions;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 4f;

    private Vector2? firstFingerPosition;
    private Vector2? secondFingerPosition;

    private void Awake()
    {
        inputActions = new PlayerController();
        //inputActions.PinchZoom.performed += OnPinchZoomPerformed;
        inputActions.Enable();
    }

    private void OnPinchZoomPerformed(InputAction.CallbackContext context)
    {
        Debug.Log(firstFingerPosition + secondFingerPosition);
        if (!firstFingerPosition.HasValue || !secondFingerPosition.HasValue)
            return;

        Vector2 delta = secondFingerPosition.Value - firstFingerPosition.Value;
        float distanceDelta = delta.magnitude;

        mainCamera.fieldOfView = Mathf.Lerp(minZoom, maxZoom, Mathf.Clamp01(distanceDelta / zoomSpeed));

        firstFingerPosition = secondFingerPosition.Value;
    }

    private void OnDestroy()
    {
        var touchControls = new PlayerController();
        touchControls.Disable();
    }

}
