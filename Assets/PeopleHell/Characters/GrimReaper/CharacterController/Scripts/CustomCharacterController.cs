using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class CustomCharacterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    
    [Header("Movement Parameters")]
    [SerializeField] private float _walkingSpeed;
    [SerializeField] private float _runningSpeed;
    [SerializeField] private float _maxSlopeAngle;
    
    [Header("Collision Handling Parameters")]
    [SerializeField] private LayerMask _collisionLayer;
    
    
    //Collide and Slide algorithm Parameters
    private int _csMaxBounces = 5;
    
    //Collision Handling Parameters
    private float _skinWith = 0.015f;     //Very small distance inside our collider that we start our collision check from
    private Bounds _bounds;
    
    //InputSystem InputActions
    private InputAction _moveAction;
    private InputAction _runAction;
    
    private Vector2 _moveInputValue;
    
    private Vector3 _movementVelocity;
    private Vector3 _movementDirection;
    private float _currentSpeed;
    
    private Vector3 _gravity = new Vector3(0, -9.81f, 0);
    
    //Capsule TopBottomSphereCenter (TBSC)
    private Vector3 _capsuleTBSC;
    

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
        
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _capsuleTBSC = transform.up * (_collider.height / 2f - _collider.radius);
    }

    private void Update()
    {
        ColliderBoundingBox();
        Move(_runAction.IsPressed());
    }
    
    private void Move(bool running)
    {
        //Get the speed (Scalar)
        _currentSpeed = IsRunning(running);
        
        //Get the direction (Unit vector)
        _moveInputValue =  _moveAction.ReadValue<Vector2>();
        _movementDirection= new Vector3(_moveInputValue.x, 0, _moveInputValue.y);
        
        //Create velocity (Vector)
        _movementVelocity = _movementDirection * _currentSpeed;
        
        //Update velocity
        //TODO Understand this :(
        _movementVelocity = CollideAndSlide(_movementVelocity, transform.position, 0, false, _movementVelocity);
        _movementVelocity += CollideAndSlide(_gravity, transform.position + _movementVelocity, 0, true, _gravity);
        
        transform.Translate(_movementVelocity * Time.deltaTime, Space.Self);
    }

    private float IsRunning(bool running)
    {
        return running ? _runningSpeed : _walkingSpeed;
    }
    
    //Kasper Fauerby algorithm
    private Vector3 CollideAndSlide(Vector3 velocity, Vector3 position, int recursionDepth, bool gravityPass, Vector3 initialVelocity)
    {
        if(recursionDepth >= _csMaxBounces) return Vector3.zero;
        
        float collisionCheckDistance = velocity.magnitude + _skinWith;
        
        RaycastHit hit;
        if (Physics.CapsuleCast(position + _capsuleTBSC, position - _capsuleTBSC, _bounds.extents.x, velocity.normalized, out hit, collisionCheckDistance, _collisionLayer ))
        {
            Vector3 snapToSurface = velocity.normalized * (hit.distance - _skinWith);
            Vector3 leftover = velocity - snapToSurface;
            float angle = Vector3.Angle(Vector3.up, hit.normal);

            if (snapToSurface.magnitude <= _skinWith) snapToSurface = Vector3.zero;
            
            //Normal ground / slope
            if (angle <= _maxSlopeAngle)
            {
                if(gravityPass) return snapToSurface;
                leftover = ProjectAndScaleVector(leftover, hit.normal);
            }
            //Wall or step slope
            else
            {
                float scale = 1 - Vector3.Dot(
                    new Vector3(hit.normal.x, 0, hit.normal.z).normalized, 
                    - new Vector3(initialVelocity.x, 0, initialVelocity.z).normalized);
                
                leftover = ProjectAndScaleVector(leftover, hit.normal) *  scale;
            }

            return snapToSurface + CollideAndSlide(leftover, position, recursionDepth + 1, gravityPass, initialVelocity);
        }

        return velocity;
    }

    private Vector3 ProjectAndScaleVector(Vector3 vector, Vector3 normal)
    {
        float magnitude = vector.magnitude;
        vector = Vector3.ProjectOnPlane(vector, normal).normalized;
        vector *= magnitude;
        return vector;
    }

    private void ColliderBoundingBox()
    {
        _bounds = _collider.bounds;
        _bounds.Expand(-2 * _skinWith);
    }
}
