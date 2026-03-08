using System.Collections;
using System.Collections.Generic;
using MemoryCardGame.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MemoryCardGame.Core
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] RectTransform boardRoot;
        [SerializeField] GridLayoutGroup gridLayout;
        [SerializeField] GameObject cardPrefab;
        [SerializeField] Sprite[] cardFaces;
        [SerializeField] Sprite cardBack;

        readonly List<CardView> cards = new List<CardView>();
        readonly List<CardView> openCards = new List<CardView>();
        readonly HashSet<CardView> closingCards = new HashSet<CardView>();

        int matches;
        int totalPairs;
        int turns;

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

            var ids = CreateShuffledIds(pairs, totalCards);

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

            LayoutRebuilder.ForceRebuildLayoutImmediate(boardRoot);

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

            if (closingCards.Contains(card))
            {
                return;
            }

            if (openCards.Contains(card))
            {
                return;
            }

            card.SetRevealed(true);
            openCards.Add(card);

            if (openCards.Count < 2)
            {
                return;
            }

            var first = openCards[openCards.Count - 2];
            var second = openCards[openCards.Count - 1];

            if (first == second)
            {
                return;
            }

            turns++;

            if (first.Id == second.Id)
            {
                first.SetMatched(true);
                second.SetMatched(true);

                matches++;

                openCards.Remove(first);
                openCards.Remove(second);

                if (matches >= totalPairs)
                {
                    OnGameCompleted();
                }
            }
            else
            {
                StartCoroutine(HidePair(first, second));
            }
        }

        IEnumerator HidePair(CardView first, CardView second)
        {
            closingCards.Add(first);
            closingCards.Add(second);

            yield return new WaitForSeconds(0.5f);

            first.SetRevealed(false);
            second.SetRevealed(false);

            closingCards.Remove(first);
            closingCards.Remove(second);

            openCards.Remove(first);
            openCards.Remove(second);
        }

        void OnGameCompleted()
        {
            SceneManager.LoadScene(SceneNames.Result);
        }
    }
}
