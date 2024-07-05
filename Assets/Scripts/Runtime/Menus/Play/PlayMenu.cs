using UnityEngine;
using UnityEngine.UI;

namespace MP.Menus.Play
{
    public class PlayMenu : Menu
    {
        [SerializeField] private Card cardPrefab;
        [SerializeField] private RectTransform board;
        [SerializeField] private GridLayoutGroup gridLayout;

        public void Initialize()
        {
            
        }
    }
}