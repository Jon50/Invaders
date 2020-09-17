using System;
using UnityEngine;

namespace Invaders.Combat
{
    public class AlienTarget : MonoBehaviour, ICombatTarget
    {
        private IProcessHit[] _processHits;

        private void Awake() => _processHits = GetComponentsInParent<IProcessHit>();
        public void ProcessHit() => Array.ForEach(_processHits, element => element.ProcessHit(this.gameObject));
    }
}