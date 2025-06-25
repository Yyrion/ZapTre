using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;

    //Movement Management
    public float MoveSpeed;
    private InputAction _moveAction;
    private Vector3 _moveDirection;

    //Drag Management
    public float GroundDrag;

    public LayerMask GroundLayer;
    private float _playerHeight = 2f;
    public bool IsGrounded;

    //Animation Management
    private Animator _animator;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _moveAction = InputSystem.actions.FindAction("Move");
        _animator = GetComponent<Animator>();

        if (_moveAction == null)
        {
            Debug.LogError("Move action not found in Input System!");
        }
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight, GroundLayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (!MasterScript.Master.IsGameOver)
        {
            float rayRadius = 0.4f;
            float rayDistance = 0.65f; // Juste sous les pieds
            Vector3 origin = transform.position + Vector3.up;

            IsGrounded = Physics.SphereCast(origin, rayRadius, Vector3.down, out _, rayDistance, GroundLayer);
            Debug.DrawRay(origin, Vector3.down, Color.red);
            // Mouvement
            Vector2 moveValue = _moveAction.ReadValue<Vector2>();
            if (moveValue.magnitude > 0.2f)
            {
                _moveDirection = (transform.forward * moveValue.y + transform.right * moveValue.x) * MoveSpeed;
                _animator.SetBool("IsMoving", true);
            }
            else
            {
                _moveDirection = Vector3.zero;
                _animator.SetBool("IsMoving", false);
            }

            _rb.AddForce(_moveDirection, ForceMode.Force);
            SpeedControl();
            _rb.linearDamping = IsGrounded ? GroundDrag : 0f;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        if (flatVelocity.magnitude >MoveSpeed)
        {
            flatVelocity = flatVelocity.normalized * MoveSpeed;
            _rb.linearVelocity = new Vector3(flatVelocity.x, _rb.linearVelocity.y, flatVelocity.z);
        }
    }

}
