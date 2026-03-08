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
        [SerializeField] CanvasGroup canvasGroup;

        int id;
        bool isRevealed;
        bool isMatched;
        Action<CardView> clickHandler;

        public int Id => id;
        public bool IsMatched => isMatched;
        public bool IsRevealed => isRevealed;

        public void Initialize(int id, Sprite face, Sprite back, Action<CardView> onClicked)
        {
            this.id = id;
            clickHandler = onClicked;

            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

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
            SetInteractable(true);
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

            if (value)
            {
                SetRevealed(true);
                SetInteractable(false);
                gameObject.SetActive(false); // hide matched pair from board
            }
        }

        // Disable button + raycasts so card can't be clicked when locked or matched
        public void SetInteractable(bool value)
        {
            if (button != null)
            {
                button.interactable = value;
            }

            if (frontImage != null)
            {
                frontImage.raycastTarget = value;
            }

            if (backImage != null)
            {
                backImage.raycastTarget = value;
            }

            if (canvasGroup != null)
            {
                canvasGroup.interactable = value;
                canvasGroup.blocksRaycasts = value;
                canvasGroup.alpha = value ? 1f : 0.75f;
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
