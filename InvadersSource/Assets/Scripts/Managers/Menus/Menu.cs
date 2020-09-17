using System.Collections.Generic;
using UnityEngine;

namespace Invaders.Managers
{
    public abstract class Menu : MonoBehaviour
    {
        [SerializeField] protected GameObject parentMenu;
        protected Stack<GameObject> subMenus = new Stack<GameObject>();

        protected abstract void ToggleParentMenu();

        private void Update() => OnButtonOrKey_ToggleOrReturnMenu();


        public void OnButtonOrKey_ToggleOrReturnMenu(bool isUIButton = false)
        {
            if (PlayerInputRef.PlayerInput.actions["CancelReturn"].triggered || isUIButton)
            {
                if (subMenus.Count > 0)
                {
                    RemoveSubMenu();

                    if (subMenus.Count <= 0)
                        parentMenu?.SetActive(true);

                    return;
                }

                if (subMenus.Count <= 0)
                    ToggleParentMenu();
            }
        }


        public void OnClick_AddSubMenu(GameObject subMenu)
        {
            parentMenu.SetActive(false);

            if (subMenus.Count > 0)
                subMenus.Peek().SetActive(false);

            subMenus.Push(subMenu);
            subMenus.Peek().SetActive(true);
        }


        protected void RemoveSubMenu()
        {
            subMenus.Peek().SetActive(false);
            subMenus.Pop();

            if (subMenus.Count > 0)
                subMenus.Peek().SetActive(true);
        }
    }
}