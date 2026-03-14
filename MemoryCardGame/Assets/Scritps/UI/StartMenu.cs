using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace MemoryCardGame
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] GameObject startPanel;
        [SerializeField] GameObject layoutPanel;
        [SerializeField] Button startButton;

        // Prefab for a single level button (with a Text or TMP_Text child for the label)
        [SerializeField] Button levelButtonPrefab;

        // ScrollRect that holds all level buttons (for smooth scrolling)
        [SerializeField] ScrollRect levelScrollRect;

        // Parent/container where level buttons will be spawned (usually the ScrollRect content)
        [SerializeField] Transform levelButtonContainer;

        // How many levels are generated in one batch before expanding again
        [SerializeField] int levelsPerBatch = 10;

        void Awake()
        {
            if (startPanel != null)
            {
                startPanel.SetActive(true);
            }

            if (layoutPanel != null)
            {
                layoutPanel.SetActive(false);
            }

            GenerateLevelButtons();
        }
        void Start()
        {
            startButton.onClick.AddListener(()=> OpenLayoutSelection());
        }

        public void OpenLayoutSelection()
        {
            if (startPanel != null)
            {
                startPanel.SetActive(false);
            }

            if (layoutPanel != null)
            {
                layoutPanel.SetActive(true);
            }

          //  AlignScrollToMiddleClamped();
        }

        void StartGame(int levelIndex, int rows, int columns)
        {
            GameSession.CurrentLevelIndex = levelIndex;
            GameSession.CurrentConfig = new GameConfig(rows, columns);
            SceneManager.LoadScene(SceneNames.Game);
        }

        void GenerateLevelButtons()
        {
            if (levelButtonPrefab == null || levelButtonContainer == null)
            {
                return;
            }

            // Clear any existing children (in case we re-enter this scene)
            for (int i = levelButtonContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(levelButtonContainer.GetChild(i).gameObject);
            }

            int highestUnlocked = GameSession.MaxLevelUnlocked;

            // Work out how many levels we should show:
            // always show at least one full "batch" ahead of the highest unlocked level.
            int batchIndex = highestUnlocked / levelsPerBatch;
            int totalLevelsToCreate = (batchIndex + 1) * levelsPerBatch;

            for (int levelIndex = 0; levelIndex < totalLevelsToCreate; levelIndex++)
            {
                int rows, columns;
                GetLayoutForLevel(levelIndex, out rows, out columns);

                var buttonInstance = Instantiate(levelButtonPrefab, levelButtonContainer);

                // Label: "Level 1 (2x2)" etc.
                string label = $"Level {levelIndex + 1} ({rows}x{columns})";

                var tmp = buttonInstance.GetComponentInChildren<TMP_Text>();
                if (tmp != null)
                {
                    tmp.text = label;
                }
                else
                {
                    var text = buttonInstance.GetComponentInChildren<Text>();
                    if (text != null)
                    {
                        text.text = label;
                    }
                }

                int capturedIndex = levelIndex;
                int capturedRows = rows;
                int capturedColumns = columns;

                buttonInstance.onClick.AddListener(
                    () => StartGame(capturedIndex, capturedRows, capturedColumns));

                // Only allow clicking levels up to the highest unlocked index
                buttonInstance.interactable = capturedIndex <= highestUnlocked;
            }

            // After creating buttons, make sure the scroll starts at the top
            if (levelScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
               // AlignScrollToMiddleClamped();
            }
        }

        void AlignScrollToMiddleClamped()
        {
            if (levelScrollRect == null)
            {
                return;
            }

            // Clamp movement + position to avoid overscroll/jitter
            levelScrollRect.movementType = ScrollRect.MovementType.Clamped;
            levelScrollRect.verticalNormalizedPosition = Mathf.Clamp01(0.5f); // mid
        }

        // Map a level index to a rows/columns layout.
        // First 10 levels follow a clear pattern starting from 2x2, then
        // each further batch of 10 gets slightly larger grids.
        void GetLayoutForLevel(int levelIndex, out int rows, out int columns)
        {
            // Base layouts for the first 10 levels
            int[,] baseLayouts = new int[,]
            {
                { 2, 2 }, // Level 1
                { 2, 3 }, // Level 2
                { 3, 2 }, // Level 3
                { 2, 4 }, // Level 4
                { 3, 3 }, // Level 5
                { 4, 2 }, // Level 6
                { 3, 4 }, // Level 7
                { 4, 3 }, // Level 8
                { 4, 4 }, // Level 9
                { 4, 5 }  // Level 10
            };

            int patternLength = baseLayouts.GetLength(0);

            int batch = levelIndex / patternLength;
            int indexInBatch = levelIndex % patternLength;

            rows = baseLayouts[indexInBatch, 0] + batch;
            columns = baseLayouts[indexInBatch, 1] + batch;
        }
    }
}