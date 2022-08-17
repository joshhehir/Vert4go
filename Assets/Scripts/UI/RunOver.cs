using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace FPSController
{
    public class RunOver : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI scoreText;

        public void Setup(int score)
        {
            gameObject.SetActive(true);
            scoreText.SetText(score.ToString());
        }

        public void RestartButton()
        {
            SceneManager.LoadScene("Josh-Test");
        }

        public void QuitButton()
        {
            Application.Quit();
            Debug.Log("Quitting");
        }
    }
}

