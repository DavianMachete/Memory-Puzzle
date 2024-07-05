using MP.Core;
using MP.Menus;
using MP.Menus.Play;
using MP.Menus.Start;

namespace MP.Game
{
    public class GameManager : Manager<GameManager>
    {
        #region Methods -> Manager overrides
        
        protected override void OnAwakeManager()
        {
            var mm = MenuManager.Instance;
            mm.ActivateMenu<StartMenu>();
        }

        protected override bool HasInitialization() => false;

        #endregion

        #region Methods -> Public

        public void StartTheLevel()
        {
            
            
            var mm = MenuManager.Instance;
            mm.ActivateMenu<PlayMenu>();
        }

        #endregion
    }
}