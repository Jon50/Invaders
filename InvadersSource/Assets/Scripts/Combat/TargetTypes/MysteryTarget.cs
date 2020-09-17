using System;
using UnityEngine;

namespace Invaders.Combat
{
    public class MysteryTarget : MonoBehaviour, ICombatTarget
    {
        private IProcessHit[] _processHits;

        private void Awake() => _processHits = GetComponents<IProcessHit>();
        public void ProcessHit() => Array.ForEach(_processHits, element => element.ProcessHit());
    }
}