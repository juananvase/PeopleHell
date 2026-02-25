using System.Collections.Generic;
using UnityEngine;

public class HearingManager : MonoBehaviour
{
    public static HearingManager Instance { get; private set; }

    public List<HearingSensor> AllSensors { get; private set; } =  new List<HearingSensor>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple HearingManagers found. Destroying " + gameObject.name);
            Destroy(this);
            return;
        }
        
        Instance = this;
    }

    public void Register(HearingSensor target)
    {
        AllSensors.Add(target);
    }
    
    public void Deregister(HearingSensor target)
    {
        AllSensors.Remove(target);
    }

    public void OnSoundEmitted(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        //Notify all sensors
        foreach (var sensor in AllSensors)
        {
            sensor.OnHeardSound(source, location, category, intensity);
        }
    }
}

public enum EHeardSoundCategory
{
    Footstep,
    Object,
    Scream
}
