using UnityEngine;
using UnityEngine.SceneManagement;
using Invaders.Enums;

namespace Invaders.Managers
{
    [CreateAssetMenu(menuName = "SceneLoader")]
    public class SceneLoader : ScriptableObject
    {
        public void StartGame() => SceneManager.LoadScene((int)Level.Game);

        public void MainMenu() => SceneManager.LoadScene((int)Level.MainMenu);

        public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void QuitGame() => Application.Quit();
    }
}