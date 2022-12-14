using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Linq;

namespace FPSController
{
    public class GameManager : MonoBehaviour
    {
        public ScoreScriptableObject score;

        public static bool gameEnded;
        public bool IsPaused { get; private set; }
        public static event Action<bool> Pause;

        public GameObject gameOverUI;
        public GameObject backgroundUI;

        public float timeValue;
        [SerializeField] TextMeshProUGUI time_remaining;
        private HighscoreTable highscoreTable;
        private InputHandler inputHandler;
        private GameManager gameManager;
        private static string nameText;


        private void Awake()
        {
            inputHandler = GetComponentInChildren<InputHandler>();
            gameManager = GetComponentInChildren<GameManager>();
        }

        // Start is called before the first frame update
        void Start()
        {

            gameEnded = false;
        }

        // Update is called once per frame
        void Update()
        {
            updateTime();
        }

        void updateTime()
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
                EndGame();

            }
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            time_remaining.SetText(string.Format("{0:00}:{1:00}", minutes, seconds).ToString());
        }

        public void EndGame()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //int score;

            InputWindow.Show_Static("Player Name", "", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVXYWZ", 3, () =>
            {

            }, (nameText) =>
            {
                // OK
                Debug.Log(nameText);
                //highscoreTable.AddHighscoreEntry(InputHandler.Score, nameText);
                score.SaveHighScore(InputHandler.Score, nameText);
                
                gameOverUI.SetActive(true);
            });
            backgroundUI.SetActive(true);
            gameEnded = true;
            //gameOverUI.SetActive(true);
            
        }

        public void SetPause(bool paused)
        {
            Debug.Log("Paused:" + paused);
            Pause(paused);
        }
    }
}

