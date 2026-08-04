using Magicat.Helpers;
using Magicat.Input;
using System.Collections;
using Unity.VisualScripting;
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
        [SerializeField]
        [Tooltip("How long the player should stand still for the rod animation when using the rod.")]
        private float _rodAnimTime; 

        private InputAction _aButtonAction;
        private InputAction _bButtonAction;
        private InputAction _startAction;
        private InputAction _selectAction;
        private InputAction _dpadAction;

        private Player _player;
        private KinematicMovement _movement;

        private Vector2 _movementDirection;
        private Directions _facingDirection;

        private bool _isMoving;
        private bool _inAnim;
        private bool _initialized;

        // Start is called before the first frame update
        private void Start()
        {
            _player = GetComponent<Player>();
            _movement = GetComponent<KinematicMovement>();
            _facingDirection = Directions.South; // TODO: Ensure player always spawns facing south!

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
            _aButtonAction.performed -= UseRod;
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

            // Don't move during anims
            if (_inAnim)
            {
                return;
            }

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
            _aButtonAction.performed += UseRod;
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

        /// <summary>
        /// Function call for using the copy rod.
        /// Rod will perform an animation and send out a 
        /// targetted collision check in the facing direction
        /// </summary>
        private void UseRod(InputAction.CallbackContext context)
        {
            // Don't overlap anims
            if(_inAnim)
            {
                return;
            }

            _player.Anim.SetInteger("Direction", ((int)_facingDirection));
            _player.UseRod(_facingDirection);

            // Stop the movement currently happening
            StopMoving(false);
            _inAnim = true;
            StartCoroutine(RodAnimRoutine());
        }

        private void UpdateMovement()
        {
            if (_isMoving)
            {
                // Read the movement vector (L-analog or WASD)
                var direction = _dpadAction.ReadValue<Vector2>();

                // Handle anim states
                // Only update facing if a singular direction is held
                if (direction.y == 0)
                {
                    if (direction.x > 0)
                    {
                        _player.Anim.SetTrigger("onMoveRight");
                        _facingDirection = Directions.East;
                    }
                    else if (direction.x < 0)
                    {
                        _player.Anim.SetTrigger("onMoveLeft");
                        _facingDirection = Directions.West;
                    }
                }
                else if (direction.x == 0)
                {
                    if (direction.y > 0)
                    {
                        _player.Anim.SetTrigger("onMoveUp");
                        _facingDirection = Directions.North;
                    }
                    else if (direction.y < 0)
                    {
                        _player.Anim.SetTrigger("onMoveDown");
                        _facingDirection = Directions.South;
                    }
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

        /// <summary>
        /// Coroutine for managing the rod animation wait time,
        /// also known as the window where the player cannot move.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RodAnimRoutine()
        {
            yield return new WaitForSeconds(_rodAnimTime);
            _inAnim = false;
            _player.Anim.SetInteger("Direction", -1);
            _player.Anim.SetTrigger("OnAnimFinish");
        }

        public void StopMoving(bool playAnim = true)
        {
            _movement.SetVelocity(Vector3.zero);
            
            if (!playAnim)
            {
                return;
            }

            // Stop movement anim
            _player.Anim.SetTrigger("onStopMoving");
        }

        public Vector2 GetMovementDirection()
        {
            return _movementDirection;
        }
    }
}
