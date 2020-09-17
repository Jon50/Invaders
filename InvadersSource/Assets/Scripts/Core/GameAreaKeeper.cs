using Invaders.Locator;
using UnityEngine;

namespace Invaders.Core
{
    public class GameAreaKeeper : MonoBehaviour
    {
        private GameArea _gameArea = default;
        [SerializeField] private float _xOffset = 0;
        [SerializeField] private float _yOffset = 0;

        private Transform _thisTransform = default;

        public float xMin { get; private set; }
        public float xMax { get; private set; }
        public float yMin { get; private set; }
        public float yMax { get; private set; }

        private void Start()
        {
            _gameArea = ServiceLocator.Resolve<GameArea>();
            _thisTransform = transform;

            xMin = _gameArea.xMin - _xOffset;
            yMin = _gameArea.yMin - _yOffset;

            xMax = _gameArea.xMax + _xOffset;
            yMax = _gameArea.yMax + _yOffset;
        }

        private void LateUpdate()
        {
            var xClamp = Mathf.Clamp(_thisTransform.position.x, xMin, xMax);
            var yClamp = Mathf.Clamp(_thisTransform.position.y, yMin, yMax);

            _thisTransform.position = new Vector2(xClamp, yClamp);
        }
    }
}