using UnityEngine;

public class DetectableTarget : MonoBehaviour
{
    void Start()
    {
        DetectableTargetManager.Instance.Register(this);
    }

    void OnDestroy()
    {
        if(DetectableTargetManager.Instance != null) 
            DetectableTargetManager.Instance.Deregister(this);
    }
}
