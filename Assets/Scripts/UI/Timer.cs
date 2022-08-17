using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FPSController
{
    public class Timer : MonoBehaviour
    {
        public float timeValue;
        [SerializeField] TextMeshProUGUI time_remaining;

        // Update is called once per frame
        void Update()
        {
            if (timeValue > 0)
            {
                timeValue -= Time.deltaTime;
            }
            else
            {
                timeValue = 0;
            }
            DisplayTime(timeValue);
        }

        void DisplayTime(float timeToDisplay)
        {
            if (timeToDisplay < 0)
            {
                timeToDisplay = 0;
                SceneManager.LoadScene("End-Scren");

            }
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            time_remaining.SetText(string.Format("{0:00}:{1:00}", minutes, seconds).ToString());
        }

    }
}

