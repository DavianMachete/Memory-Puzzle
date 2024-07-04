using System;

using UnityEngine;

// ReSharper disable once CheckNamespace
namespace MP.Core
{
	public abstract class ManagerHandle : MonoBehaviour
	{
        
        // Initialization logic
        public bool IsInitialized => InitializationResult.WasSuccessful;
        public InitializationResult InitializationResult { get; protected set; }
        public event Action<InitializationResult> OnInitialized;

        public bool HasLaunched => _hasAwoken;
        public bool IsLaunching { get; private set; }
        private bool _hasAwoken = false;

        #region Core related

        /// <summary>
        /// Launches the manager handle. And this is called from <see cref="Core"/>.
        /// </summary>
        internal void Launch()
        {
            if (HasLaunched)
            {
                return;
            }

            IsLaunching = true;

            // Awake manager
            if (!_hasAwoken)
            {
                OnAwakeManager();
                _hasAwoken = true;
            }

            IsLaunching = false;
        }

        /// <summary>
        /// Initializes the manager handle. And this is called from <see cref="Core"/>.
        /// </summary>
        internal void Initialize()
        { 
            if (IsInitialized)
                return;

            if (HasInitialization())
                InitializeManager();
            else
                SetManagerInitialized(true);
        }

        #endregion

        /// <summary>
        /// Forces to initialize the manager even if it has already been initialized.
        /// </summary>
        protected void ForceInitialize()
        {
            InitializationResult = InitializationResult.None;
            Initialize();
        }
        
        /// <summary>
        /// Initializes the manager. This is called only if the method
        /// <see cref="HasInitialization"/> returns true.
        /// </summary>
        protected virtual void InitializeManager() { }

        #region Definitions

        /// <summary>
        /// If the manager has initialization then it needs to return <b>true</b> and
        /// after manager initialization, you need to call the <see cref="SetManagerInitialized"/>
        /// method (event) with parameter as initialization result. When this returns <b>true</b> the
        /// <see cref="InitializeManager"/> method will be called on manager initialization so you can override it.
        /// If the manager doesn't have initialization just return <b>false</b> and the
        /// <see cref="SetManagerInitialized"/> method (event) will be called automatically
        /// with a successful result. 
        /// </summary>
        protected abstract bool HasInitialization();

        /// <summary>
        /// Get the load priority. If the method is not overridden it returns 1.
        /// </summary>
        /// <returns>The load priority value.</returns>
        protected internal virtual int GetLoadPriority()
        {
            return 1;
        }

        #endregion

        #region Manager Events

        /// <summary>
        /// Need to be called from derived manager if it has initialization.
        /// <i>(...if <see cref="HasInitialization"/> returns true)</i>
        /// </summary>
        /// <param name="result">It needs to be equal to true if initialization
        /// was successful and false if was failed.</param>
        protected void SetManagerInitialized(bool result)
        {
            InitializationResult = result ?
                InitializationResult.Successful : InitializationResult.Failed;
            OnInitialized?.Invoke(InitializationResult);
        }
        
        /// <summary>
        /// Need to be called from derived manager if it has initialization.
        /// <i>(...if <see cref="HasInitialization"/> returns true)</i>
        /// </summary>
        /// <param name="result">Initialization result type.</param>
        protected void SetManagerInitialized(InitializationResult result)
        {
            OnInitialized?.Invoke(result);
        }

        /// <summary>
        /// Called when the manager awakes, when manager successfully launched.
        /// </summary>
        protected virtual void OnAwakeManager() { }

        /// <summary>
        /// Called before manager will been destroyed.
        /// </summary>
        protected virtual void OnDestroyManager() { }

        #endregion

        #region Private methods

        private void OnDestroy()
        {
            // Call destroy on derived classes
            OnDestroyManager();

            ResetManager();

            if (Core.IsShuttingDown)
            {
                return;
            }

            // Remove ourselves from the core as we're no longer allowed to be used
            var core = Core.Instance;
            core.RemoveManager(this);
        }

        private void ResetManager()
        {
            _hasAwoken = false;
            InitializationResult = InitializationResult.None;
        }

        #endregion
    }
}