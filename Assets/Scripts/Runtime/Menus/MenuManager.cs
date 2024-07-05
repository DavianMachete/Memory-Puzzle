using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using MP.Core;

namespace MP.Menus
{
    public class MenuManager : Manager<MenuManager>
    {
		// Misc
		private readonly Dictionary<Type, Menu> _menuInstances = new ();

		// Menu stacking
		private readonly List<Menu> _menuStack = new ();
		public List<Menu> MenuStack => _menuStack;
		public Menu ActiveMenu => _menuStack.Count > 0 ? _menuStack[^1] : null;

		// Events
		public event Action<Menu> OnManuActivated, OnManuDeactivated, OnMenuAdded, OnMenuRemoved;

		protected override void OnAwakeManager()
        {
			_menuInstances.Clear();
		}

		#region Menu Activation

		public T ActivateMenu<T>() where T : Menu
		{
			return ActivateMenu(typeof(T)) as T;
		}

		// ReSharper disable Unity.PerformanceAnalysis
		public Menu ActivateMenu(Type type)
		{
			// Activate the menu by its pre-existing instance (if any)
			if (TryGetMenu(type, out var menuInstance))
			{
				ActivateMenu(menuInstance);
				return menuInstance;
			}

			Debug.LogError($"The menu does not exist");
			return null;
		}

		// ReSharper disable Unity.PerformanceAnalysis
		public void ActivateMenu(Menu menu)
		{
			if (menu is null)
			{
				Debug.LogError("Unable to activate menu because it is null.");
				return;
			}

			// Don't activate the menu if it's already active
			if (menu.MenuType != MenuType.NoSequencing && menu == ActiveMenu)
			{
				return;
			}

			// Get the previous menu.
			var prevActiveMenu = ActiveMenu;

			// Push the new menu onto the stack.
			if (menu.MenuType != MenuType.NoSequencing)
				MenuStack.Add(menu);

			if (prevActiveMenu is not null && 
			    prevActiveMenu.MenuType == MenuType.Sequencing &&
			    menu.MenuType != MenuType.NoSequencing)
			{
				prevActiveMenu.Hide();
				OnManuDeactivated?.Invoke(prevActiveMenu);
			}

			menu.Show();
			OnManuActivated?.Invoke(menu);
		}
		
		#endregion

		#region Menu Deactivation
		
		public Menu DeactivateTheLastMenu()
		{
			if (ActiveMenu != null) 
				return DeactivateMenu(ActiveMenu);
			
			Debug.LogError($"The last menu does not exist");
			return null;

		}
		
		public T DeactivateMenu<T>() where T : Menu
		{
			return DeactivateMenu(typeof(T)) as T;
		}
		
		public Menu DeactivateMenu(Type type)
		{
			// Deactivate the menu by its pre-existing instance (if any)
			if (!TryGetMenu(type, out var menuInstance)) 
				return null;
			
			DeactivateMenu(menuInstance);
			return menuInstance;

		}
		
		// ReSharper disable Unity.PerformanceAnalysis
		public Menu DeactivateMenu(Menu menu)
		{
			if (_menuStack.Count == 0 || MenuStack.All(m => m != menu))
			{
				return null;
			}

			// Remove the menu from stack
			MenuStack.Remove(menu);

			// Activate the menu.
			if (ActiveMenu != null && !ActiveMenu.IsShown)
			{
				ActiveMenu.Show();
				OnManuActivated?.Invoke(ActiveMenu);
			}

			menu.Hide();
			OnManuDeactivated?.Invoke(menu);

			return menu;
		}

		#endregion

		#region Menus managment

		public void SetMenusActive(bool value)
		{
			foreach (var menu in _menuInstances)
			{
				menu.Value.SetActive(value);
			}
		}

		public void AddMenu(Menu menu)
		{
			_menuInstances[menu.GetType()] = menu;
			OnMenuAdded?.Invoke(menu);
		}

		public void RemoveMenu(Menu menu)
		{
			_menuInstances.Remove(menu.GetType());
			OnMenuRemoved?.Invoke(menu);
		}

		public bool TryGetMenu<T>(out T menu)
			where T : Menu
		{
			if (!_menuInstances.TryGetValue(typeof(T), out var menuInstance))
			{
				menu = null;
				return false;
			}

			menu = menuInstance as T;
			return menu != null;
		}

		public bool TryGetMenu(Type menuType, out Menu menu)
		{
			return _menuInstances.TryGetValue(menuType, out menu);
		}

		public Menu GetMenu(Type menuType)
		{
			return TryGetMenu(menuType, out var menu) ? menu : null;
		}

		#endregion

		#region Manager related

		protected override bool HasInitialization() => false;

		#endregion
    }
}
