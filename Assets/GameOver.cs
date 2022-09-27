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

        void OnEnable()
        {

            textScore.text = "Score: " + InputHandler.Score.ToString();
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
            //SceneManager.LoadScene("MainMenu");
            Debug.Log("Main Menu");
        }

    }
}
  
