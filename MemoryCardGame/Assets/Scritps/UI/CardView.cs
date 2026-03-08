using System;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryCardGame.UI
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] Image frontImage;
        [SerializeField] Image backImage;
        [SerializeField] Button button;

        int id;
        bool isRevealed;
        bool isMatched;
        Action<CardView> clickHandler;

        public int Id => id;
        public bool IsMatched => isMatched;

        public void Initialize(int id, Sprite face, Sprite back, Action<CardView> onClicked)
        {
            this.id = id;
            clickHandler = onClicked;

            if (frontImage != null)
            {
                frontImage.sprite = face;
            }

            if (backImage != null)
            {
                backImage.sprite = back;
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(HandleClick);
            }

            isMatched = false;
            SetRevealed(false);
        }

        public void SetRevealed(bool revealed)
        {
            isRevealed = revealed;

            if (frontImage != null)
            {
                frontImage.gameObject.SetActive(revealed);
            }

            if (backImage != null)
            {
                backImage.gameObject.SetActive(!revealed);
            }
        }

        public void SetMatched(bool value)
        {
            isMatched = value;

            if (button != null)
            {
                button.interactable = !value;
            }
        }

        void HandleClick()
        {
            if (clickHandler != null)
            {
                clickHandler(this);
            }
        }
    }
}
