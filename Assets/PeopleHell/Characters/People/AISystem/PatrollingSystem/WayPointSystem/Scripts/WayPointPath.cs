using System;
using UnityEngine;

public class WayPointPath : MonoBehaviour
{
    public enum PathType
    {
        Loop,
        ReverseWhenComplete
    }

    [Header("Path Parameters")]
    [SerializeField] private Transform[] _wayPoints;
    [SerializeField] private PathType _pathType = PathType.Loop;

    private int _direction = 1;
    private int _index;

    private int GetNextWaypointIndex()
    {
        //Move to the next index
        _index += _direction;

        switch(_pathType) 
        {
            case PathType.Loop:
                _index %= _wayPoints.Length;
                break;
            
            case PathType.ReverseWhenComplete:
                //Reverse direction if at bounds
                if (_index >= _wayPoints.Length || _index < 0)
                {
                    _direction *= -1;
                    _index += _direction * 2;
                }
                break;
        }
        
        return _index;
    }
    
    public Vector3 ResetWayPointPath()
    {
        _index = 0;
        return _wayPoints[_index].position;
    }
    
    // Get the next waypoint based on the path type
    public Vector3 GetNextWayPoint()
    {
        if(_wayPoints.Length == 0) return transform.position;

        _index = GetNextWaypointIndex();
        Vector3 nextWayPoint = _wayPoints[_index].position;
        
        return nextWayPoint;
    }

    private void OnDrawGizmos()
    {
        if(_wayPoints == null || _wayPoints.Length == 0) return;
        Gizmos.color = Color.white;
        
        //Draw Lines between Waypoints
        for (int i = 0; i < _wayPoints.Length - 1; i++)
        {
            Gizmos.DrawLine(_wayPoints[i].position, _wayPoints[i + 1].position);
        }
        
        //Loop back to  the start if the path is a loop
        if (_pathType == PathType.Loop)
        {
            Gizmos.DrawLine(_wayPoints[_wayPoints.Length - 1].position, _wayPoints[0].position);
        }
        
        Gizmos.color = Color.red;
        
        //Draw the Waypoints as spheres
        foreach (Transform wayPoint in _wayPoints)
        {
            Gizmos.DrawWireSphere(wayPoint.position, 0.5f);
        }
    }
}
