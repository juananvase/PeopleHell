using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[RequireComponent(typeof(AwarenessSystem))]
public class VisionSensor : MonoBehaviour
{
    [Header("Parameters")]
    [field: SerializeField] public float VisionConeAngle { get; private set; } = 30f;
    [field: SerializeField] public float VisionConeRange { get; private set; } = 15f;
    [field: SerializeField] public Color VisionConeColour { get; private set; } = new Color(1f, 0f, 0f, 0.25f);
    
    [Header("Layers")]
    [SerializeField] private LayerMask _detectionLayer;
    
    public float CosVisionConeAngle { get; private set; } = 0f;
    
    [Header("References")]
    [SerializeField] private AwarenessSystem _awareness;

    private void Awake()
    {
        _awareness = GetComponent<AwarenessSystem>();
        CosVisionConeAngle = Mathf.Cos(VisionConeAngle * Mathf.Deg2Rad);
    }

    private void Update()
    {
        //Check all candidates
        for (int i = 0; i < DetectableTargetManager.Instance.AllTargets.Count; i++)
        {
            DetectableTarget candidateTarget = DetectableTargetManager.Instance.AllTargets[i];
            
            //Skip if the candidate is ourselves
            if(candidateTarget.gameObject == gameObject) continue;

            Vector3 vectorToTarget = candidateTarget.transform.position - _awareness.BodyLocation;
            
            //if out of range cannot see
            if(vectorToTarget.sqrMagnitude > (VisionConeRange * VisionConeAngle)) continue;
            
            vectorToTarget.Normalize();
            
            //If out of vision cone cannot see
            if(Vector3.Dot(vectorToTarget, _awareness.BodyDirection) < CosVisionConeAngle) continue;
            
            //Raycast to target passes?
            RaycastHit hitResult;
            if (Physics.Raycast(_awareness.BodyLocation, vectorToTarget, out hitResult, VisionConeRange, _detectionLayer, QueryTriggerInteraction.Collide))
            {
                if (hitResult.collider.GetComponentInParent<DetectableTarget>() == candidateTarget)
                {
                    //TODO do teh cool thing aka an event or something that allows to change from state
                    _awareness.ReportCanSee(candidateTarget);
                    
                }
            }
        }
    }
    
    
}

#if UNITY_EDITOR
[CustomEditor(typeof(VisionSensor))]
public class VisionSensorEditor : Editor
{
    public void OnSceneGUI()
    {
        VisionSensor visionSensor = (VisionSensor)target;

        // work out the start point of the vision cone
        Vector3 startPoint = Mathf.Cos(-visionSensor.VisionConeAngle * Mathf.Deg2Rad) * visionSensor.transform.forward +
                             Mathf.Sin(-visionSensor.VisionConeAngle * Mathf.Deg2Rad) * visionSensor.transform.right;

        // draw the vision cone
        Handles.color = visionSensor.VisionConeColour;
        Handles.DrawSolidArc(visionSensor.transform.position, Vector3.up, startPoint, visionSensor.VisionConeAngle * 2f, visionSensor.VisionConeRange);        
    }
}
#endif // UNITY_EDITOR
