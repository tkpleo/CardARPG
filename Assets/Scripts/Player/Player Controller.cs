using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    using Attack;
    using DeckBuilding;
    using DeckBuilding.Cards;

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : PlayerComponent
    {
        #region Serialized Fields
        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 360f;

        [Header("Dodge")]
        [SerializeField] private float dodgeCooldown = 1.5f;
        [SerializeField] private float dodgeTime = 0.5f;
        [SerializeField] private float dodgeSpeed = 7f;
        #endregion

        static class InputActions
        {
            public const string Move = "Move";
            public const string Look = "Look";
            public const string Interact = "Interact";
            public const string Dodge = "Dodge";
            public const string LeftCard = "Left Card";
            public const string RightCard = "Right Card";
        }

        // Input booleans
        private bool _canDodge, _dodgeInput,
            _canPlayCards, _leftCardInput, _rightCardInput;

        private bool isAttacking => Player.GetComponent<PlayerAttack>()?.isAttacking == true;

        public Camera mainCamera;
        public LayerMask groundLayer;
        public Vector2 mousePosition;

        private Vector3 _velocity;
        private InputSystem_Actions _playerInputActions;
        private Vector3 _input;
        private CharacterController _characterController;
        public Canvas _CardCanvas;
        public HandManager handManagerScript;

        public bool isMoving, isDodging;
        public event Action OnCardPlayed;
        public event Action OnDodge;


        protected override void Awake()
        {
            base.Awake();

            _playerInputActions = new InputSystem_Actions();
            _characterController = GetComponent<CharacterController>();


            _canDodge = true;
            _canPlayCards = true;
            isDodging = false;
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

            if (_dodgeInput && _canDodge && !isDodging)
            {
                StartCoroutine(Dodge());
            }

            if (_canPlayCards && !isAttacking && !isDodging)
            {
                if (_leftCardInput)       DeckManager.PlayCard(true, new CardContext());
                else if (_rightCardInput) DeckManager.PlayCard(false, new CardContext());
            }
        }

        // Prevents player from being knocked into the air
        private void LateUpdate() => transform.position = new Vector3(transform.position.x, 0, transform.position.z);


        //Handles dodging animation and logic booleans
        private IEnumerator Dodge()
        {

            _canDodge = false;
            _canPlayCards = false;
            isDodging = true;
            yield return new WaitForSeconds(dodgeTime);
            isDodging = false;
            _canPlayCards = true;
            yield return new WaitForSeconds(dodgeCooldown);
            _canDodge = true;
        }

        //Rotates player based on input unless attacking or dodging
        private void Look()
        {

            if (_input == Vector3.zero || isDodging == true) return;

            Matrix4x4 isometricMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
            Vector3 multipliedMatrix = isometricMatrix.MultiplyPoint3x4(_input);

            Quaternion rotation = Quaternion.LookRotation(multipliedMatrix, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);
        }

        //checks if player is dodging or attacking before allowing movement, forces forward movement when dodging
        private void Move()
        {
            if (isDodging == true)
            {
                _characterController.Move(transform.forward * dodgeSpeed * Time.deltaTime);
            }

            Vector3 mDirection = _input.magnitude * speed * Time.deltaTime * transform.forward + _velocity;
            _characterController.Move(mDirection);

            isMoving = _input.magnitude > 0; // Set isMoving based on input magnitude
        }

        //Gets player input from Input System
        private void GetInput()
        {
            Vector2 input = _playerInputActions.Player.Move.ReadValue<Vector2>();
            _input = new Vector3(input.x, 0, input.y);
            mousePosition = _playerInputActions.Player.Look.ReadValue<Vector2>();
            _dodgeInput = _playerInputActions.Player.Dodge.IsPressed();
            _leftCardInput = _playerInputActions.Player.LeftCard.IsPressed();
            _rightCardInput = _playerInputActions.Player.RightCard.IsPressed();
        }
    }
}
