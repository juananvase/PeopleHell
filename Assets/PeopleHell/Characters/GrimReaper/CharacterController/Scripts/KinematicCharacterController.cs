using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class KinematicCharacterController : MonoBehaviour
{
    
#region SerializeFields
    [Header("References")]
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    
    [Header("Movement Parameters")]
    [SerializeField] private float _walkingSpeed;
    [SerializeField] private float _runningSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _maxSlopeAngle;
    
    [Header("Collision Handling Parameters")]
    [SerializeField] private LayerMask _collisionLayer;
#endregion
    
#region Collide and Slide variables
    private int _csMaxBounces = 3;
#endregion
    
#region Collision Handling Parameters
    private float _skinWith = 0.015f;   //Very small distance inside our collider that we start our collision check from
    private Bounds _bounds;
    
    private Vector3 _capsuleTBSC;   //Capsule TopBottomSphereCenter (TBSC)
#endregion
    
#region InputSystem InputActions
    private InputAction _moveAction;
    private InputAction _runAction;
#endregion

#region Movement variables
    private Vector3 _movementDirection;
    
    private Vector3 _movementVelocity;
    private Vector3 _gravity = new Vector3(0, -9.81f, 0);
#endregion


#region Unity Methods
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
        
        _rigidbody.isKinematic = true;
        
        ReduceColliderBoundsBySkinWidth();
    }

    private void Start()
    {
        _capsuleTBSC = transform.up * (_collider.height / 2f - _collider.radius);
    }

    private void Update()
    {
        //Get the speed (Scalar)
        float currentSpeed = IsRunning(_runAction.IsPressed());
        
        Move(currentSpeed);
        Rotate(_movementDirection, _rotationSpeed);
        
        ApplyGravity();
    }
#endregion

#region Movement Methods
    private void Move(float speed)
    {
        //Get the direction (Unit vector)
        Vector2 moveInputValue =  _moveAction.ReadValue<Vector2>();
        _movementDirection = new Vector3(moveInputValue.x, 0, moveInputValue.y);
            
        //Create velocity (Vector)
        _movementVelocity = _movementDirection * speed;

        Vector3 projectedVelocity = CollideAndSlide(_movementVelocity * Time.deltaTime, transform.position);
        
        transform.Translate(projectedVelocity, Space.World);
    }

    private void Rotate(Vector3 movementDirection, float speed)
    {
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);
        }
    }

    private float IsRunning(bool running)
    {
        return running ? _runningSpeed : _walkingSpeed;
    }
    
    private void ApplyGravity()
    {
        Vector3 projectedGravityVelocity = CollideAndSlide(_gravity * Time.deltaTime, transform.position, 0, true);
        transform.Translate(projectedGravityVelocity ,Space.World);
    }
    
    /// <summary>
    /// Kasper Fauerby algorithm Collision response
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="position"></param>
    /// <param name="initialVelocity"></param>
    /// <param name="recursionDepth"></param>
    /// <param name="gravityPass"></param>
    /// <returns>Velocity projected to a plane</returns>
    private Vector3 CollideAndSlide(Vector3 velocity, Vector3 position, int recursionDepth = 0, bool gravityPass = false)
    {
        if (recursionDepth >= _csMaxBounces) return Vector3.zero;
        
        float collisionCheckDistance = velocity.magnitude + _skinWith;
        Vector3 normalizeVelocity = velocity.normalized;
        
        RaycastHit plane;
        if (Physics.CapsuleCast(position + _capsuleTBSC, position - _capsuleTBSC, _bounds.extents.x,
                normalizeVelocity, out plane, collisionCheckDistance, _collisionLayer))
        {
            Vector3 distanceToSurface = normalizeVelocity * (plane.distance - _skinWith);
            Vector3 remainingVelocity = velocity - distanceToSurface;
            
            if(distanceToSurface.magnitude <= _skinWith) distanceToSurface = Vector3.zero;  //Make sure that have enough room for the collision check to work properly
            
            Vector3 planeNormal = plane.normal;
            float angle = Vector3.Angle(Vector3.up, planeNormal);
            
            //Ground and Climbable Slopes
            if (angle <= _maxSlopeAngle)
            {
                if (gravityPass) return distanceToSurface;
                
                remainingVelocity = ProjectAndScale(remainingVelocity, planeNormal);
            }
            //Walls and Steep Slopes-(Treat them like walls)
            else
            {
                Vector3 horizontalPlaneNormal = new Vector3(planeNormal.x, 0, planeNormal.z);
                
                float scale = 1 - Vector3.Dot(horizontalPlaneNormal, -new Vector3(velocity.x, 0, velocity.z));
                
                remainingVelocity = ProjectAndScale(new Vector3(remainingVelocity.x, 0, remainingVelocity.z), horizontalPlaneNormal) * scale;
            }

            return distanceToSurface + CollideAndSlide(remainingVelocity, position + distanceToSurface, recursionDepth + 1, gravityPass);
        }
        
        return velocity;
    }
    
    /// <summary>
    /// Takes a velocity that overlap with a plane and projects it on a plane with the right scale 
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="normal"></param>
    /// <returns>Projected Velocity</returns>
    private Vector3 ProjectAndScale(Vector3 velocity, Vector3 normal)
    {
        float velocityMagnitude = velocity.magnitude;
        velocity = Vector3.ProjectOnPlane(velocity, normal).normalized;
        velocity *= velocityMagnitude;
        return velocity;
    }
#endregion

#region Collision Methods
    private void ReduceColliderBoundsBySkinWidth()
    {
        _bounds = _collider.bounds;
        _bounds.Expand(-2 * _skinWith);
    }
#endregion

#if UNITY_EDITOR
#region Debug Functions
    private void OnDrawGizmos()
    {
        Vector3 collisionCheckDistance = _movementVelocity.normalized*(_movementVelocity.magnitude + _skinWith);
        Gizmos.color = Color.violetRed;
        Gizmos.DrawWireSphere(transform.position + _capsuleTBSC + collisionCheckDistance, _bounds.extents.x);
        Gizmos.DrawWireSphere(transform.position - _capsuleTBSC + collisionCheckDistance, _bounds.extents.x);
        Gizmos.DrawLine(transform.position,transform.position + collisionCheckDistance);
    }
#endregion
#endif
}
