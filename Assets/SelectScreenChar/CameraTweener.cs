using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/*
public class CameraTweener : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] float duration;
    // Start is called before the first frame update
    void Start()
    {
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Move()
    {
       //LeanTween.moveX(gameObject,targetTransform.position.x, duration);
        LeanTween.moveLocal(gameObject, targetTransform.position, duration);
    }
}
*/

public class TargetSwitcher : MonoBehaviour
{
    public List<GameObject> targets; // List of target GameObjects to switch between
    public List<GameObject> targetUIElements; // List of UI elements unique to each target

    private int currentIndex = 0; // Start with the first target
    private bool isInitialMoveComplete = false; // Flag to track if the initial move has finished
    private bool initialTargetLocked = false; // Flag to prevent access to the initial target after moving away
    public int currentScene;
    public GameObject[] characters;
    public int selectedCharacter = 0;

    // Input Actions for switching targets
    public InputAction switchLeft;
    public InputAction switchRight;
    public InputAction selectChar;

    public float moveDuration = 2f; // Duration for the camera to move to the target's position
    public float rotateDuration = 2f; // Duration for the camera to rotate to the target's rotation

    void OnEnable()
    {
        // Enable input actions
        switchLeft.Enable();
        switchRight.Enable();
        selectChar.Enable();

        // Subscribe to the performed event of each action
        switchLeft.performed += _ => MoveLeft();
        switchRight.performed += _ => MoveRight();
        selectChar.performed += _ => StartLevel();
    }

    void OnDisable()
    {
        // Disable input actions and unsubscribe
        switchLeft.Disable();
        switchRight.Disable();
        selectChar.Disable();
    }

    void Start()
    {
        if (targets.Count > 0)
        {
            // Ensure all UI elements are hidden at the beginning
            foreach (GameObject uiElement in targetUIElements)
            {
                if (uiElement != null)
                {
                    uiElement.SetActive(false);
                }
            }

            StartInitialMove(); // Begin the initial move
        }
    }

    // Method to move to the previous target
    private void MoveLeft()
    {
        if (isInitialMoveComplete)
        {
            do
            {
                currentIndex = (currentIndex - 1 + targets.Count) % targets.Count;
            } while (initialTargetLocked && currentIndex == 0); // Skip initial target if locked

            // Switch characters
            SwitchCharacter(-1);
            UpdateTarget();
        }
    }

    // Method to move to the next target
    private void MoveRight()
    {
        if (isInitialMoveComplete)
        {
            do
            {
                currentIndex = (currentIndex + 1) % targets.Count;
            } while (initialTargetLocked && currentIndex == 0); // Skip initial target if locked

            // Switch characters
            SwitchCharacter(1);
            UpdateTarget();
        }
    }

    private void StartLevel()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
    }

    void StartInitialMove()
    {
        Debug.Log("Playing initial move...");
        Invoke("CompleteInitialMove", 0f); // Simulate the initial move
    }

    void CompleteInitialMove()
    {
        Debug.Log("Initial move complete. Target switching enabled.");
        isInitialMoveComplete = true; // Enable target switching
        UpdateTarget(); // Show the initial target
    }

    private void UpdateTarget()
    {
        if (targets.Count > 0 && currentIndex < targets.Count)
        {
            // Hide all UI elements before showing the active target's UI
            foreach (GameObject uiElement in targetUIElements)
            {
                if (uiElement != null)
                {
                    uiElement.SetActive(false);
                }
            }

            // Show the UI element for the current target
            if (targetUIElements[currentIndex] != null)
            {
                targetUIElements[currentIndex].SetActive(true);
            }

            GameObject currentTarget = targets[currentIndex];
            Debug.Log("Current Target: " + currentTarget.name);

            // Move to the target's position
            LeanTween.move(gameObject, currentTarget.transform.position, moveDuration).setEase(LeanTweenType.easeInOutQuad);

            // Rotate to match the target's rotation
            LeanTween.rotate(gameObject, currentTarget.transform.rotation.eulerAngles, rotateDuration).setEase(LeanTweenType.easeInOutQuad);

            // Lock the initial target after moving to a new target
            if (currentIndex != 0)
            {
                initialTargetLocked = true;
            }
        }
    }

    private void SwitchCharacter(int direction)
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + direction + characters.Length) % characters.Length;
        characters[selectedCharacter].SetActive(true);
    }
}


