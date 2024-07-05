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

        /// <summary>
        /// Gets level data by <see cref="CurrentLevel"/> value from prepared
        /// levels if exists, if so generates a new one.
        /// </summary>
        /// <returns>Prepared or new generated level data depending on <see cref="CurrentLevel"/></returns>
        public LevelData GetLevel()
        {
            // Get and return level data by current level
           return GetLevel(CurrentLevel);
        }

        /// <summary>
        /// Gets level data from prepared levels if exists, if so generates a new one.
        /// </summary>
        /// <param name="levelNumber">The value that describes level difficulty</param>
        /// <returns>Prepared or new generated level data depending on <b>"levelNumber"</b></returns>
        public LevelData GetLevel(int levelNumber)
        {
            // check and get Level from prepared
            foreach (var levelData in preparedLevels)
            {
                if (levelData.Level == levelNumber)
                    return levelData;
            }
            
            // if there is no prepared level data generate new one.
            var newLevelData = LevelData.GenerateLevel(levelNumber);
            return newLevelData;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        [ContextMenu("Add Level")]
        public void AddLevel()
        {
            _currentLevel += 1;
            Debug.Log($"Level added. Current level is {_currentLevel}");
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