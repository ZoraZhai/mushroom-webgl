using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Image infoImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 更新信息标记的位置
        Vector3 startPos = transform.position + Vector3.up * 0.05f;
        Vector3 endPos = transform.position + Vector3.right * 0.1f + Vector3.up * 0.1f;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        // 更新信息图片的位置
        infoImage.transform.position = endPos;
    }
}
