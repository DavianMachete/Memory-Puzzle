using System.Collections.Generic;
using MP.Cards;
using UnityEngine;

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

        
        public static LevelData GenerateLevel(int level)
        {
            var newLevelData = CreateInstance<LevelData>();
            newLevelData.Generate(level);
            return newLevelData;
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
        
        /// <summary>
        /// Generates a new level data model by level number
        /// </summary>
        /// <param name="levelNumber">is a whole number between 1 and max applied level number.</param>
        private void Generate(int levelNumber)
        {
            level = levelNumber;
            
            CalculateGridSize();
            ValidateGrid();
        }
        
        private void OnValidate()
        {
            ValidateGrid();
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

        private void GenerateGrid()
        {
            var area = RowsCount * ColumnsCount;
            var maxAvailableCardsCount = area / 2;
            
            // Get cards by difficulty
            var difficulty = level / area;
            var value = Mathf.Lerp(2, maxAvailableCardsCount, difficulty);
            var cardsCount = Mathf.RoundToInt(value);
            var lm = LevelManager.Instance;
            var settings = lm.ProceduralLevelSettings;
            var cards = settings.cards.GetFew(cardsCount);

            AssignCards(cards);
            Shuffle();
        }

        private void AssignCards(IReadOnlyList<CardData> cardsData)
        {
            var dataIndex = 0;
            var counter = 0;
            for (var row = 0; row < rowsCount; row++)
            {
                for (var col = 0; col < columnsCount; col++)
                {
                    counter++;
                    _cardsGrid[row, col] = cardsData[dataIndex].id;
                    if (counter != 2) 
                        continue;
                    
                    counter = 0;
                    dataIndex++;
                    if (dataIndex >= cardsData.Count)
                        dataIndex = 0;
                }
            }
        }

        /// <summary>
        /// Shuffles the grid using the Fisher-Yates algorithm
        /// </summary>
        private void Shuffle()
        {
            var random = new System.Random();
            for (var i = _cardsGrid.Length - 1; i > 0; i--)
            {
                var i0 = i / columnsCount;
                var i1 = i % columnsCount;

                var j = random.Next(i + 1);
                var j0 = j / columnsCount;
                var j1 = j % columnsCount;

                (_cardsGrid[i0, i1], _cardsGrid[j0, j1]) = (_cardsGrid[j0, j1], _cardsGrid[i0, i1]);
            }
        }
    }
}