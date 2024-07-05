using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using MP.Cards;

namespace MP.Menus.Play
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Button button;

        [SerializeField] private float animationDuration = 0.28f;
        [SerializeField] private GameObject backgroundObject;
        [SerializeField] private RectTransform rectTransform;

        public string Id { get; private set; }

        private bool _isOpened;

        private void Awake()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        public void Initialize(CardData data)
        {
            Id = data.id;
            image.sprite = data.sprite;
            image.preserveAspect = true;

            _isOpened = false;
        }

        public void DestroyCard()
        {
            Destroy(gameObject);
        }

        private void OnClick()
        {
            if(_isOpened)
                return;

            Open();
        }

        private void Close()
        {
            rectTransform.DOKill();
            rectTransform.DORotate(Vector3.up * 90f, animationDuration / 2f).OnComplete(() =>
            {
                backgroundObject.SetActive(true);
                rectTransform.DORotate(Vector3.zero, animationDuration / 2f);
                _isOpened = false;
            });
        }
        
        private void Open()
        {
            rectTransform.DOKill();
            rectTransform.DORotate(Vector3.up * 90f, animationDuration / 2f).OnComplete(() =>
            {
                backgroundObject.SetActive(false);
                rectTransform.DORotate(Vector3.zero, animationDuration / 2f);
                _isOpened = true;
            });
        }
    }
}