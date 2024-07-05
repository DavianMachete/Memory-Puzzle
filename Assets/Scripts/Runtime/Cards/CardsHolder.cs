using System.Collections.Generic;
using UnityEngine;

namespace MP.Cards
{
    [CreateAssetMenu(fileName = "Cards Holder", menuName = "Memory Puzzle/Cards Holder", order = 1)]
    public class CardsHolder : ScriptableObject
    {
        [SerializeField] private CardData[] cards;

        public bool TryGetCard(string id, out CardData data)
        {
            foreach (var card in cards)
            {
                if(card.id != id)
                    continue;

                data = card;
                return true;
            }
            
            data = default;
            return false;
        }

        /// <summary>
        /// Gets cards data from 0 index to count-1 and returns.
        /// </summary>
        /// <param name="count">The count of data to get</param>
        public List<CardData> GetFew(int count)
        {
            if (count > cards.Length)
            {
                Debug.LogWarning("Cards data length not enough.");
                count = cards.Length;
            }

            if (count < 1)
            {
                Debug.LogError("No cards data found.");
                return null;
            }

            var collection = new List<CardData>();
            for (var index = 0; index < count; index++)
            {
                var card = cards[index];
                collection.Add(card);
            }

            return collection;
        }
    }
}