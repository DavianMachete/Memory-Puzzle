using System;

using UnityEngine;

namespace MP.Menus
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MenuElement : MonoBehaviour
    {
        public bool IsShown { get; private set; }
        protected CanvasGroup CanvasGroup;

        #region Methods -> Unity callbacks

        private void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            AnimateHide(null);
        }

        private void Start()
        {
            OnMenuStart();
        }

        private void OnDestroy()
        {
            OnMenuDestroy();
        }
        
        #endregion

        #region Methods -> Public Menu

        // ReSharper disable Unity.PerformanceAnalysis
        public void Show(Action onAnimated = null)
        {
            AnimateShow(onAnimated);
            IsShown = true;
        }

        public void Hide(Action onAnimated = null)
        {
            AnimateHide(onAnimated);
            IsShown = false;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        #endregion

        #region Menu Element Behaviour

        protected virtual void OnMenuStart() { }
        
        protected virtual void OnMenuDestroy() { }

        protected virtual void AnimateShow(Action onAnimated)
        {
            // Dry animation of show. Override this if you will have custom transition
            CanvasGroup.alpha = 1f;
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
            onAnimated?.Invoke();
        }

        protected virtual void AnimateHide(Action onAnimated)
        {
            // Dry animation of hide. Override this if you will have custom transition
            CanvasGroup.alpha = 0f;
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;
            onAnimated?.Invoke();
        }
        
        #endregion
    }
}