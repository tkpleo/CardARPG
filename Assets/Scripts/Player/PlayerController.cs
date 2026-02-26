using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Dodge")]
    [SerializeField] private float dodgeCooldown = 1.5f;
    [SerializeField] private float dodgeTime = 0.5f;
    [SerializeField] private float dodgeSpeed = 7f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackTime = 0.2f;

    private bool _canDodge;
    private bool _isDodging;
    private bool _dodgeInput;

    private bool _canAttack;
    private bool _isAttacking;
    private bool _attackInput;

    //Inputs for playing cards from hand
    private bool _card1Input;
    private bool _card2Input;
    private bool _card3Input;
    private bool _card4Input;
    private bool _card5Input;

    private bool _redrawInput;

    public Camera mainCamera;
    public LayerMask groundLayer;
    public Vector2 mousePosition;

    private Animator animator;
    private Vector3 _velocity;
    private InputSystem_Actions _playerInputActions;
    private Vector3 _input;
    private CharacterController _characterController;
    private PlayerAttack playerAttack;
    public Canvas _CardCanvas;
    public HandManager handManagerScript;

    private void Awake()
    {
        _playerInputActions = new InputSystem_Actions();
        _characterController = GetComponent<CharacterController>();
        playerAttack = GetComponent<PlayerAttack>();


        _canAttack = true;
        _canDodge = true;
        _isDodging = false;
        _isAttacking = false;
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
    }

    private void Update()
    {
        GetInput();

        Look();

        Move();

        if (_card1Input || _card2Input || _card3Input || _card4Input || _card5Input)
        {
            CardPlay();
        }

        if (_redrawInput)
        {
            handManagerScript.RedrawHand();
        }

        if (_dodgeInput && _canDodge && _isDodging == false)
        {
            StartCoroutine(Dodge());
        }
        
        if (_attackInput && _canAttack && _isAttacking == false)
        {
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 lookAtPoint = hitInfo.point;
                Vector3 direction = lookAtPoint - transform.position;
                direction.y = 0;
               

                if (direction != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = rotation;
                }
                _isAttacking = true;
                _canAttack = false;
                StartCoroutine(Attack());
                handManagerScript.RedrawIfBuffed();
                playerAttack.InitAttack();
                //Debug.DrawLine(ray.origin, hitInfo.point, Color.red);

            }
        }

    }

    // Prevents player from being knocked into the air
    private void LateUpdate() => transform.position = new Vector3(transform.position.x, 0, transform.position.z);

    private void CardPlay()
    {
        if (_card1Input)
        {
            handManagerScript.PlayCard(1);
        }
        if (_card2Input)
        {
            handManagerScript.PlayCard(2);
        }
        if (_card3Input)
        {
            handManagerScript.PlayCard(3);
        }
        if (_card4Input)
        {
            handManagerScript.PlayCard(4);
        }
        if (_card5Input)
        {
            handManagerScript.PlayCard(5);
        }
    }

    //Handles attacking animation and logic booleans
    private IEnumerator Attack()
    {
        animator.SetBool("isAttacking", _isAttacking);
        yield return new WaitForSeconds(attackTime);
        _isAttacking = false;
        animator.SetBool("isAttacking", _isAttacking);
        yield return new WaitForSeconds(attackCooldown);
        _canAttack = true;

    }

    //Handles dodging animation and logic booleans
    private IEnumerator Dodge()
    {
        
        _canDodge = false;
        _canAttack = false;
        _isDodging = true;
        animator.SetBool("isDodging", _isDodging);
        yield return new WaitForSeconds(dodgeTime);
        _isDodging = false;
        animator.SetBool("isDodging", _isDodging);
        _canAttack = true;
        yield return new WaitForSeconds(dodgeCooldown);
        _canDodge = true;
    }

    //Rotates player based on input unless attacking or dodging
    private void Look()
    {

        if (_input == Vector3.zero || _isDodging == true || _isAttacking == true)
        {
            return;
        }
        Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        Vector3 multipliedMatrix = isometricMatrix.MultiplyPoint3x4(_input);

        Quaternion rotation = Quaternion.LookRotation(multipliedMatrix, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);
    }

    //checks if player is dodging or attacking before allowing movement, forces forward movement when dodging
    private void Move()
    {
        if (_isDodging == true)
        {
            _characterController.Move(transform.forward * dodgeSpeed * Time.deltaTime);
        }
        if (_isAttacking == true)
        {
            return;
        }
        Vector3 mDirection = transform.forward * speed * _input.magnitude * Time.deltaTime + _velocity;
        _characterController.Move(mDirection);
        if (_input == Vector3.zero)
        {
            animator.SetBool("isRunning", false);
            return;
        }
        else
        {
            animator.SetBool("isRunning", true);
        }
    }

    //Gets player input from Input System
    private void GetInput()
    {
        Vector2 input = _playerInputActions.Player.Move.ReadValue<Vector2>();
        _input = new Vector3(input.x, 0, input.y);
        _dodgeInput = _playerInputActions.Player.Dodge.IsPressed();
        _attackInput = _playerInputActions.Player.Attack.IsPressed();
        mousePosition = _playerInputActions.Player.AttackPos.ReadValue<Vector2>();
        _card1Input = _playerInputActions.Player.Card1.triggered;
        _card2Input = _playerInputActions.Player.Card2.triggered;
        _card3Input = _playerInputActions.Player.Card3.triggered;
        _card4Input = _playerInputActions.Player.Card4.triggered;
        _card5Input = _playerInputActions.Player.Card5.triggered;
        _redrawInput = _playerInputActions.Player.Redraw.triggered;

        //Debug.Log(_input); Uncomment to see input vector in console
    }
}
