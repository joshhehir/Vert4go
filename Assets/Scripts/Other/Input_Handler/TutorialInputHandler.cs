using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Collections;
using TMPro;

namespace FPSController
{
    public class TutorialInputHandler : MonoBehaviour
    {
        private FirstPersonController fpsController;
        private CameraController cameraController;
        private Leaning leaning;
        private Dodge dodge;
        private Slide slide;

        Gamepad gamepad;
        Keyboard keyboard;
        Mouse mouse;

        [SerializeField] private bool toggleRun;
        [SerializeField] private bool toggleZoom;
        bool isLookingAround;
        bool isMoving;

        [Header("Spray")]
        public GameObject spray;
        public float sprayRange;
        public AudioClip spraySFX;
        private AudioSource audioSource;
        public int sprayAmount;

        [Space, Header("Input Data")]
        [SerializeField] private CameraInputData cameraInputData = null;
        [SerializeField] private MovementInputData movementInputData = null;

        void Awake()
        {
            fpsController = GetComponent<FirstPersonController>();
            cameraController = GetComponentInChildren<CameraController>();
            leaning = GetComponent<Leaning>();
            dodge = GetComponent<Dodge>();
            slide = GetComponent<Slide>();
            movementInputData.ResetInput();
            LeanTween.reset();
            InitialInput();
        }

        void Start()
        {
            cameraInputData.ResetInput();
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            gamepad = Gamepad.current;
            keyboard = Keyboard.current;
            mouse = Mouse.current;

            if (fpsController.m_currentSpeed <= 0.5f || fpsController.m_inputVector.y < .5f || fpsController.m_hitWall)
            {
                movementInputData.IsRunning = false;
                movementInputData.RunReleased = true;
            }

            if (movementInputData.IsRunning)
                movementInputData.IsCrouching = false;

            if (!toggleRun)
                movementInputData.IsRunning = movementInputData.RunHeld;

            if (!isMoving)
            {
                float currentX = movementInputData.InputVector.x;
                float currentY = movementInputData.InputVector.y;
                movementInputData.InputVectorX = Mathf.Lerp(currentX, 0, 50 * Time.deltaTime);
                movementInputData.InputVectorY = Mathf.Lerp(currentY, 0, 50 * Time.deltaTime);
            }

            if (!fpsController.m_isGrounded || slide.isSliding || fpsController.m_inputVector.y == 0)
                isLookingAround = false;

            cameraController.isLookingAround = isLookingAround;

            JumpControl();
            CrouchControl();
            SprayControl();
            if (toggleZoom)
                ZoomToggle();
            else
                ZoomHold();
        }

        public void JumpControl()
        {
            if (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame || keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
                movementInputData.JumpClicked = true;
            else
                movementInputData.JumpClicked = false;
        }

        public void CrouchControl()
        {
            if (gamepad != null && gamepad.buttonEast.wasPressedThisFrame || keyboard != null && keyboard.leftCtrlKey.wasPressedThisFrame)
                movementInputData.CrouchClicked = true;
            else
                movementInputData.CrouchClicked = false;
        }

        public void SprayControl()
        {
            if (mouse != null && mouse.leftButton.wasPressedThisFrame && sprayAmount != 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(cameraController.transform.position, cameraController.transform.forward, out hit, sprayRange))
                {
                    audioSource.PlayOneShot(spraySFX);
                    sprayAmount--;
                    Instantiate(spray, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up) * Quaternion.Euler(0, 180, 0));
                }
            }
        }

        public void ZoomToggle()
        {
            if (gamepad != null && gamepad.rightStickButton.wasPressedThisFrame || keyboard != null && keyboard.zKey.wasPressedThisFrame)
                cameraInputData.ZoomClicked = true;
            else
                cameraInputData.ZoomClicked = false;
        }

        public void ZoomHold()
        {
            if (gamepad != null && gamepad.rightStickButton.wasPressedThisFrame || keyboard != null && keyboard.zKey.wasPressedThisFrame)
                cameraInputData.ZoomClicked = true;
            else
                cameraInputData.ZoomClicked = false;

            if (gamepad != null && gamepad.rightStickButton.wasReleasedThisFrame || keyboard != null && keyboard.zKey.wasReleasedThisFrame)
                cameraInputData.ZoomReleased = true;
            else
                cameraInputData.ZoomReleased = false;
        }

        public void RunToggle(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("RunToggle");
                movementInputData.IsRunning = !movementInputData.IsRunning;
                if (movementInputData.IsCrouching)
                    fpsController.InvokeCrouchRoutine();
            }
        }

        public void RunHold(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("RunHold");
                movementInputData.RunHeld = true;
                if (movementInputData.IsCrouching)
                    fpsController.InvokeCrouchRoutine();
            }
            else
                movementInputData.RunHeld = false;
        }

        public void MovementControl(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isMoving = true;
                Vector2 inputVector = context.ReadValue<Vector2>();
                movementInputData.InputVectorX = inputVector.x;
                movementInputData.InputVectorY = inputVector.y;
            }
            else
                isMoving = false;
        }

        public void CameraControl(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Vector2 inputVector = context.ReadValue<Vector2>();
                cameraInputData.InputVectorX = inputVector.x;
                cameraInputData.InputVectorY = inputVector.y;
            }
            else
            {
                cameraInputData.InputVectorX = 0;
                cameraInputData.InputVectorY = 0;
            }
        }

        public void UnlockCameraControl(InputAction.CallbackContext context)
        {
            if (context.performed)
                isLookingAround = true;
            else
                isLookingAround = false;
        }

        void InitialInput()
        {
            PlayerControls playerControls = new PlayerControls();
            playerControls.Player.Enable();

            playerControls.Player.RunToggle.started += RunToggle;
            playerControls.Player.RunHold.started += RunHold;
            playerControls.Player.Movement.performed += MovementControl;
            playerControls.Player.Camera.performed += CameraControl;
            playerControls.Player.UnlockCamera.performed += UnlockCameraControl;
        }
    }

}