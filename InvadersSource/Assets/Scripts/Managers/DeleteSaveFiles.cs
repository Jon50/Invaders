using UnityEngine;

using Invaders.Save;
using static Invaders.Static.ConstantValues;

namespace Invaders.Managers
{
    public class DeleteSaveFiles : MonoBehaviour
    {
        public void DeleteHiScoreFile()
        {
            SavingSystem.DeleteSave(HIGH_SCORE_SAVE);
        }
    }
}