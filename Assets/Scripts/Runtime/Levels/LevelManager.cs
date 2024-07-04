using UnityEngine;
using MP.Core;

namespace MP.Levels
{
    public class LevelManager : Manager<LevelManager>
    {
        public int MaximumWidth => maximumWidth;
        [SerializeField] private int maximumWidth = 10;
        public int MaximumHeight => maximumHeight;
        [SerializeField] private int maximumHeight = 10;
        public int MinimumWidth => minimumWidth;
        [SerializeField] private int minimumWidth = 2;
        public int MinimumHeight => minimumHeight;
        [SerializeField] private int minimumHeight = 2;
        
        public int CurrentLevel => _currentLevel;
        private int _currentLevel = 1;

        private const string LevelKey = "Level";

        protected override void OnAwakeManager()
        {
            LoadLevel();
        }

        protected override bool HasInitialization()
        {
            return false;
        }

        public void AddLevel()
        {
            _currentLevel++;
            PlayerPrefs.SetInt(LevelKey, _currentLevel);
            PlayerPrefs.Save();
        }

        private void LoadLevel()
        {
            if (!PlayerPrefs.HasKey(LevelKey))
                return;

            _currentLevel = PlayerPrefs.GetInt(LevelKey);
        }
    }
}