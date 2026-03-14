using System.Collections;
using System.Collections.Generic;
using MemoryCardGame.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MemoryCardGame.Core
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] TMP_Text matchesText;
        [SerializeField] TMP_Text turnsText;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] RectTransform boardRoot;
        [SerializeField] GridLayoutGroup gridLayout;
        [SerializeField] GameObject cardPrefab;
        [SerializeField] Sprite[] cardFaces;
        [SerializeField] Sprite cardBack;

        readonly List<CardView> cards = new List<CardView>();
        readonly List<CardView> pending = new List<CardView>(); // cards currently face-up waiting to be compared
        readonly HashSet<CardView> locked = new HashSet<CardView>(); // cards temporarily non-clickable (e.g. mismatch flip-back)

        int matches;
        int totalPairs;
        int turns;
        int score;

        void Start()
        {
            if (!GameSession.HasConfig)
            {
                SceneManager.LoadScene(SceneNames.Start);
                return;
            }

            BuildBoard(GameSession.CurrentConfig);
        }

        void BuildBoard(GameConfig config)
        {
            ClearBoard();
            ConfigureGrid(config);

            int totalCards = config.Rows * config.Columns;
            int pairs = totalCards / 2;

            totalPairs = pairs;
            matches = 0;
            turns = 0;
            score = 0;
            pending.Clear();
            locked.Clear();

            var ids = CreateShuffledIds(pairs, totalCards); // each pair id appears twice, then shuffle

            for (int i = 0; i < totalCards; i++)
            {
                var instance = Instantiate(cardPrefab, gridLayout.transform);
                var view = instance.GetComponent<CardView>();

                if (view != null)
                {
                    int id = ids[i];
                    var face = GetFaceSprite(id);
                    view.Initialize(id, face, cardBack, OnCardClicked);
                    cards.Add(view);
                }
            }

            RefreshHud();
        }

        void ClearBoard()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] != null)
                {
                    Destroy(cards[i].gameObject);
                }
            }

            cards.Clear();
        }

        void ConfigureGrid(GameConfig config)
        {
            if (gridLayout == null)
            {
                return;
            }

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = config.Columns;

            var size = CalculateCellSize(config);
            gridLayout.cellSize = size;
        }

        Vector2 CalculateCellSize(GameConfig config)
        {
            if (boardRoot == null)
            {
                return gridLayout.cellSize;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(boardRoot); // ensure rect is valid before reading size

            var rect = boardRoot.rect;
            var width = rect.width;
            var height = rect.height;

            if (width <= 0f || height <= 0f)
            {
                return gridLayout.cellSize;
            }

            var cellWidth = width / config.Columns;
            var cellHeight = height / config.Rows;

            var size = Mathf.FloorToInt(Mathf.Min(cellWidth, cellHeight));

            return new Vector2(size, size);
        }

        // Build [0,0,1,1,...] then Fisher-Yates shuffle for O(n) random layout
        List<int> CreateShuffledIds(int pairs, int totalCards)
        {
            var ids = new List<int>(totalCards);

            for (int i = 0; i < pairs; i++)
            {
                ids.Add(i);
                ids.Add(i);
            }

            for (int i = ids.Count; i < totalCards; i++)
            {
                ids.Add(ids[i % ids.Count]);
            }

            for (int i = ids.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int temp = ids[i];
                ids[i] = ids[j];
                ids[j] = temp;
            }

            return ids;
        }

        Sprite GetFaceSprite(int id)
        {
            if (cardFaces == null || cardFaces.Length == 0)
            {
                return null;
            }

            int index = id % cardFaces.Length;
            return cardFaces[index];
        }

        void OnCardClicked(CardView card)
        {
            if (card == null)
            {
                return;
            }

            if (card.IsMatched)
            {
                return;
            }

            if (locked.Contains(card))
            {
                return;
            }

            if (pending.Contains(card))
            {
                return;
            }

            card.SetRevealed(true);
            pending.Add(card);

            // Process pairs in a loop so rapid clicks don't leave pending stuck with 2+ cards
            while (pending.Count >= 2)
            {
                var first = pending[0];
                var second = pending[1];
                pending.RemoveAt(1);
                pending.RemoveAt(0);

                locked.Add(first);
                locked.Add(second);

                turns++;
                RefreshHud();

                if (first.Id == second.Id)
                {
                    first.SetMatched(true);
                    second.SetMatched(true);

                    locked.Remove(first);
                    locked.Remove(second);

                    matches++;
                    score += 100;
                    RefreshHud();

                    if (matches >= totalPairs)
                    {
                        OnGameCompleted();
                        return;
                    }
                }
                else
                {
                    score -= 10;
                    if (score < 0)
                    {
                        score = 0;
                    }

                    RefreshHud();
                    StartCoroutine(HidePair(first, second));
                }
            }
        }

        IEnumerator HidePair(CardView first, CardView second)
        {
            yield return new WaitForSeconds(0.5f);

            first.SetRevealed(false);
            second.SetRevealed(false);
            locked.Remove(first);
            locked.Remove(second); // now clickable again for next turn
        }

        void OnGameCompleted()
        {
            GameSession.LastResult = new GameResult(score, turns, totalPairs);

            // Unlock the next level (next button) for this run, if any
            if (GameSession.CurrentLevelIndex >= GameSession.MaxLevelUnlocked)
            {
                GameSession.MaxLevelUnlocked = GameSession.CurrentLevelIndex + 1;
            }

            SceneManager.LoadScene(SceneNames.Result);
        }

        void RefreshHud()
        {
            if (matchesText != null)
            {
                matchesText.text = "Matches - " + matches;
            }

            if (turnsText != null)
            {
                turnsText.text = "Turns - " + turns;
            }

            if (scoreText != null)
            {
                scoreText.text = "Score - " + score;
            }
        }
    }
}
