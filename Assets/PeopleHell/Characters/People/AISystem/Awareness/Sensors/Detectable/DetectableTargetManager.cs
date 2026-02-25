using System.Collections.Generic;
using UnityEngine;

public class DetectableTargetManager : MonoBehaviour
{
    public static DetectableTargetManager Instance { get; private set; }

    public List<DetectableTarget> AllTargets { get; private set; } =  new List<DetectableTarget>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple DetectableTargetManagers found. Destroying " + gameObject.name);
            Destroy(this);
            return;
        }
        
        Instance = this;
    }

    public void Register(DetectableTarget target)
    {
        AllTargets.Add(target);
    }
    
    public void Deregister(DetectableTarget target)
    {
        AllTargets.Remove(target);
    }

}
