using UnityEngine;
using UnityEngine.UI;

namespace MemoryCardGame.UI
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] Image frontImage;
        [SerializeField] Image backImage;

        int id;
        bool isRevealed;

        public int Id => id;

        public void Initialize(int id, Sprite face, Sprite back)
        {
            this.id = id;

            if (frontImage != null)
            {
                frontImage.sprite = face;
            }

            if (backImage != null)
            {
                backImage.sprite = back;
            }

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
    }
}
