using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MP.Menus.Background
{
    public class BackgroundMenu : Menu
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private Sprite[] backgrounds;

        protected override void OnMenuStart()
        {
            base.OnMenuStart();

            // Activate this menu.
            var mm = MenuManager.Instance;
            mm.ActivateMenu(this);
            
            // Update background on each menu activation.
            mm.OnManuActivated += UpdateBackground;
        }

        protected override void OnMenuDestroy()
        {
            var mm = MenuManager.Instance;
            mm.OnManuActivated -= UpdateBackground;
            
            base.OnMenuDestroy();
        }

        protected override void AnimateShow(Action onAnimated)
        {
            // We do not need to animate this menu
        }

        protected override void AnimateHide(Action onAnimated)
        {
            // We do not need to animate this menu
        }

        private void UpdateBackground(Menu menu)
        {
            var range = backgrounds.Length;
            var random = Random.Range(0, range);
            bgImage.sprite = backgrounds[random];
        }
    }
}