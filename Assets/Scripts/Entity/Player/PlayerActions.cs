using Magicat.Helpers;
using Magicat.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magicat.Entity.Player
{
    [RequireComponent(typeof(KinematicMovement))]
    [RequireComponent(typeof(Player))]
    public class PlayerActions : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The offset of the hitbox. This should be the center of the player!!")]
        private Vector3 _offset;

        private InputAction _aButtonAction;
        private InputAction _bButtonAction;
        private InputAction _startAction;
        private InputAction _selectAction;
        private InputAction _dpadAction;

        private Player _player;
        private KinematicMovement _movement;

        private Vector2 _movementDirection;

        private bool _isMoving;
        private bool _initialized;

        // Start is called before the first frame update
        private void Start()
        {
            _player = GetComponent<Player>();
            _movement = GetComponent<KinematicMovement>();

            // Default value (dash up if no input has been pushed)
            _movementDirection = Vector2.up;

            if (!_initialized)
            {
                SetInputActions();
            }
        }

        private void OnEnable()
        {
            if (InputManager.Instance != null)
            {
                SetInputActions();
                _initialized = true;
            }
        }

        private void OnDisable()
        {
            // Remove delegates
            _dpadAction.performed -= OnMove;
            _dpadAction.canceled -= OnMove;
            _dpadAction.Disable();

            // TODO: Other actions
            _aButtonAction.Disable();
            _bButtonAction.Disable();
            _startAction.Disable();
            _selectAction.Disable();

            _initialized = false;
        }

        private void Update()
        {
            // Don't move during cutscenes
            /*if (GameManager.Instance.InCutscene)
            {
                return;
            }*/

            UpdateMovement();
        }

        private void SetInputActions()
        {
            // TLDR: started = animations, performed = start input, cancelled = stop input
            // context is just which one was passed in

            _dpadAction = InputManager.Instance.DpadGameplay;
            _dpadAction.Enable();
            _dpadAction.performed += OnMove;
            _dpadAction.canceled += OnMove;

            _aButtonAction = InputManager.Instance.AButtonGameplay;
            //_aButtonAction.performed +=
            _aButtonAction.Enable();

            _bButtonAction = InputManager.Instance.BButtonGameplay;
            //_bButtonAction.performed += 
            _bButtonAction.Enable();

            _startAction = InputManager.Instance.StartGameplay;
            //_startAction.performed +=
            _startAction.Enable();

            _selectAction = InputManager.Instance.SelectGameplay;
            ///_selectAction.performed +=
            _selectAction.Enable();
        }

        private void UpdateMovement()
        {
            if (_isMoving)
            {
                // Read the movement vector (L-analog or WASD)
                var direction = _dpadAction.ReadValue<Vector2>();

                // Flip sprite
                // Don't change if 0 (neutral)
                if (direction.x > 0.0f)
                {
                    // Moving right (default)
                    _player.Sprite.flipX = false;
                }
                else if (direction.x < 0.0f)
                {
                    // Moving left
                    _player.Sprite.flipX = true;
                }

                // Basic movement (fix this later)
                _movement.SetVelocity(direction * _player.speed);

                // Update our direction
                _movementDirection = direction.normalized;
            }
            else
            {
                StopMoving();
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _isMoving = true;
            }
            else if (context.canceled)
            {
                _isMoving = false;
            }
        }

        public void StopMoving()
        {
            _movement.SetVelocity(Vector3.zero);
        }

        public Vector2 GetMovementDirection()
        {
            return _movementDirection;
        }
    }
}
