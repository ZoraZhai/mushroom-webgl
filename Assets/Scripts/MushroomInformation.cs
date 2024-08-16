using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MushroomInformation : MonoBehaviour
{
    public int difficultyLevel;
    public bool isPoisonous;
    public string mushroomName;
    public string description;
    public Sprite spriteToLoad;
    public float zoomOutDistance;

    private LevelManager1 levelManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject ScriptManager = GameObject.Find("LevelManager");
        levelManager = ScriptManager.GetComponent<LevelManager1>();

        zoomOutDistance = levelManager.zoomOutDistance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
