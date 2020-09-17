using System.Collections.Generic;
using Invaders.Locator;

namespace Invaders.Preservable
{
    public class ValuePreserver : ServiceRegister<ValuePreserver>
    {
        private static readonly List<IPreservable> _preservables = new List<IPreservable>();
        private static readonly Dictionary<string, object> _preservedValues = new Dictionary<string, object>();

        public static void RegisterPerservable(IPreservable preserveable)
        {
            if (!_preservables.Contains(preserveable))
                _preservables.Add(preserveable);
        }

        private void Awake() => RegisterService(this);

        private void Start() => SetPreservedValue();


        private void SetPreservedValue()
        {
            if (_preservedValues.Count == 0) return;

            foreach (var preservable in _preservables)
            {
                preservable?.SetPreservedValue(_preservedValues);
            }
        }


        public void PreserveValue()
        {
            foreach (var preservable in _preservables)
            {
                var tuple = preservable?.PreserveValue();
                if (!tuple.HasValue) continue;
                RegisterValue(key: tuple.Value.Item1, value: tuple.Value.Item2);
            }
        }


        private void RegisterValue(string key, object value)
        {
            if (_preservedValues.ContainsKey(key))
                _preservedValues[key] = value;
            else
                _preservedValues.Add(key, value);
        }


        public void OnClick_ClearPreservedValue()
        {
            _preservables?.Clear();
            _preservedValues?.Clear();
        }
    }
}