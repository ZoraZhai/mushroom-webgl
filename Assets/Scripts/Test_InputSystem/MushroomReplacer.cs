using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MushroomReplacer : MonoBehaviour
{
    public string localAssetBundlePath = "path_to_your_assetbundle/mushroom_bundle";
    //public string prefabName = "1mushroom"; // Prefab��������AssetBundle�е�����
    private List<GameObject> newMushroomPrefab = new List<GameObject>();
    public LevelManager levelManager;

    void Start()
    {
        // �ӱ����ļ�ͬ������AssetBundle
        AssetBundle bundle = AssetBundle.LoadFromFile(localAssetBundlePath);

        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return;
        }

        // ��AssetBundle�м���Prefab��Ģ����
        //GameObject mushroomPrefab = bundle.LoadAsset<GameObject>(prefabName);
        var mushroomPrefabs = bundle.LoadAllAssets<GameObject>();

        // ɸѡ���Ѷ�Ϊ1��Ģ��Prefabs
        int goodmushroomCount = 0;
        int totalmushroomCount = 0;
        foreach (var prefab in mushroomPrefabs)
        {
            MushroomInformation difficultyComponent = prefab.GetComponent<MushroomInformation>();

            //int totalMushroom = levelManager.numberOfBoxes;
            //һ��Ҫ��3��Ģ������һ����Ģ��
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
        // ���ҳ�����������Ϊ"Cube"�Ķ��󣨻�ʹ�ñ�ǩ����������
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        

        //����滻Cube
        List<GameObject> availableMushrooms = new List<GameObject>(newMushroomPrefab);
        for (int i =0;i<cubes.Length;i++)
        {
            if (availableMushrooms.Count > 0)
            {
                int randomIndex = Random.Range(0, availableMushrooms.Count);
                GameObject mushroomToInstantiate = availableMushrooms[randomIndex];
                ReplaceCubeWithMushroom(cubes[i], mushroomToInstantiate);// ʵ����Prefab
                availableMushrooms.RemoveAt(randomIndex); // �Ƴ��Ѿ�ʹ�õ�Ģ��Prefab�������ظ�
            }
            
            //Instantiate(mushroomPrefab, cube.transform.position, cube.transform.rotation);
            
        }
    

        // ж��AssetBundle���ͷ��ڴ�,���첽���ر��asset����
        bundle.Unload(false);
    }
    private void ReplaceCubeWithMushroom(GameObject cube, GameObject mushroomPrefab)
    {
        Instantiate(mushroomPrefab, cube.transform.position, cube.transform.rotation);
        Destroy(cube);
    }


}
