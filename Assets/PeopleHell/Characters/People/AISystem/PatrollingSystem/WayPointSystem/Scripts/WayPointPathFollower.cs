using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
public class WayPointPathFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WayPointPath _path;
    [SerializeField] private NavMeshAgent _agent;
    
    [Header("Waypoint Parameters")]
    [SerializeField] private float _waitTimeOnWaypoint = 0;

    private float _time = 0f;

    private void Awake()
    {
        _agent = _agent.GetComponent<NavMeshAgent>();
    }

    public void ChangeWayPointPath(WayPointPath path)
    {
        _path = path;
    }

    //Use this method on Start if there is no other class doing it
    public void StartNewWayPointPath()
    {
        _agent.destination = _path.ResetWayPointPath();
    }

    public void FollowPath()
    {
        if (_agent.remainingDistance <= 0.1f)
        {
            _time+=Time.deltaTime;
            
            if (_time >= _waitTimeOnWaypoint)
            {
                _time = 0;
                _agent.destination = _path.GetNextWayPoint();
            }
        }
    }
}
