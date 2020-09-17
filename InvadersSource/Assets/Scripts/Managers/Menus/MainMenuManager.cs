namespace Invaders.Managers
{
    public class MainMenuManager : Menu
    {
        private void Start()
        {
            var audioManager = GetComponent<AudioManager>();
            audioManager.PlayMusic("MainMenu");
        }


        protected override void ToggleParentMenu() => parentMenu.SetActive(true);
    }
}