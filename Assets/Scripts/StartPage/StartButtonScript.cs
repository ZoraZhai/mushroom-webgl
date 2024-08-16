using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartButtonScript : MonoBehaviour, IPointerClickHandler
{
    public string PageName;
    //private PlayerController inputActions;
    /*public void Awake()
    {
        inputActions = new PlayerController();
        inputActions.UI.ClickPlay.performed += OnClickPlay;
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }*/

    //private void OnClickPlay(InputAction.CallbackContext context)
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(PageName);
        Debug.Log("点击成功，游戏开始");
    }

    

}
