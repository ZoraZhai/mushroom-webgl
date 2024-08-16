using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MushroomReplacer : MonoBehaviour
{
    public string localAssetBundlePath = "path_to_your_assetbundle/mushroom_bundle";
    //public string prefabName = "1mushroom"; // Prefab的名称在AssetBundle中的名称
    private List<GameObject> newMushroomPrefab = new List<GameObject>();
    public LevelManager levelManager;

    void Start()
    {
        // 从本地文件同步加载AssetBundle
        AssetBundle bundle = AssetBundle.LoadFromFile(localAssetBundlePath);

        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return;
        }

        // 从AssetBundle中加载Prefab（蘑菇）
        //GameObject mushroomPrefab = bundle.LoadAsset<GameObject>(prefabName);
        var mushroomPrefabs = bundle.LoadAllAssets<GameObject>();

        // 筛选出难度为1的蘑菇Prefabs
        int goodmushroomCount = 0;
        int totalmushroomCount = 0;
        foreach (var prefab in mushroomPrefabs)
        {
            MushroomInformation difficultyComponent = prefab.GetComponent<MushroomInformation>();

            //int totalMushroom = levelManager.numberOfBoxes;
            //一定要在3个蘑菇中有一个好蘑菇
            if (totalmushroomCount < 3)
            {
                if (goodmushroomCount == 0 && difficultyComponent.isPoisonous == false)
                {
                    newMushroomPrefab.Add(prefab);
                    goodmushroomCount++;
                    totalmushroomCount++;
                }
                else if (goodmushroomCount == 1 && difficultyComponent.isPoisonous == true)
                {
                    newMushroomPrefab.Add(prefab);
                    totalmushroomCount++;
                }
            }
        }
        // 查找场景中所有名为"Cube"的对象（或使用标签进行搜索）
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        

        //随机替换Cube
        List<GameObject> availableMushrooms = new List<GameObject>(newMushroomPrefab);
        for (int i =0;i<cubes.Length;i++)
        {
            if (availableMushrooms.Count > 0)
            {
                int randomIndex = Random.Range(0, availableMushrooms.Count);
                GameObject mushroomToInstantiate = availableMushrooms[randomIndex];
                ReplaceCubeWithMushroom(cubes[i], mushroomToInstantiate);// 实例化Prefab
                availableMushrooms.RemoveAt(randomIndex); // 移除已经使用的蘑菇Prefab，避免重复
            }
            
            //Instantiate(mushroomPrefab, cube.transform.position, cube.transform.rotation);
            
        }
    

        // 卸载AssetBundle以释放内存,（异步加载别的asset？）
        bundle.Unload(false);
    }
    private void ReplaceCubeWithMushroom(GameObject cube, GameObject mushroomPrefab)
    {
        Instantiate(mushroomPrefab, cube.transform.position, cube.transform.rotation);
        Destroy(cube);
    }


}
