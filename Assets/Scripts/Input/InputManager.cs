using Magicat.Helpers;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magicat.Input
{
    public class InputManager : MonoBehaviour
    {
        // NOTE: I don't understand the event system UI shit rn so it just has the default UI map copied
        // over but I need to address this at some point because UI should be handled with the General map

        public static InputManager Instance { get { return _instance; } }

        // Action Accessors
        // Menu
        public InputAction AButtonMenu { get { return _aButtonMenu; } }
        public InputAction BButtonMenu { get { return _bButtonMenu; } }
        public InputAction StartMenu { get { return _startMenu; } }
        public InputAction SelectMenu { get { return _selectMenu; } }
        public InputAction DpadMenu {  get { return _dpadMenu; } }


        // Gameplay
        public InputAction AButtonGameplay { get { return _aButtonGameplay; } }
        public InputAction BButtonGameplay { get { return _bButtonGameplay; } }
        public InputAction StartGameplay {  get { return _startGameplay; } }
        public InputAction SelectGameplay {  get { return _selectGameplay; } }
        public InputAction DpadGameplay { get { return _dpadGameplay; } }

        public InputAction ConsoleAction { get { return _consoleAction; } }
        public InputAction PreviousSubmissionAction { get { return _previousSubmissionAction; } }
        public InputAction NextSubmissionAction { get { return _nextSubmissionAction; } }
        public InputAction CaretPositionAction { get { return _caretPositionAction; } }


        private static InputManager _instance;

        private PlayerInput _input;

        private InputActionMap[] _actionMaps;

        // Menu
        private InputAction _aButtonMenu;
        private InputAction _bButtonMenu;
        private InputAction _startMenu;
        private InputAction _selectMenu;
        private InputAction _dpadMenu;


        // Gameplay
        private InputAction _aButtonGameplay;
        private InputAction _bButtonGameplay;
        private InputAction _startGameplay;
        private InputAction _selectGameplay;
        private InputAction _dpadGameplay;

        // Console
        private InputAction _consoleAction;
        private InputAction _previousSubmissionAction;
        private InputAction _nextSubmissionAction;
        private InputAction _caretPositionAction;

        // Start is called before the first frame update
        void Start()
        {
            _actionMaps = new InputActionMap[Enum.GetNames(typeof(ActionMaps)).Length];
            foreach (var map in _input.actions.actionMaps)
            {
                switch (map.name)
                {
                    case "Gameplay":
                        _actionMaps[(int)ActionMaps.Gameplay] = map;
                        break;
                    case "Menu":
                        _actionMaps[(int)ActionMaps.Menu] = map;
                        break;
                    case "Console":
                        _actionMaps[(int)ActionMaps.Console] = map;
                        break;
                }

                // It seems like this shit is enabling all the maps at once so fix this xD
                if (map.name == _input.defaultActionMap)
                {
                    _input.currentActionMap = map;
                    map.Enable();
                }
                else
                {
                    map.Disable();
                }
            }

            // Enable console
            _actionMaps[(int)ActionMaps.Console].Enable();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
            }

            _input = GetComponent<PlayerInput>();

            _aButtonGameplay = _input.actions["A Button [Gameplay]"];
            _bButtonGameplay = _input.actions["B Button [Gameplay]"];
            _startGameplay = _input.actions["Start [Gameplay]"];
            _selectGameplay = _input.actions["Select [Gameplay]"];
            _dpadGameplay = _input.actions["Dpad [Gameplay]"];

            _aButtonMenu = _input.actions["A Button [Menu]"];
            _bButtonMenu = _input.actions["B Button [Menu]"];
            _startMenu = _input.actions["Start [Menu]"];
            _selectMenu = _input.actions["Select [Menu]"];
            _dpadMenu = _input.actions["Dpad [Menu]"];

            _consoleAction = _input.actions["Console"];
            _previousSubmissionAction = _input.actions["PreviousSubmission"];
            _nextSubmissionAction = _input.actions["NextSubmission"];
            _caretPositionAction = _input.actions["CaretPosition"];
        }

        // Update is called once per frame
        void Update()
        {
        }

        public ActionMaps GetCurrentActionMap()
        {
            if (_input.currentActionMap == _actionMaps[(int)ActionMaps.Gameplay])
            {
                return ActionMaps.Gameplay;
            }
            else if (_input.currentActionMap == _actionMaps[(int)ActionMaps.Menu])
            {
                return ActionMaps.Menu;
            }
            else if (_input.currentActionMap == _actionMaps[(int)ActionMaps.Console])
            {
                return ActionMaps.Console;
            }

            // Default
            return ActionMaps.Gameplay;
        }

        public void ToggleActionMap(ActionMaps map)
        {
            if (_input.currentActionMap == _actionMaps[(int)map])
            {
                // Already active
                return;
            }

            _input.currentActionMap.Disable();
            _actionMaps[(int)map].Enable();
            _input.currentActionMap = _actionMaps[(int)map];

            // Enable console (just in case)
            _actionMaps[(int)ActionMaps.Console].Enable();
        }

        public void ForceDisableActionMap(ActionMaps map)
        {
            _actionMaps[(int)map].Disable();
        }
    }
}
