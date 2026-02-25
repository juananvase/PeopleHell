using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[RequireComponent(typeof(AwarenessSystem))]
public class ProximitySensor : MonoBehaviour
{
    [Header("Parameters")]
    [field: SerializeField] public float ProximityDetectionRange { get; private set; } = 1.5f;
    [field: SerializeField] public Color ProximityDetectionColour { get; private set; } = new Color(1f, 1f, 1f, 0.25f);
    
    [Header("References")]
    [SerializeField] private AwarenessSystem _awareness;

    private void Awake()
    {
        _awareness = GetComponent<AwarenessSystem>();
    }

    private void Update()
    {
        for (int i = 0; i < DetectableTargetManager.Instance.AllTargets.Count ; i++)
        {
            DetectableTarget candidateTarget = DetectableTargetManager.Instance.AllTargets[i];
            
            //Skip if the candidate is ourselves
            if(candidateTarget.gameObject == gameObject) continue;

            if (Vector3.Distance(_awareness.BodyLocation, candidateTarget.transform.position) <= ProximityDetectionRange)
            {
                //TODO do teh cool thing aka an event or something that allows to change from state
                _awareness.ReportInProximity(candidateTarget);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ProximitySensor))]
public class EnemyAIEditor : Editor
{
    public void OnSceneGUI()
    {
        ProximitySensor proximitySensor = (ProximitySensor)target;

        // draw the detection range
        Handles.color = proximitySensor.ProximityDetectionColour;
        Handles.DrawSolidDisc(proximitySensor.transform.position, Vector3.up, proximitySensor.ProximityDetectionRange);
        
    }
}
#endif // UNITY_EDITOR
