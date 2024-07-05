using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using MP.Game;
using MP.Levels;

namespace MP.Menus.Start
{
    public class NextMenu : Menu
    { 
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text currentLevel;

        private const string CurrentLevelFormat = "Current Level {0}";

        protected override void OnMenuStart()
        {
            base.OnMenuStart();
            
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextLevel);
        }

        protected override void AnimateShow(Action onAnimated)
        {
            base.AnimateShow(onAnimated);

            var lm = LevelManager.Instance;
            var level = lm.CurrentLevel;
            currentLevel.text = string.Format(CurrentLevelFormat, level);
        }

        private void NextLevel()
        {
            var gm = GameManager.Instance;
            gm.StartTheLevel();
        }
    }
}