using Invaders.StateMachine;

namespace Invaders.Managers
{
    public class PauseMenuManager : Menu
    {
        private GameManager _gameManager;

        private void Start() => _gameManager = GetComponent<GameManager>();

        protected override void ToggleParentMenu()
        {
            // parentMenu.SetActive(!parentMenu.activeSelf);

            // if (parentMenu.activeSelf)
            //     _gameManager.SetNewState(GamePauseState.Instance);
            // else
            //     _gameManager.SetNewState(GamePlayingState.Instance);

            if (!GamePauseState.Instance.IsGamePaused)
            {
                _gameManager.SetNewState(GamePauseState.Instance);

                if (GamePauseState.Instance.IsGamePaused)
                    parentMenu.SetActive(!parentMenu.activeSelf);
            }
            else
            {
                _gameManager.SetNewState(GamePlayingState.Instance);

                if (!GamePauseState.Instance.IsGamePaused)
                    parentMenu.SetActive(!parentMenu.activeSelf);
            }
        }
    }
}