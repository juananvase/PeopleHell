using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

[RequireComponent(typeof(NavMeshAgent))]
public class ExampleStateMachine : StateManager<ExampleStateMachine.EExampleState>
{
    //States Keys
    public enum EExampleState
    {
        Idle,
        Patrolling
    }
    
    private ExampleContext _context;    //Context
    private ExampleBaseState _currentExampleState;
    
    [Header("References")]
    [SerializeField] private Transform _characterTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private NavMeshAgent _navAgent;
    
    [Header("Layers")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _playerLayer;
    
    private void Awake()
    {
        ValidateParameters();
        _context = new ExampleContext(_characterTransform, _playerTransform,  _navAgent, _groundLayer, _playerLayer);
        InitializeStates();
    }
    
    private void ValidateParameters()
    {
        //TODO finish this validations
        Assert.IsNotNull(_characterTransform, "Character Transform must be set");
        Assert.IsNotNull(_playerTransform, "Player Transform must be set");
        Assert.IsNotNull(_navAgent, "Nav Agent must be set");
    }
    
    private void InitializeStates()
    {
        //Add States to inherited StateManager "States" dictionary and Set Initial State
        States.Add(EExampleState.Idle, new ExampleIdleState(EExampleState.Idle, _context));
        
        CurrentState = States[EExampleState.Idle]; //Change initial State over here
    }
    
    //Special States for this machine

    public void DoTheCoolThing(GameObject coolGameObject)
    {
        _currentExampleState = (ExampleBaseState) CurrentState;
        _currentExampleState.DoTheCoolThing(coolGameObject);
    }

}
