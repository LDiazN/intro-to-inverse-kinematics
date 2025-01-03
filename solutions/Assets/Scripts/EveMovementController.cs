using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class EveMovementController : MonoBehaviour
{
    #region Inspector Properties
    // -------------------------
    [Header("Movement")] 
    [SerializeField]
    private float moveSpeed = 5;
    [SerializeField]
    private float walkingSpeed = 2.5f;
    [SerializeField]
    private float turnSpeed = 120;
    [SerializeField]
    private bool walking;
    public bool Walking { get => walking; set => walking = value; }
    
    [Header("Animation")]
    [SerializeField]
    private string speedParameterName = "Speed";
    [SerializeField]
    private string walkingParameterName = "Walking";
    // -------------------------
    #endregion
    
    #region Components
    // -------------------------
    private Rigidbody _rigidbody;
    private Animator _animator;
    private EveAimController _aim;
    // -------------------------
    #endregion
    
    #region Private State
    // -------------------------
    private float _forwardInput;
    private float _turnInput;
    
    private float _speed;
    private float _angularSpeed;

    private int _speedParameter;
    private int _walkingParameter;
    // -------------------------
    #endregion

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _speedParameter = Animator.StringToHash(speedParameterName);
        _walkingParameter = Animator.StringToHash(walkingParameterName);
        _aim = GetComponent<EveAimController>();
    }

    private void Update()
    {
        CollectInput();
        ComputeSpeed();
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void CollectInput()
    {
        _forwardInput = Input.GetAxis("Vertical");
        _turnInput = Input.GetAxisRaw("Horizontal");
    }

    private void ComputeSpeed()
    {
        var movementSpeed = walking ? walkingSpeed : moveSpeed;
        _speed = _forwardInput * movementSpeed;
        _angularSpeed = _turnInput * turnSpeed;
    }

    private void UpdateMovement()
    {
        if (_aim is not null && _aim.Aiming)
            return;
        
        _rigidbody.velocity = _speed * transform.forward;
        var newRotation = transform.rotation.eulerAngles;
        newRotation.y += _angularSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(newRotation);
    }

    private void UpdateAnimationState()
    {
        _animator.SetFloat(_speedParameter, _speed);
        _animator.SetBool(_walkingParameter, walking);
    }
}
