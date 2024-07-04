using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using MP.Tools;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MP.Core
{
	public class Core : MonoBehaviour
	{
		// Singleton
		private static Core _instance;
		public static Core Instance
		{
			get
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					Debug.LogWarning("Instance request not allowed in Editor mode.");
					return null;
				}
#endif
				if (!_hasInstance && !_isShuttingDown)
				{
					_instance = CreateNewInstance();
				}

				return _instance;
			}
		}

		// Misc
		private static bool _hasInstance;

		// Roots
		private Transform _persistentRoot, _persistentManagerRoot;

		// State
		private static bool _isShuttingDown;
		public static bool IsShuttingDown => _isShuttingDown;
		private bool _isInitialized;

		public static bool IsHeadlessModeEnabled { get; private set; }

		// Managers
		private List<ManagerHandle> _persistentManagerPrefabs;
		private readonly Dictionary<Type, ManagerHandle> _persistentManagers = new ();

		#region Initialization and verification

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void KickStart()
		{
			if (_isShuttingDown)
			{
				return;
			}

			if (_instance == null)
			{
				_instance = Instance; // Invokes singleton
			}
		}

		private static Core CreateNewInstance()
		{
			// Load the core prefab from Resources (default name is "Core.prefab")
			const string coreName = "Core";

			Core core;
			var corePrefab = ResourcesManager.Load<Core>(coreName);
			if (corePrefab != null)
			{
				core = Instantiate(corePrefab);
			}
			else
			{
				Debug.LogWarning($"Core prefab not found. Please put {coreName}.prefab in the Resources " +
					"folder. Using an empty core instead.");
				var coreGo = new GameObject(coreName);
				core = coreGo.AddComponent<Core>();
			}

			core.name = coreName;
			return core;
		}

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
			Initialize();
		}

		private void Initialize()
		{
			if (_isInitialized)
			{
				return;
			}

			Application.targetFrameRate = 300;

			// Debug initialization
			Debug.Log("Core initializing...");
			
			// Assign a new instance
			_instance = this;
			_hasInstance = true;

			// Cache batch mode usage
			IsHeadlessModeEnabled = GetIsHeadlessModeEnabled();

			// Cache all manager prefabs from Resources, load, launch and initialize
			CacheManagerPrefabs();
			LoadCoreTransforms();
			LoadPersistentManagers();
			LaunchPersistentManagers();
			InitializePersistentManagers();

			_isInitialized = true;
			Debug.Log("Core initialized.");
		}

		private void LoadCoreTransforms()
		{
			if (_persistentRoot is not null) 
				return;
			
			// Create the persistent root
			var persistentRootGo = new GameObject("~ Engine (persistent) ~");
			var persistentRoot = persistentRootGo.transform;
			_persistentRoot = persistentRoot;
			persistentRoot.SetSiblingIndex(0);
			DontDestroyOnLoad(persistentRootGo);

			// Create the persistent managers root
			var childGo = new GameObject("Managers");
			_persistentManagerRoot = childGo.transform;
			_persistentManagerRoot.SetParent(persistentRoot, false);

			// Parent ourselves under the persistent root
			transform.SetParent(persistentRoot);
		}

		#endregion

		#region Managers

		private void CacheManagerPrefabs()
		{
			// Load all manager prefabs from both the engine and the game
			var engineManagerPrefabs = new List<ManagerHandle>(ResourcesManager.LoadAll<ManagerHandle>("Managers",
				ResourcesManager.LoadMode.EngineAssetsOnly));
			var gameManagerPrefabs = ResourcesManager.LoadAll<ManagerHandle>("Managers",
				ResourcesManager.LoadMode.GameAssetsOnly);

			// Filter out engine managers that are inherited by game managers
			foreach (var gameManagerPrefab in gameManagerPrefabs)
			{
				var type = gameManagerPrefab.GetType();

				for (var y = 0; y < engineManagerPrefabs.Count; y++)
				{
					var engineManager = engineManagerPrefabs[y];
					var engineManagerType = engineManager.GetType();
					if (!type.IsSubclassOf(engineManagerType) && type != engineManagerType) 
						continue;
					
					engineManagerPrefabs.RemoveAt(y);
					y--;
				}
			}

			// Combine the resulting engine managers and the game managers
			_persistentManagerPrefabs = new List<ManagerHandle>(engineManagerPrefabs);
			_persistentManagerPrefabs.AddRange(gameManagerPrefabs);
			_persistentManagerPrefabs = _persistentManagerPrefabs
				.OrderBy((m) => m.GetLoadPriority())
				.ToList();
		}

		private void LoadPersistentManagers()
		{
			Debug.Log("Loading core managers...");

			if (_persistentManagerPrefabs == null)
			{
				return;
			}

			foreach (var manager in _persistentManagerPrefabs)
			{
				if (manager == null)
				{
					Debug.LogWarning("Found null manager. Ignoring.");
					continue;
				}

				LoadManager(manager);
			}
		}

		private void LoadManager(ManagerHandle managerPrefab)
		{
			if (managerPrefab == null)
			{
				Debug.LogWarning("Can't load manager by prefab because the prefab is null.");
				return;
			}

			// Fetch the type and CoreTransform of the manager
			var managerType = managerPrefab.GetType();

			// Make sure a manager of the same type does not already exist
			if (!_persistentManagers.TryGetValue(managerType, out var preExistingManager))
			{
				// Instantiate the manager
				var newInst = Instantiate(managerPrefab, _persistentManagerRoot, false);
				newInst.name = managerType.Name;

				// Add the manager
				AddManager(newInst);
			}
			else
			{
				Debug.LogWarning($"Tried to load persistent manager \"{managerPrefab.name}\" " +
					$"but it already exists in the scene as \"{preExistingManager.name}\". Skipping.",
					managerPrefab);
			}
		}
		
        private void LaunchPersistentManagers()
        {
            Debug.Log("Launching persistent managers...");
            var launchedManagers =
	            _persistentManagers.Values.Where(persistentManager => !persistentManager.HasLaunched);
            foreach (var persistentManager in launchedManagers)
            {
	            try
	            {
		            Debug.Log("Launching persistent manager " +
		                       $"\"{persistentManager.name}\"...", persistentManager);
		            persistentManager.Launch();
	            }
	            catch (Exception ex)
	            {
		            Debug.LogException(ex);
	            }
            }
		}

        private void InitializePersistentManagers()
        {
            Debug.Log( "Initializing persistent managers...");
            var initializedManagers =
	            _persistentManagers.Values.Where(manHandle => !manHandle.IsInitialized);
            foreach (var persistentManager in initializedManagers)
            {
                try
                {
                    Debug.Log("Initializing persistent manager " +
                        $"\"{persistentManager.name}\"...", persistentManager);
                    persistentManager.Initialize();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        private void AddManager(ManagerHandle managerHandle)
		{
			if (managerHandle is null)
			{
				return;
			}

			// Register the manager with the appropriate persistence collection
			var managerType = managerHandle.GetType();
			managerHandle.transform.SetParent(_persistentManagerRoot);
			_persistentManagers[managerType] = managerHandle;

			// Log the load
			Debug.Log($"Loaded persistent manager \"{managerHandle.name}\".", managerHandle);
		}

		internal void RemoveManager<T>() where T : ManagerHandle
		{
			RemoveManager(typeof(T));
		}

		private bool RemoveManager(Type managerType)
		{
			// Make sure the manager exists before trying to remove it
			if (!_persistentManagers.ContainsKey(managerType))
			{
				Debug.LogWarning($"Tried to remove manager of type \"{managerType.Name}\" but no such " +
					"manager has been added.");
				return false;
			}

			// Remove the manager. 
			_persistentManagers.Remove(managerType);
			return true;
		}

		internal bool RemoveManager(ManagerHandle manager)
		{
			return manager != null && RemoveManager(manager.GetType());
		}

		public T GetManager<T>() where T : ManagerHandle
		{
			// Attempt to fetch the manager by its type
			T manager = null;
			if (_persistentManagers.TryGetValue(typeof(T), out var managerHandle))
			{
				manager = managerHandle as T;
			}
			else
			{
				// Attempt to find the manager via base type finding (O(N) operation!)
				foreach (var keyPair in _persistentManagers)
				{
					var managerInstance = keyPair.Value;
					var castedManager = managerInstance as T;
					if (castedManager == null) 
						continue;
					
					manager = castedManager;
					break;
				}
			}

            // If the manager was found and is not yet initialized, initialize it
            if (manager == null || manager.HasLaunched || manager.IsLaunching) 
	            return manager;
            
            try
            {
	            manager.Launch();
            }
            catch (Exception exception)
            {
	            Debug.LogException(exception);
            }

            return manager;
		}

		#endregion

		public static void Quit()
		{
			Application.Quit();
		}

		private static bool GetIsHeadlessModeEnabled()
		{
			// Load the batch mode config
			var args = Environment.GetCommandLineArgs();

			// Note that the arguments can be null on platforms other than standalone windows
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			return args != null &&
			       // Find the batch mode argument
			       args.Any(arg => arg.Equals("-batchmode"));
		}

		#region Events

		private void OnDestroy()
		{
			if (_instance == this)
			{
				_hasInstance = false;
			}
		}

		private void OnApplicationQuit()
		{
			_isShuttingDown = true;
		}

		#endregion

		#region Clear Data

#if UNITY_EDITOR
		[MenuItem("Core/Cache/Clear PlayerPrefs")]
		public static void DeveloperClearPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
			Debug.Log("PlayerPrefs cleared");
		}

		[MenuItem("Core/Cache/Clear PersistentData")]
		public static void DeveloperClearPersistentData()
        {
            DeleteDirectory(Application.persistentDataPath);

			Debug.Log("The PersistentDataPath is cleared");
		}

		private static void DeleteDirectory(string targetDir)
		{
            var files = Directory.GetFiles(targetDir);
			var dirs = Directory.GetDirectories(targetDir);

			foreach (var file in files)
			{
				File.Delete(file);
			}

			foreach (var dir in dirs)
			{
				DeleteDirectory(dir);
			}

			Directory.Delete(targetDir, false);
		}
#endif
		#endregion
	}
}