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

        #region Methods -> Manager overrides
        
        protected override void OnAwakeManager()
        {
            LoadLevel();
        }

        protected override bool HasInitialization()
        {
            return false;
        }
        
        #endregion

        #region Methods -> Public

        public LevelData GetLevel()
        {
            // check and get Level from prepared
            foreach (var levelData in preparedLevels)
            {
                if (levelData.Level == CurrentLevel)
                    return levelData;
            }
            
            // if there is no prepared level data generate new one.
            var newLevelData = LevelData.GenerateLevel(CurrentLevel);
            return newLevelData;
        }
        
        public void AddLevel()
        {
            _currentLevel++;
            PlayerPrefs.SetInt(LevelKey, _currentLevel);
            PlayerPrefs.Save();
        }
        
        #endregion

        private void LoadLevel()
        {
            if (!PlayerPrefs.HasKey(LevelKey))
                return;

            _currentLevel = PlayerPrefs.GetInt(LevelKey);
        }
    }
}