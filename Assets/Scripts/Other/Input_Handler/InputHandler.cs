using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Collections;
using TMPro;

namespace FPSController
{
    public class InputHandler : MonoBehaviour
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
        public GameObject topSprayMat;
        public GameObject midSprayMat;
        public GameObject botSprayMat;
        public GameObject specSprayMat;

        [Space, Header("Input Data")]
        [SerializeField] private CameraInputData cameraInputData = null;
        [SerializeField] private MovementInputData movementInputData = null;

        [Space, Header("UI")]
        public TextMeshProUGUI text_sprays_remaining;
        public TextMeshProUGUI textScore;

        public static int Score;


        void Awake()
        {
            fpsController = GetComponent<FirstPersonController>();
            cameraController = GetComponentInChildren<CameraController>();
            leaning = GetComponent<Leaning>();
            dodge = GetComponent<Dodge>();
            slide = GetComponent<Slide>();
            movementInputData.ResetInput();

            InitialInput();
        }

        void Start()
        {
            Score = 0;
            cameraInputData.ResetInput();
            //movementInputData.ResetInput();
            audioSource = GetComponent<AudioSource>();
        }

        void Update()
        {
            gamepad = Gamepad.current;
            keyboard = Keyboard.current;
            mouse = Mouse.current;

            //If player is moving slow, forward is not being pressed, or if they hit a wall, automatically stops running.
            if (fpsController.m_currentSpeed <= 0.5f || fpsController.m_inputVector.y < .5f || fpsController.m_hitWall)
            {
                movementInputData.IsRunning = false;
                movementInputData.RunReleased = true;
            }

            //Can't crouch while running.
            if (movementInputData.IsRunning)
                movementInputData.IsCrouching = false;

            //Running state is controlled by either RunClicked or RunHeld, depending on toggleRun check.
            if (!toggleRun)
                movementInputData.IsRunning = movementInputData.RunHeld;

            //Lerps player input back to zero to create a more natural stop.
            if (!isMoving)
            {
                float currentX = movementInputData.InputVector.x;
                float currentY = movementInputData.InputVector.y;
                movementInputData.InputVectorX = Mathf.Lerp(currentX, 0, 50 * Time.deltaTime);
                movementInputData.InputVectorY = Mathf.Lerp(currentY, 0, 50 * Time.deltaTime); ;
            }

            //Can only look around if Y has input and player is not airborne or sliding.
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

            if (GameManager.gameEnded)
            {
                this.enabled = false;
                return;
            }
        }

        //Unfortunately, crouch, jump and zoom bindings still need to be altered in script instead of through the new input system
        //because context.started != GetButtonDown and they rely on specific frame timing.
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

        private void SprayControl()
{
            if (mouse != null && mouse.leftButton.wasPressedThisFrame && sprayAmount != 0)
        {
                 //Debug.Log("Spray was clicked");
                 RaycastHit hit;
                 if (Physics.Raycast(cameraController.transform.position, cameraController.transform.forward, out hit, sprayRange))
                 {
                      DecalProjector dP;
                        audioSource.PlayOneShot(spraySFX);
                        sprayAmount--;
                        float hitHeight = hit.point.y;
                        int hitHeightInt = Mathf.RoundToInt(hitHeight);
                        switch (hit.collider.gameObject.tag)
                        {
                            case "Top":
                                Score += hitHeightInt *= 40;
                                spray = topSprayMat;
                                break;

                            case "Middle":
                                Score += hitHeightInt *= 20;
                                spray = midSprayMat;
                                break;

                            case "Lower":
                                Score += 10;
                                Score += hitHeightInt *= 10;
                                spray = botSprayMat;
                                break;

                            case "Special":
                                Score += hitHeightInt *= 100;
                                spray = specSprayMat;
                                break;
                        }
                        
                        // Calculate the rotation to align the decal projector with the wall surface
                        Quaternion rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                        
                        // Flip the rotation horizontally by applying a rotation around the Y-axis
                        rotation *= Quaternion.Euler(0, 180, 0);

                        // Spawn the decal projector with the calculated rotation
                        GameObject newSpray = Instantiate(spray, hit.point, rotation);
                        
                        dP = newSpray.GetComponent<DecalProjector>();
                        Debug.Log(dP);
                    }
                }
                text_sprays_remaining.SetText("Sprays: " + sprayAmount.ToString());
                textScore.SetText("Score: " + Score.ToString());
                Debug.DrawRay(cameraController.transform.position, cameraController.transform.forward * sprayRange, Color.green);
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

        void DodgeInput(InputAction.CallbackContext context)
        {
            if (context.performed)
                dodge.DoDodge();
        }

        void LeftLeanInput(InputAction.CallbackContext context)
        {
            if (context.performed)
                leaning.LeftLeanInput();
            else
                leaning.isLeaningLeft = false;
        }
        void RightLeanInput(InputAction.CallbackContext context)
        {
            if (context.performed)
                leaning.RightLeanInput();
            else
                leaning.isLeaningRight = false;
        }
        void UpLeanInput(InputAction.CallbackContext context)
        {
            if (context.performed)
                leaning.UpLeanInput();
            else
                leaning.isLeaningUp = false;
        }

        void InitialInput()
        {
            PlayerControls playerControls = new PlayerControls();
            playerControls.Player.Enable();

            playerControls.Player.RunToggle.started += RunToggle;
            playerControls.Player.RunToggle.performed += RunToggle;
            playerControls.Player.RunToggle.canceled += RunToggle;

            playerControls.Player.RunHold.started += RunHold;
            playerControls.Player.RunHold.performed += RunHold;
            playerControls.Player.RunHold.canceled += RunHold;

            playerControls.Player.Movement.started += MovementControl;
            playerControls.Player.Movement.performed += MovementControl;
            playerControls.Player.Movement.canceled += MovementControl;

            playerControls.Player.Camera.started += CameraControl;
            playerControls.Player.Camera.performed += CameraControl;
            playerControls.Player.Camera.canceled += CameraControl;

            playerControls.Player.UnlockCamera.started += UnlockCameraControl;
            playerControls.Player.UnlockCamera.performed += UnlockCameraControl;
            playerControls.Player.UnlockCamera.canceled += UnlockCameraControl;

            playerControls.Player.LeanLeft.started += LeftLeanInput;
            playerControls.Player.LeanLeft.performed += LeftLeanInput;
            playerControls.Player.LeanLeft.canceled += LeftLeanInput;

            playerControls.Player.LeanRight.started += RightLeanInput;
            playerControls.Player.LeanRight.performed += RightLeanInput;
            playerControls.Player.LeanRight.canceled += RightLeanInput;

            playerControls.Player.LeanUp.started += UpLeanInput;
            playerControls.Player.LeanUp.performed += UpLeanInput;
            playerControls.Player.LeanUp.canceled += UpLeanInput;

            playerControls.Player.Dodge.performed += DodgeInput;
        }
    }
}
