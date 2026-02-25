using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[RequireComponent(typeof(AwarenessSystem))]
public class HearingSensor : MonoBehaviour
{
    [Header("Parameters")]
    [field: SerializeField] public float HearingRange { get; private set; } = 15f;
    [field: SerializeField] public Color HearingRangeColour { get; private set; } = new Color(1f, 1f, 0f, 0.25f);
    
    [Header("References")]
    [SerializeField] private AwarenessSystem _awareness;

    private void Awake()
    {
        _awareness = GetComponent<AwarenessSystem>();
    }
    
    void Start()
    {
        HearingManager.Instance.Register(this);
    }

    void OnDestroy()
    {
        if(HearingManager.Instance != null) 
            HearingManager.Instance.Deregister(this);
    }
    
    public void OnHeardSound(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        //Outside of hearing range
        if(Vector3.Distance(location, _awareness.BodyLocation) > HearingRange) return;
        
        //TODO do teh cool thing aka an event or something that allows to change from state
        _awareness.ReportCanHear(source, location, category, intensity);
        
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(HearingSensor))]
public class HearingSensorEditor : Editor
{
    public void OnSceneGUI()
    {
        HearingSensor hearingSensor = (HearingSensor)target;

        // draw the hearing range
        Handles.color = hearingSensor.HearingRangeColour;
        Handles.DrawSolidDisc(hearingSensor.transform.position, Vector3.up, hearingSensor.HearingRange);
    }
}
#endif // UNITY_EDITOR
