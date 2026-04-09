using System;
using System.Collections;
using UnityEngine;

namespace Player
{
    using Attack;
    using DeckBuilding;
    using DeckBuilding.Cards;
    using Unity.VisualScripting;
    using UnityEngine.UI;

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
        [SerializeField] private Slider staminaSlider;

        [Header("Stamina")]
        [SerializeField] private float maxStamina = 1f; // 1f = full bar
        [SerializeField] private int dashesPerFullStamina = 3;
        [SerializeField] private float staminaRegenDelay = 1.5f;
        [SerializeField] private float staminaRegenRate = 0.5f; // per second (fraction of bar)
        private float currentStamina;
        private float lastDashTime = -999f;
        private float dashStaminaCost => maxStamina / dashesPerFullStamina;
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

            currentStamina = maxStamina;
            if (staminaSlider != null)
                staminaSlider.value = 1f;

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
                if (currentStamina >= dashStaminaCost)
                {
                    StartCoroutine(Dodge());
                }
                // else: not enough stamina to dash
            }

            // Regenerate stamina if not dashing and after delay
            if (!isDodging && currentStamina < maxStamina && Time.time - lastDashTime > staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
                if (staminaSlider != null)
                    staminaSlider.value = currentStamina / maxStamina;
            }

            if (_canPlayCards && !isAttacking && !isDodging)
            {
                if (_leftCardInput)       DeckManager.PlayCard(true, new CardContext());
                else if (_rightCardInput) DeckManager.PlayCard(false, new CardContext());
            }
        }

        // Prevents player from being knocked into the air
        private void LateUpdate() => transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        private IEnumerator FadeInStaminaBar()
        {
            if (staminaSlider != null)
            {
                staminaSlider.gameObject.SetActive(true);
                CanvasGroup canvasGroup = staminaSlider.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = staminaSlider.gameObject.AddComponent<CanvasGroup>();

                float elapsedTime = 0f;
                while (elapsedTime < 0.25f)
                {
                    canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / 0.5f);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                canvasGroup.alpha = 1f;
            }
        }

        private IEnumerator FadeOutStaminaBar()
        {
            if (staminaSlider != null)
            {
                CanvasGroup canvasGroup = staminaSlider.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = staminaSlider.gameObject.AddComponent<CanvasGroup>();

                float elapsedTime = 0f;
                while (elapsedTime < 0.5f)
                {
                    canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / 0.5f);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                canvasGroup.alpha = 0f;
                staminaSlider.gameObject.SetActive(false);
            }
        }


        //Handles dodging animation and logic booleans
        private IEnumerator Dodge()
        {
            _canDodge = false;
            _canPlayCards = false;
            isDodging = true;

            // Consume stamina for dash
            StartCoroutine(FadeInStaminaBar());
            currentStamina -= dashStaminaCost;
            currentStamina = Mathf.Max(currentStamina, 0f);
            if (staminaSlider != null)
                staminaSlider.value = currentStamina / maxStamina;
            lastDashTime = Time.time;
            

            yield return new WaitForSeconds(dodgeTime);
            isDodging = false;
            _canPlayCards = true;
            yield return new WaitForSeconds(dodgeCooldown);
            StartCoroutine(FadeOutStaminaBar());
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
                return;
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
