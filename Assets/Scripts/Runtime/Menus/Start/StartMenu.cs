using System;
using MP.Game;
using MP.Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MP.Menus.Start
{
    public class StartMenu : Menu
    {
        [SerializeField] private Button startButton;
        [SerializeField] private TMP_Text currentLevel;

        private const string CurrentLevelFormat = "Current Level {0}";

        protected override void OnMenuStart()
        {
            base.OnMenuStart();
            
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGame);

            var lm = LevelManager.Instance;
            var level = lm.CurrentLevel;
            currentLevel.text = string.Format(CurrentLevelFormat, level);
        }

        private void StartGame()
        {
            var gm = GameManager.Instance;
            gm.StartTheLevel();
        }
    }
}