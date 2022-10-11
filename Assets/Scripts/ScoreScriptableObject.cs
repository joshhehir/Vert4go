using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPSController
{
    
    [System.Serializable]
    public class Highscore 
    {
        public string playerName;
        public int playerScore;

        public Highscore(int score, string name) {
            playerName = name;
            playerScore = score;
        }
    }
    [CreateAssetMenu]
    public class ScoreScriptableObject : ScriptableObject
    {
        public List<Highscore> highscores = new List<Highscore>();

        public void SaveHighScore(int score, string name)
        {
            if (highscores.Where(x => x.playerScore == score).Where(x => x.playerName == name).Any()) return;
            highscores.Add(new Highscore(score, name));
        }
    }
}