/*
public class TargetSwitcher : MonoBehaviour
{
    public List<GameObject> targets; // List of target GameObjects to switch between
    public List<GameObject> targetUIElements; // List of UI elements unique to each target

    private int currentIndex = 0; // Start with the first target
    private bool isInitialMoveComplete = false; // Flag to track if the initial move has finished
    private bool initialTargetLocked = false; // Flag to prevent access to the initial target after moving away
    public int currentScene;
    public GameObject[] characters;
    public int selectedCharacter = 0;

    // Input Actions for switching targets
    public InputAction switchLeft;
    public InputAction switchRight;
    public InputAction selectChar;

    public float moveDuration = 2f; // Duration for the camera to move to the target's position
    public float rotateDuration = 2f; // Duration for the camera to rotate to the target's rotation

    void OnEnable()
    {
        // Enable input actions
        switchLeft.Enable();
        switchRight.Enable();
        selectChar.Enable();

        // Subscribe to the performed event of each action
        switchLeft.performed += _ => MoveLeft();
        switchRight.performed += _ => MoveRight();
        selectChar.performed += _ => StartLevel();
    }

    void OnDisable()
    {
        // Disable input actions and unsubscribe
        switchLeft.Disable();
        switchRight.Disable();
    }

    void Start()
    {
        if (targets.Count > 0)
        {
            // Ensure all UI elements are hidden at the beginning
            foreach (GameObject uiElement in targetUIElements)
            {
                if (uiElement != null)
                {
                    uiElement.SetActive(false);
                }
            }

            if (targets.Count > 0)
            {
                StartInitialMove(); // Begin the initial move
            }
            StartInitialMove(); // Begin the initial move
        }
    }

    // Method to move to the previous target
    private void MoveLeft()
    {
        if (isInitialMoveComplete)
        {
            do
            {
                currentIndex = (currentIndex - 1 + targets.Count) % targets.Count;
            } while (initialTargetLocked && currentIndex == 0); // Skip initial target if locked

            characters[selectedCharacter].SetActive(false);
            selectedCharacter--;
            if (selectedCharacter < 0)
            {
                selectedCharacter += characters.Length;
            }
            characters[selectedCharacter].SetActive(true);

            UpdateTarget();
        }
    }

    // Method to move to the next target
    private void MoveRight()
    {
        if (isInitialMoveComplete)
        {
            do
            {
                currentIndex = (currentIndex + 1) % targets.Count;
            } while (initialTargetLocked && currentIndex == 0); // Skip initial target if locked

            characters[selectedCharacter].SetActive(false);
            selectedCharacter = (selectedCharacter + 1) % characters.Length;
            characters[selectedCharacter].SetActive(true);

            UpdateTarget();
        }
    }

    private void StartLevel()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
    }

    // Function to simulate the initial move, replace with your desired initial action
    void StartInitialMove()
    {
        Debug.Log("Playing initial move...");
        Invoke("CompleteInitialMove", 0f); // Replace 2f with the time it takes for your initial move
    }

    // Call this when the initial move is done
    void CompleteInitialMove()
    {
        Debug.Log("Initial move complete. Target switching enabled.");
        isInitialMoveComplete = true; // Enable target switching
        UpdateTarget(); // Show the initial target
    }

    private void UpdateTarget()
    {
        if (targets.Count > 0 && currentIndex < targets.Count)
        {
            // Hide all UI elements before showing the active target's UI
            foreach (GameObject uiElement in targetUIElements)
            {
                if (uiElement != null)
                {
                    uiElement.SetActive(false);
                }
            }

            // Show the UI element for the current target
            if (targetUIElements[currentIndex] != null)
            {
                targetUIElements[currentIndex].SetActive(true);
            }

            GameObject currentTarget = targets[currentIndex];
            Debug.Log("Current Target: " + currentTarget.name);

            // Move to the target's position
            LeanTween.move(gameObject, currentTarget.transform.position, moveDuration).setEase(LeanTweenType.easeInOutQuad);

            // Rotate to match the target's rotation
            LeanTween.rotate(gameObject, currentTarget.transform.rotation.eulerAngles, rotateDuration).setEase(LeanTweenType.easeInOutQuad);

            // Lock the initial target after moving to a new target
            if (currentIndex != 0)
            {
                initialTargetLocked = true;
            }
        }
    }
}

    */
