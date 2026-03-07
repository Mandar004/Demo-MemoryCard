using UnityEngine;
using UnityEngine.SceneManagement;

namespace MemoryCardGame
{
    public class StartMenu : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene(SceneNames.Game);
        }
    }
}