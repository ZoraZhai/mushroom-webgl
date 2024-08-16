using UnityEngine;

public class AlwaysFaceCameraTool : MonoBehaviour
{

    public bool yAxisOnly = false;

    private Transform myTransform = null;

    void Awake()
    {
        myTransform = transform;
    }

    void Update()
    {
        if ((myTransform != null) && (UnityEngine.Camera.main != null))
        {
            if (yAxisOnly)
            {
                myTransform.LookAt(new Vector3(UnityEngine.Camera.main.transform.position.x, myTransform.position.y, UnityEngine.Camera.main.transform.position.z));
            }
            else
            {
                myTransform.LookAt(UnityEngine.Camera.main.transform);
            }
        }
    }

}
