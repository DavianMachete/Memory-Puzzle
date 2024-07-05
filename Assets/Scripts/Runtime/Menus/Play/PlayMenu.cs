using System.Collections.Generic;

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

        private readonly List<Card> _instantiatedCards = new();

        public void Initialize(LevelData levelData)
        {
            // Reset
            foreach (var instantiatedCard in _instantiatedCards)
            {
                instantiatedCard.DestroyCard();
            }
            _instantiatedCards.Clear();
            
            // Set cards
            gridLayout.constraintCount = levelData.ColumnsCount;
            
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
    }
}