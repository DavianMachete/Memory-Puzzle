using System.Collections.Generic;

using UnityEngine;

using MP.Core;
using MP.Levels.Procedural;

namespace MP.Levels
{
    public class LevelManager : Manager<LevelManager>
    {
        public ProceduralLevelSettings ProceduralLevelSettings => proceduralLevelSettings;
        [SerializeField] private ProceduralLevelSettings proceduralLevelSettings;

        [SerializeField] private LevelData[] preparedLevels;
        
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