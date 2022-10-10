using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace FPSController
{
    public class GameOver : MonoBehaviour
    {
        public TextMeshProUGUI textScore;
        private InputHandler inputHandler;

        private void Awake()
        {
            inputHandler = GetComponentInChildren<InputHandler>();
        }

        void OnEnable()
        {
            textScore.text = "Score: " + inputHandler.Score.ToString();
        }
        public void RestartButton()
        {
            SceneManager.LoadScene("City");
        }

        public void QuitButton()
        {
            Application.Quit();
            Debug.Log("Quitting");
        }

        public void MainMenuButton()
        {
            SceneManager.LoadScene("MainMenu");
            Debug.Log("Main Menu");
        }

    }
}
  
