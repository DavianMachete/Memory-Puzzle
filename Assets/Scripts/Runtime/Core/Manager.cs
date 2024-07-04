using UnityEngine;

// ReSharper disable once CheckNamespace
namespace MP.Core
{
	public abstract class Manager<T> : ManagerHandle where T : ManagerHandle
	{

		// Singleton
		private static bool _hasInstance;
		private static T _instance;
		public static T Instance
		{
			get
			{
				if (_hasInstance)
				{
					return _instance;
				}

				// Fetch & cache our instance
				_instance = Core.Instance.GetManager<T>();
				_hasInstance = _instance != null;
				return _instance;
			}
		}
	}
}