using System.Collections.Generic;
using UnityEngine;

using Invaders.Locator;

namespace Invaders.StateMachine
{
    public class StatePreparator : ServiceRegister<StatePreparator>
    {
        [SerializeField] private List<MonoBehaviour> _components = new List<MonoBehaviour>();

        private void Awake() => RegisterService(this);


        public void DisableComponents()
        {
            foreach (var cmp in _components)
            {
                if (cmp.IsNull()) continue;
                cmp.enabled = false;
            }
        }


        public void EnableComponents()
        {
            foreach (var cmp in _components)
            {
                if (cmp.IsNull()) continue;
                cmp.enabled = true;
            }
        }
    }
}