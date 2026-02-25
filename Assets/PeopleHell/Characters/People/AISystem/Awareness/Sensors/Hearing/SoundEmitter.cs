using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private EHeardSoundCategory _soundCategory;
    [SerializeField] private float _soundIntensity;
    [SerializeField] private KeyCode _keyToMakeSound;
    
    private Vector3 _soundLocation => transform.position;
    void Update()
    {
        if (Input.GetKeyDown(_keyToMakeSound))
        {
            EmmitSound(_soundCategory, _soundIntensity);
        }
    }

    public void EmmitSound(EHeardSoundCategory soundCategory, float soundIntensity)
    {
        Debug.Log($"Sound Emitted by {gameObject.name}");
        HearingManager.Instance.OnSoundEmitted(gameObject, _soundLocation, soundCategory, soundIntensity);
    }
}
