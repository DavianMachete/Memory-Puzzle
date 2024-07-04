using System;
using UnityEngine;
using MP.Tools;

namespace MP.Levels
{
    [CreateAssetMenu(fileName = "Level", menuName = "Memory Puzzle/New Level", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int Level => level;
        [SerializeField] private int level;
        public int ColumnsCount => columnsCount;
        [Tooltip("Level grid columns count")]
        [SerializeField] private int columnsCount;
        public int RowsCount => rowsCount;
        [Tooltip("Level grid rows count")]
        [SerializeField] private int rowsCount;

        [Tooltip("Level cards grid id by coordinates, where the index will be " +
                 "the row index and the wrapper value index will be the column index.")]
        [SerializeField] private string[,] _cardsGrid;

        /// <summary>
        /// Generates a new level data model by level number
        /// </summary>
        /// <param name="level">is a whole number between 1 and max applied level number.</param>
        public LevelData(int level)
        {
            this.level = level;
            
            CalculateGridSize();
            ValidateGrid();
        }

        private void OnValidate()
        {
            ValidateGrid();
        }

        public string GetCardId(int rowIndex, int columnIndex)
        {
            if (rowIndex >= _cardsGrid.GetLength(0) ||
                columnIndex >= _cardsGrid.GetLength(1))
                return string.Empty;
            
            return _cardsGrid[rowIndex, columnIndex];
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void SetCardId(string value, int rowIndex, int columnIndex)
        {
            if (rowIndex >= _cardsGrid.GetLength(0) ||
                columnIndex >= _cardsGrid.GetLength(1))
                Debug.LogError("Card coordinates are out of bounds.");

            _cardsGrid[rowIndex, columnIndex] = value;
        }

        public void ValidateGrid()
        {
            if (_cardsGrid == null || 
                _cardsGrid.GetLength(0) != rowsCount || 
                _cardsGrid.GetLength(1) != columnsCount)
            {
                _cardsGrid = new string[rowsCount, columnsCount];
            }
        }

        private void CalculateGridSize()
        {
            var lm = LevelManager.Instance;
            var settings = lm.ProceduralLevelSettings;
            var gridSettings = settings.gridSettings;
            
            var maxColumns = gridSettings.maximumColumns;
            var minColumns = gridSettings.minimumColumns;
            var maxRows = gridSettings.maximumRows;
            var minRows = gridSettings.minimumRows;

            var maxLevel = settings.maximumLevel;
            var difficulty = level / maxLevel;
            
            var columnsValue = Mathf.Lerp(minColumns, maxColumns, difficulty);
            var rowsValue = Mathf.Lerp(minRows, maxRows, difficulty);
            
            columnsCount = Mathf.RoundToInt(columnsValue);
            rowsCount = Mathf.RoundToInt(rowsValue);
            
            // Check if the area is multiplier of 2.
            var area = columnsCount * rowsCount;
            if (area % 2 == 0)
                return;
            
            // If area is not a multiplier of 2 we need to reproduce
            if (columnsCount > rowsCount)
            {
                columnsCount--;
            }
            else
            {
                rowsCount--;
            }
        }
    }
}