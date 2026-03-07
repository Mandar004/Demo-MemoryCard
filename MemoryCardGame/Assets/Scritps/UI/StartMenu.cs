using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MemoryCardGame
{
    public class StartMenu : MonoBehaviour
    {
        [System.Serializable]
        public struct SelectionMode
        {
            public Button button;
            public int rows;
            public int columns;
        }

        [SerializeField] GameObject startPanel;
        [SerializeField] GameObject layoutPanel;
        [SerializeField] Button startButton;
        [SerializeField] SelectionMode[] layoutOptions;

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

            for (int i = 0; i < layoutOptions.Length; i++)
            {
                var option = layoutOptions[i];
                var button = option.button;
                var rows = option.rows;
                var columns = option.columns;

                if (button != null)
                {
                    button.onClick.AddListener(() => StartGame(rows, columns));
                }
            }
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
        }

        void StartGame(int rows, int columns)
        {
            GameSession.CurrentConfig = new GameConfig(rows, columns);
            SceneManager.LoadScene(SceneNames.Game);
        }
    }
}