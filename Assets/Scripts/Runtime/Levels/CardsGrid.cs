using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using MP.Cards;

using Random = System.Random;

namespace MP.Levels
{
    [Serializable]
    public class CardsGrid
    {
        public int RowsCount => rowsCount;
        [SerializeField] private int rowsCount;
        public int ColumnsCount => columnsCount;
        [SerializeField] private int columnsCount;
        [SerializeField] private List<string> values;
        
        private readonly Random _random;

        public CardsGrid(int rows, int columns)
        {
            rowsCount = rows;
            columnsCount = columns;
            values = new List<string>(new string[rows * columns]);
            _random = new Random();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public string Get(int row, int column)
        {
            var index = row * columnsCount + column;
            if (index < values.Count) 
                return values[index];
            
            Debug.LogError($"There is no value in grid by " +
                           $"[{row},{column}] coordinates to get.");
            return string.Empty;

        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Set(int row, int column, string value)
        {
            var index = row * columnsCount + column;
            if (index < values.Count)
            {
                values[index] = value;
                return;
            }

            Debug.LogError($"There is no coordinate in grid by" +
                           $" [{row},{column}] to set card id - {value}");
        }

        /// <summary>
        /// Calculates and return all card types count in the grid.
        /// </summary>
        /// <returns>All card types count</returns>
        public int GetAllCardTypesCount()
        {
            var cardTypeList = values.GroupBy( cardId => cardId );
            return cardTypeList.Count();
        }

        /// <summary>
        /// Updates the cards grid with new data -
        /// assigns cards data to grid and shuffles.
        /// </summary>
        /// <param name="cardsData">cards data to update grid</param>
        public void UpdateGrid(IReadOnlyList<CardData> cardsData)
        {
            var dataIndex = 0;
            var counter = 0;

            for (var index = 0; index < values.Count; index++)
            {
                counter++;
                values[index] = cardsData[dataIndex].id;
                
                if (counter != 2) 
                    continue;
                
                counter = 0;
                dataIndex++;
                if (dataIndex >= cardsData.Count)
                    dataIndex = 0;
            }

            Shuffle();
        }
        
        /// <summary>
        /// Shuffles the grid using the Fisher-Yates algorithm
        /// </summary>
        private void Shuffle()
        {   
            var n = values.Count;  
            while (n > 1) {  
                n--;  
                var k = _random.Next(n + 1);  
                (values[k], values[n]) = (values[n], values[k]);
            }
        }
    }
}