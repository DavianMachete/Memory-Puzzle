using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using MP.Audio;
using MP.Cards;
using MP.Game;

namespace MP.Menus.Play
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Button button;
        [SerializeField] private GameObject blocker;

        [SerializeField] private float animationDuration = 0.28f;
        [SerializeField] private GameObject backgroundObject;
        [SerializeField] private RectTransform rectTransform;

        public string Id { get; private set; }
        public bool IsOpened { get; private set; }
        public bool IsHidden { get; private set; }

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

            SetBlock(false);
            IsHidden = false;
            IsOpened = false;
        }

        public void SetBlock(bool value)
        {
            blocker.SetActive(value);
        }

        public void Close()
        {
            rectTransform.DOKill();
            rectTransform.DORotate(Vector3.up * 90f, animationDuration / 2f).OnComplete(() =>
            {
                backgroundObject.SetActive(true);
                rectTransform.DORotate(Vector3.zero, animationDuration / 2f);
                IsOpened = false;
            });
        }

        public void Hide()
        {
            SetBlock(true);
            IsHidden = true;
            
            rectTransform.DOKill();
            rectTransform.DOScale(Vector3.zero, animationDuration);
        }

        public void DestroyCard()
        {
            Destroy(gameObject);
        }

        private void OnClick()
        {
            if(IsOpened)
                return;

            Open();
        }
        
        private void Open()
        {
            var gm = GameManager.Instance;
            gm.AddCardToCompare(this);

            var am = AudioManager.Instance;
            am.PlayCardFlip();
            
            rectTransform.DOKill();
            rectTransform.DORotate(Vector3.up * 90f, animationDuration / 2f).OnComplete(() =>
            {
                backgroundObject.SetActive(false);
                rectTransform.DORotate(Vector3.zero, animationDuration / 2f);
                IsOpened = true;
            });
        }
    }
}