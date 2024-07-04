using UnityEngine;

namespace MP.Menus
{
    public abstract class Menu : MenuElement
    {
        // Misc
        public MenuType MenuType => menuType;
        [SerializeField] private MenuType menuType = MenuType.Static;

        #region Methods -> Menu Element Callbacks

        protected override void OnMenuStart()
        {
            MenuManager.Instance.AddMenu(this);
        }
        
        protected override void OnMenuDestroy()
        {
            MenuManager.Instance.RemoveMenu(this);
        }

        #endregion
    }
}