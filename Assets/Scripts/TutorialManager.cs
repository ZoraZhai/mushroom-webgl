using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private bool touchStarted = false;

    public static bool hasTutorial = false;//ȫ�ֱ���֪���Ƿ���й����ֽ̳�

    public GameObject hand1;
    public GameObject hand2;
    private Animator hand1animator;
    private Animator hand2animator;

    public bool isStep1Completed = false;
    public bool isStep2Completed = false;
    public bool isStep3Completed = false;

    public Image stepImage;
    public Sprite step1Image;
    public Sprite step2Image;
    public Sprite step3Image;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        //��ʼ��
        isStep1Completed = false;
        isStep2Completed = false;
        isStep3Completed = false;

        stepImage.enabled = true;
        mainCamera = Camera.main;

        if (hasTutorial == false)
        {
            ShowStepImage(step1Image);
            //�����ֲ�����
            hand1.SetActive(true);
        }
        hand2.SetActive(false);
        hand1animator = hand1.GetComponent<Animator>();
        hand2animator = hand2.GetComponent<Animator>();
        hand1animator.enabled = true;
        
    }

    private void ShowStepImage(Sprite stepImages)
    {
        stepImage.sprite = stepImages;
    }
    // Update is called once per frame
    void Update()
    {
        /*if ()
        {
            // ������ǰ����
            SkipCurrentStep();
        }*/

        if (hasTutorial == true)
        {
            stepImage.enabled = false;
        }
        var touches = Touchscreen.current.touches;
        // ��ʼ��������
        int activeTouches = 0;

        // ����touches���飬���㴦�ڻ״̬�Ĵ���������
        foreach (var touch in touches)
        {
            // Check if the touch is in progress
            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (touch.isInProgress)
                {
                    activeTouches++;

                }
            }
        }


        // Step 1: ������Ļ�ƶ��ӽ�

        if (!isStep1Completed)
        {
            if (touches[0].phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                // ��������һ����־����ʾ�����ѿ�ʼ
                touchStarted = true;
            }
            else if (touches[0].phase .ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended&&touchStarted ==true)
            {
                isStep1Completed = true;
                //StartCoroutine(WaitForSeconds());
                
                hand1animator.enabled = false;
                hand1.SetActive(false);
            }
        }

        //Step2��˫ָ�Ŵ�
        if (isStep1Completed && !isStep2Completed)
        {
            ShowStepImage(step2Image);
            hand2animator.enabled = true;
            hand2.SetActive(true);
            
            if (touches[0].isInProgress && touches[1].isInProgress)
            {
                isStep2Completed = true;
                hand2animator.enabled = false;
                hand2.SetActive(false);
            }
        }

        //Step3�����Ģ��
        if (isStep2Completed && !isStep3Completed)
        {
            ShowStepImage(step3Image);
            
            if (touches[0].isInProgress)
            {
                isStep3Completed = true;
                stepImage.enabled = false;

                hasTutorial = true;//ֻ��ʾһ��������
            }
        }
    }

    //���Զ�����
    public void SkipCurrentStep()
    {
        hand1animator.enabled = false;
        hand1.SetActive(false);
        stepImage.enabled = false;
        hasTutorial = true;
    }

    IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(0.2f);
    }
}
