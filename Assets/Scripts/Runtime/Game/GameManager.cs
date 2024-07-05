using System.Collections;
using MP.Core;
using MP.Levels;
using MP.Menus;
using MP.Menus.Play;
using MP.Menus.Start;
using UnityEngine;

namespace MP.Game
{
    public class GameManager : Manager<GameManager>
    {
        [SerializeField, Range(0f, 5f)] private float secondsToCompareCards = 1f;
        
        /// <summary>
        /// Cards to compare
        /// </summary>
        private Card _card1, _card2;
        private PlayMenu _playMenu;
        
        #region Methods -> Manager overrides

        protected override bool HasInitialization() => false;

        #endregion

        #region Methods -> Public

        public void StartTheLevel()
        {
            var lm = LevelManager.Instance;
            var levelData = lm.GetLevel();
            
            var mm = MenuManager.Instance;
            _playMenu = mm.ActivateMenu<PlayMenu>();
            _playMenu.Initialize(levelData);
        }

        public void AddCardToCompare(Card card)
        {
            // assign as the first selected card if there is no
            if (_card1 == null)
            {
                _card1 = card;
                return;
            }

            // assign as the second selected card
            _card2 = card;

            // update play menu
            _playMenu.SetBlock(true);
            _playMenu.AddTurns();

            StartCoroutine(Compare());
        }

        private IEnumerator Compare()
        {
            yield return new WaitUntil(() => _card1.IsOpened && _card2.IsOpened);
            yield return new WaitForSeconds(secondsToCompareCards);
            
            // start cards interactions
            _playMenu.SetBlock(false);
            
            // compare
            if (_card1.Id != _card2.Id)
            {
                // close and continue
                _card1.Close();
                _card2.Close();

                _card1 = null;
                _card2 = null;
                yield break;
            }
            
            // Add matches and hide cards from grid
            _playMenu.AddMatches();
            
            _card1.Hide();
            _card2.Hide();

            _card1 = null;
            _card2 = null;
            
            // check for win
            var allCardsMatched = _playMenu.AreAllCardsHidden();
            if (!allCardsMatched)
                yield break;

            var lm = LevelManager.Instance;
            lm.AddLevel();
            var mm = MenuManager.Instance;
            mm.ActivateMenu<NextMenu>();
        }

        #endregion
    }
}