﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace FPSController
{
    public class HighscoreTable : MonoBehaviour
    {
        public ScoreScriptableObject score;
        private Transform entryContainer;
        private Transform entryTemplate;
        private List<Transform> highscoreEntryTransformList;

        private void Awake()
        {
            //Uncomment to clear the leaderboard
            //PlayerPrefs.DeleteAll();
            
            entryContainer = transform.Find("highscoreEntryContainer");
            entryTemplate = entryContainer.Find("highscoreEntryTemplate");

            entryTemplate.gameObject.SetActive(false);

            //PlayerPrefs.DeleteKey("highscoreTable");
            string jsonString = PlayerPrefs.GetString("highscoreTable");
            Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

            if (highscores == null)
            {
                // There's no stored table, initialize
                Debug.Log("Initializing table with default values...");
                AddHighscoreEntry(1000, "JSH");
                // Reload
                jsonString = PlayerPrefs.GetString("highscoreTable");
                highscores = JsonUtility.FromJson<Highscores>(jsonString);
            }

            foreach (Highscore s in score.highscores)
            {
                AddHighscoreEntry(s.playerScore, s.playerName);
            }
            score.highscores.Clear();

            RefreshHighscoreTable();
        }

        private void RefreshHighscoreTable()
        {
            string jsonString = PlayerPrefs.GetString("highscoreTable");
            Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

            // Sort entry list by Score
            for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
            {
                for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
                {
                    if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                    {
                        // Swap
                        HighscoreEntry tmp = highscores.highscoreEntryList[i];
                        highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                        highscores.highscoreEntryList[j] = tmp;
                    }
                }
            }

            if (highscoreEntryTransformList != null)
            {
                foreach (Transform highscoreEntryTransform in highscoreEntryTransformList)
                {
                    Destroy(highscoreEntryTransform.gameObject);
                }
            }

            highscoreEntryTransformList = new List<Transform>();
            foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
            {
                CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
            }
        }

        private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
        {
            float templateHeight = 31f;
            Transform entryTransform = Instantiate(entryTemplate, container);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
            entryTransform.gameObject.SetActive(true);

            int rank = transformList.Count + 1;
            string rankString;
            switch (rank)
            {
                default:
                    rankString = rank + "TH"; break;

                case 1: rankString = "1ST"; break;
                case 2: rankString = "2ND"; break;
                case 3: rankString = "3RD"; break;
            }

            entryTransform.Find("posText").GetComponent<Text>().text = rankString;

            int score = highscoreEntry.score;

            entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

            string name = highscoreEntry.name;
            entryTransform.Find("nameText").GetComponent<Text>().text = name;

            // Set background visible odds and evens, easier to read
            entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

            // Highlight First
            if (rank == 1)
            {
                entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
                entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
                entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
            }

            transformList.Add(entryTransform);
        }

        public void AddHighscoreEntry(int score, string name)
        {
            // Create HighscoreEntry
            HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };

            // Load saved Highscores
            string jsonString = PlayerPrefs.GetString("highscoreTable");
            Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

            if (highscores == null)
            {
                // There's no stored table, initialize
                highscores = new Highscores()
                {
                    highscoreEntryList = new List<HighscoreEntry>()
                };
            }

            // Add new entry to Highscores
            highscores.highscoreEntryList.Add(highscoreEntry);

            // Save updated Highscores
            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString("highscoreTable", json);
            PlayerPrefs.Save();

            RefreshHighscoreTable();
        }

        private class Highscores
        {
            public List<HighscoreEntry> highscoreEntryList;
        }

        [System.Serializable]
        private class HighscoreEntry
        {
            public int score;
            public string name;
        }

    }
}