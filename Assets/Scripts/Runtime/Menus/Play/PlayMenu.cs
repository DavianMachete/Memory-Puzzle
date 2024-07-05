using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using MP.Cards;
using MP.Levels;

namespace MP.Menus.Play
{
    public class PlayMenu : Menu
    {
        [SerializeField] private CardsHolder cardsHolder;
        [SerializeField] private Card cardPrefab;
        [SerializeField] private RectTransform board;
        [SerializeField] private GridLayoutGroup gridLayout;
        [Tooltip("The ratio of cell width divides to cell height.")]
        [SerializeField, Range(0.01f, 2f)] private float cellHeightToWeightRatio;

        private readonly List<Card> _instantiatedCards = new();

        public void Initialize(LevelData levelData)
        {
            ResetMenu();
            InitializeGrid(levelData);
        }

        public void SetBlock(bool value)
        {
            foreach (var instantiatedCard in _instantiatedCards)
            {
                instantiatedCard.SetBlock(value);
            }
        }

        public bool AreAllCardsHidden()
        {
            return _instantiatedCards.All(card => card.IsHidden);
        }

        #region Methods -> Private

        private void ResetMenu()
        {
            foreach (var instantiatedCard in _instantiatedCards)
            {
                instantiatedCard.DestroyCard();
            }
            _instantiatedCards.Clear();
            
        }

        private void InitializeGrid(LevelData levelData)
        {
            UpdateGrid(levelData);
            InstantiateCards(levelData);
        }

        private void UpdateGrid(LevelData levelData)
        {
            gridLayout.constraintCount = levelData.ColumnsCount;

            var rowsCount = levelData.RowsCount;
            var height = board.rect.height;
            var verticalSpacing = gridLayout.spacing.y;
            var topPadding = gridLayout.padding.top;
            var bottomPadding = gridLayout.padding.bottom;
            var spacingSum = topPadding + bottomPadding + (rowsCount - 1) * verticalSpacing;
            
            var cellHeight = height / rowsCount - spacingSum;
            var cellWidth = cellHeight * cellHeightToWeightRatio;
            
            gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        }

        private void InstantiateCards(LevelData levelData)
        {
            for (var col = 0; col < levelData.ColumnsCount; col++)
            {
                for (var row = 0; row < levelData.RowsCount; row++)
                {
                    var cardId = levelData.GetCardId(row, col);
                    if (!cardsHolder.TryGetCard(cardId, out var cardData))
                        continue;

                    var card = Instantiate(cardPrefab, board);
                    card.Initialize(cardData);
                    _instantiatedCards.Add(card);
                }
            }
        }

        #endregion
    }
}