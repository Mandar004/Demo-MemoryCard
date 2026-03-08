using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace MemoryCardGame.UI
{
    public class ResultController : MonoBehaviour
    {
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text turnsText;
        [SerializeField] Image star1;
        [SerializeField] Image star2;
        [SerializeField] Image star3;
        [SerializeField] Sprite starFilled;
        [SerializeField] Sprite starEmpty;
        [SerializeField] Button homeButton;
        [SerializeField] Button restartButton;

        void Start()
        {
            var result = GameSession.LastResult;

            if (!result.HasValue)
            {
                SceneManager.LoadScene(SceneNames.Start);
                return;
            }

            var r = result.Value;
            int stars = GetStarCount(r);

            if (scoreText != null) scoreText.text = "Score: " + r.Score;
            if (turnsText != null) turnsText.text = "Turns: " + r.Turns;

            if (star1 != null) star1.sprite = stars >= 1 ? starFilled : starEmpty;
            if (star2 != null) star2.sprite = stars >= 2 ? starFilled : starEmpty;
            if (star3 != null) star3.sprite = stars >= 3 ? starFilled : starEmpty;

            if (homeButton != null) homeButton.onClick.AddListener(GoHome);
            if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        }

        // Stars based on turns vs optimal (1 turn per pair): 3★ ≤1.2x, 2★ ≤1.5x, 1★ else
        static int GetStarCount(GameResult r)
        {
            if (r.TotalPairs <= 0) return 0;
            float ratio = (float)r.Turns / r.TotalPairs;
            if (ratio <= 1.2f) return 3;
            if (ratio <= 1.5f) return 2;
            return 1;
        }

        void GoHome()
        {
            GameSession.LastResult = null;
            SceneManager.LoadScene(SceneNames.Start);
        }

        void RestartGame()
        {
            GameSession.LastResult = null;
            SceneManager.LoadScene(SceneNames.Game);
        }
    }
}
