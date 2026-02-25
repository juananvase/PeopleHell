using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CustomCharacterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    
    [Header("Movement Parameters")]
    [SerializeField] private float _walkingSpeed;
    [SerializeField] private float _runningSpeed;
    
    //InputSystem InputActions
    private InputAction _moveAction;
    private InputAction _runAction;
    
    private Vector2 _moveInputValue;
    
    private float _currentSpeed;

    private void OnEnable()
    {
        _inputActions.FindActionMap("Player").Enable();
    }
    
    private void OnDisable()
    {
        _inputActions.FindActionMap("Player").Disable();
    }
    
    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _runAction = InputSystem.actions.FindAction("Run");
        
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move(_runAction.IsPressed());
    }

    private void Move(bool running )
    {
        _currentSpeed = IsRunning(running);
        
        _moveInputValue =  _moveAction.ReadValue<Vector2>();
        Vector3 movementDirection = new Vector3(_moveInputValue.x, 0, _moveInputValue.y);
        
        transform.Translate(movementDirection.normalized * (_currentSpeed * Time.deltaTime), Space.Self);
    }

    private float IsRunning(bool running)
    {
        return running ? _runningSpeed : _walkingSpeed;
    }
    
}
