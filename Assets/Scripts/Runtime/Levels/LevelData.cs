using UnityEngine;

namespace MP.Levels
{
    [CreateAssetMenu(fileName = "Level", menuName = "Memory Puzzle/New Level", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int CardsTypeCount => cardsGrid.GetAllCardTypesCount();
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
        [SerializeField] private CardsGrid cardsGrid;
        
        
        #region Methods -> Unity callbacks
        
        private void OnValidate()
        {
            ValidateGrid();
        }

        #endregion

        #region Methods -> Public
        
        public static LevelData GenerateLevel(int level)
        {
            var newLevelData = CreateInstance<LevelData>();
            newLevelData.Generate(level);
            return newLevelData;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void ValidateGrid()
        {
            if (cardsGrid != null && 
                cardsGrid.RowsCount == rowsCount &&
                cardsGrid.ColumnsCount == columnsCount) 
                return;
            
            Debug.Log($"Validating the cards grid with rows: {rowsCount}, columns: {columnsCount}");
            cardsGrid = new CardsGrid(rowsCount, columnsCount);
        }

        public string GetCardId(int row, int column)
        {
            return cardsGrid.Get(row, column);
        }
        
        public void SetCardId(string value, int row, int col)
        {
            cardsGrid.Set(row, col, value);
        }

        #endregion
        

        #region Methods -> Private
        
        /// <summary>
        /// Generates a new level data model by level number
        /// </summary>
        /// <param name="levelNumber">is a whole number between 1 and max applied level number.</param>
        private void Generate(int levelNumber)
        {
            level = levelNumber;
            
            CalculateGridSize();
            ValidateGrid();
            GenerateGrid();
        }

        private void CalculateGridSize()
        {
            var lm = LevelManager.Instance;
            var settings = lm.ProceduralLevelSettings;
            var gridSettings = settings.gridSettings;
            
            var minColumns = gridSettings.minimumColumns;
            var maxColumns = gridSettings.maximumColumns;
            var minRows = gridSettings.minimumRows;
            var maxRows = gridSettings.maximumRows;

            var maxLevel = settings.maximumLevel;
            var difficulty = level / (float)maxLevel;
            
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
                columnsCount -= 1;
            }
            else
            {
                rowsCount -= 1;
            }
        }

        private void GenerateGrid()
        {
            var area = RowsCount * ColumnsCount;
            var maxAvailableCardsCount = area / 2f;
            
            // Get cards by difficulty
            var lm = LevelManager.Instance;
            var settings = lm.ProceduralLevelSettings;
            var maxLevel = settings.maximumLevel;
            var difficulty = level / (float)maxLevel;
            var value = Mathf.Lerp(2f, maxAvailableCardsCount, difficulty);
            var cardsCount = Mathf.RoundToInt(value);
            var cards = settings.cards.GetFew(cardsCount);
            
            // Update cards grid
            cardsGrid.UpdateGrid(cards);
        }
        
        #endregion
    }
}