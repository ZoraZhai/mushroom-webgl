using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadFile : MonoBehaviour
{
    private string releaseContentUrl = "https://a.unity.cn/client_api/v1/buckets/aabb899d-3265-4b5d-a3f8-a142c3e2ac08/content/defaultlocalgroup_assets_all_e2dbc6868a29b2960f6ea47c489a422f.bundle";
    private string mushroomLabel = "mushroom";
    void Start()
    {
        StartCoroutine(GetReleaseContentFromURL());
    }

    IEnumerator GetReleaseContentFromURL()
    {
        UnityWebRequest www = UnityWebRequest.Get(releaseContentUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            // �����ص����ݣ�������Ը���ʵ�ʷ��ص����ݸ�ʽ���н���
            Debug.Log("Error: " + www.error);
            // ��ӡ���������صĴ�����Ϣ
            Debug.Log("Server Response: " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log("Server Response: " + www.downloadHandler.text);

            AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(mushroomLabel, null);
            handle.Completed += OnMushroomsLoaded;
            
        }
    }

    private void OnMushroomsLoaded(AsyncOperationHandle<IList<GameObject>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (GameObject mushroom in handle.Result)
            {
                // ʵ����ÿһ��Ģ��ģ��
                Instantiate(mushroom, RandomPosition(), Quaternion.identity);
            }
        }
        else
        {
            Debug.LogError("Ģ��ģ�ͼ���ʧ��: " + handle.OperationException.ToString());
        }
    }

    Vector3 RandomPosition()
    {
        return new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));

    }
}