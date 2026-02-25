using UnityEngine;
using UnityEngine.AI;

public class ExampleContext
{
    //References
    public Transform CharacterTransform { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public NavMeshAgent NavAgent { get; private set; }
    
    //Layers
    public LayerMask GroundLayer { get; private set; }
    public LayerMask PlayerLayer { get; private set; }

    public ExampleContext(Transform characterTransform, Transform playerTransform, NavMeshAgent navAgent, LayerMask groundLayer, LayerMask playerLayer)
    {
        CharacterTransform = characterTransform;
        PlayerTransform = playerTransform;
        NavAgent = navAgent;
        GroundLayer = groundLayer;
        PlayerLayer = playerLayer;
    }
}
