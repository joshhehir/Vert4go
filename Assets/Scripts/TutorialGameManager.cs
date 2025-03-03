using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.Linq;

namespace FPSController
{
    public class TutorialGameManager : MonoBehaviour
    {
        public static bool gameEnded;
        public bool IsPaused { get; private set; }
        public static event Action<bool> Pause;

        public GameObject gameOverUI;
        public GameObject backgroundUI;

        private InputHandler inputHandler;

        private void Awake()
        {
            inputHandler = GetComponentInChildren<InputHandler>();
        }

        // Start is called before the first frame update
        void Start()
        {
            gameEnded = false;
        }

        public void EndGame()
        {
            if (gameEnded) return; // Prevents multiple calls

            gameEnded = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            gameOverUI.SetActive(true);
            backgroundUI.SetActive(true);
        }

        public void SetPause(bool paused)
        {
            Debug.Log("Paused:" + paused);
            Pause(paused);
        }
    }
}

