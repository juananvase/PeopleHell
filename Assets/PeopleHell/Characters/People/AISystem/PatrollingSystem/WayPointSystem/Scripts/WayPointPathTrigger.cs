using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class WayPointPathUnityEvent : UnityEvent<WayPointPath> { }
public class WayPointPathTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WayPointPath _pathOwner;
    [SerializeField] private BoxCollider _boxCollider;
    
    [Header("Events")]
    [SerializeField] private WayPointPathUnityEvent _onChangingPath;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _onChangingPath.Invoke(_pathOwner);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawWireCube(_boxCollider.center, _boxCollider.size);
    }
}
