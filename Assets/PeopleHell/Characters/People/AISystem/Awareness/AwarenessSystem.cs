using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameObjectUnityEvent : UnityEvent<GameObject> { }

[DisallowMultipleComponent]
public class AwarenessSystem : MonoBehaviour
{
    [Header("Vision Parameters")]
    [SerializeField] private AnimationCurve _visionSensitivity;
    [SerializeField] private float _visionMinimumAwareness = 1f;
    [Tooltip("OneSecond / BuildRate")] [SerializeField] private float _visionAwarenessBuildRate = 10f;
    
    [Header("Hearing Parameters")]
    [SerializeField] private float _hearingMinimumAwareness = 0f;
    [Tooltip("OneSecond / BuildRate")] [SerializeField] float _hearingAwarenessBuildRate = 0.5f;
    
    [Header("Proximity Parameters")]
    [SerializeField] private float _proximityMinimumAwareness = 0f;
    [Tooltip("OneSecond / BuildRate")] [SerializeField] float _proximityAwarenessBuildRate = 1f;
    
    [Header("Awareness Parameters")]
    [SerializeField] private float _awarenessDecayDelay = 0.1f;
    [Tooltip("OneSecond / BuildRate")][SerializeField] private float _awarenessDecayRate = 0.1f;
    
    [Header("Events")] 
    [SerializeField] private UnityEvent _onSuspicious;
    [SerializeField] private GameObjectUnityEvent _onDetected;
    [SerializeField] private GameObjectUnityEvent _onFullyDetected;
    [SerializeField] private GameObjectUnityEvent _onLostDetection;
    [SerializeField] private UnityEvent _onLostSuspicion;
    [SerializeField] private UnityEvent _onFullyLost;
    
    [Header("Debug")] 
    [SerializeField] private float _awarenessLevel;
    
    public Vector3 BodyLocation => transform.position;
    public Vector3 BodyDirection => transform.forward;
    
    Dictionary<GameObject, TrackedTarget> Targets = new Dictionary<GameObject, TrackedTarget>();

    private void Update()
    {
        DecayAwareness();
    }
    
    private void UpdateAwareness(GameObject targetGameObject, DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        //Not in targets
        if (!Targets.ContainsKey(targetGameObject)) 
            Targets[targetGameObject] = new TrackedTarget();
        
        //Update target awareness
        if (Targets[targetGameObject].UpdateAwareness(target, position, awareness, minAwareness))
        {
            _awarenessLevel =  Targets[targetGameObject].Awareness;//Debug
            
            if (Targets[targetGameObject].Awareness >= 2f) _onFullyDetected.Invoke(targetGameObject);
            else if (Targets[targetGameObject].Awareness >= 1f) _onDetected.Invoke(targetGameObject);
            else _onSuspicious.Invoke();
        }
    }

    private void DecayAwareness()
    {
        List<GameObject> toCleanUp = new List<GameObject>();
        foreach (GameObject targetGameObject in Targets.Keys)
        {
            if (Targets[targetGameObject].DecayAwareness(_awarenessDecayDelay, _awarenessDecayRate * Time.deltaTime))
            {
                if (Targets[targetGameObject].Awareness <= 0f)
                {
                    _onFullyLost.Invoke();
                    toCleanUp.Add(targetGameObject);
                }
                else
                {
                    _awarenessLevel =  Targets[targetGameObject].Awareness;//Debug
                    
                    if(Targets[targetGameObject].Awareness >= 1f) _onLostDetection.Invoke(targetGameObject);
                    else _onLostSuspicion.Invoke();
                }
            }
        }
        
        //Cleanup targets that are no longer detected
        foreach (GameObject targetGameObject in toCleanUp)
        {
            Targets.Remove(targetGameObject);
        }
    }

    public void ReportCanSee(DetectableTarget target)
    {
        //Determine where the target is in the flied of view
        Vector3 vectorToTarget = (target.transform.position - BodyLocation).normalized;
        float dotProduct = Vector3.Dot(vectorToTarget, BodyDirection);
        
        //Determine the awareness contribution
        float awareness = _visionSensitivity.Evaluate(dotProduct) * _visionAwarenessBuildRate * Time.deltaTime;

        UpdateAwareness(target.gameObject, target, target.transform.position, awareness, _visionMinimumAwareness);
    }

    public void ReportCanHear(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        float awareness = intensity * _hearingAwarenessBuildRate * Time.deltaTime;
        
        UpdateAwareness(source, null, location, awareness, _hearingMinimumAwareness);
    }
    
    public void ReportInProximity(DetectableTarget target)
    {
        float awareness = _proximityAwarenessBuildRate * Time.deltaTime;
        
        UpdateAwareness(target.gameObject, target, target.transform.position, awareness, _proximityMinimumAwareness);
    }
    
}

public class TrackedTarget
{
    public DetectableTarget Detectable;
    public Vector3 RawPosition;

    public float LastSenseTime = -1f;
    public float Awareness; // 0    = no aware (will be culled);
                            // 0-1  = rough idea (no set location);
                            // 1-2  = likely target (location);
                            // 2    = fully detected;

    public bool UpdateAwareness(DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        float oldAwareness = Awareness;
        
        if(target != null) Detectable = target;
        
        RawPosition = position;
        LastSenseTime = Time.time;
        Awareness = Mathf.Clamp(MathF.Max(Awareness, minAwareness) + awareness, 0f, 2f);

        if (oldAwareness < 2f && Awareness >= 2f) return true;
        if (oldAwareness < 1f && Awareness >= 1f) return true;
        if (oldAwareness <= 0f && Awareness >= 0f) return true;
        
        return false;
    }

    public bool DecayAwareness(float decayTime, float amount)
    {
        //Detected too recently then no change
        if(Time.time - LastSenseTime < decayTime) return false;
        
        float oldAwareness = Awareness;
        Awareness -= amount;
        
        if (oldAwareness >= 2f && Awareness < 2f) return true;
        if (oldAwareness >= 1f && Awareness < 1f) return true;
        
        return Awareness <= 0f;
    }
}
