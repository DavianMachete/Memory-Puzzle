using UnityEngine;
using UnityEngine.UI;

using MP.Cards;

namespace MP.Menus.Play
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Button button;
        [SerializeField] private RectTransform rectTransform;

        public string Id { get; private set; }

        private bool _isOpened;
        
        public void Initialize(CardData data)
        {
            Id = data.id;
            image.sprite = data.sprite;

            _isOpened = false;
        }

        private void Close()
        {
            
        }
        
        private void Open()
        {
            
        }
    }
}