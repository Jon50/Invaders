using UnityEngine;

namespace Invaders.Combat
{
    public interface IProcessHit
    {
        void ProcessHit(GameObject obj = default);
    }
}