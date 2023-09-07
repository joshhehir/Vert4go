using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VolumeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the GameObject that entered the trigger is the one you want to trigger your code.
        if (other.CompareTag("Player"))
        {
            // Code to initiate when the trigger is entered by the "Player" GameObject.
            Debug.Log("Trigger entered by Player!");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(0);
            // Add your code here.
        }
    }
}