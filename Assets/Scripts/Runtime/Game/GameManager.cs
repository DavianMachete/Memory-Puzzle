using System.Collections;
using MP.Core;
using MP.Levels;
using MP.Menus;
using MP.Menus.Play;
using MP.Menus.Start;

namespace MP.Game
{
    public class GameManager : Manager<GameManager>
    {
        #region Methods -> Manager overrides

        protected override bool HasInitialization() => false;

        #endregion

        #region Methods -> Public

        public void StartTheLevel()
        {
            var lm = LevelManager.Instance;
            var levelData = lm.GetLevel();
            
            var mm = MenuManager.Instance;
            var pm = mm.ActivateMenu<PlayMenu>();
            pm.Initialize(levelData);
        }

        #endregion
    }
}