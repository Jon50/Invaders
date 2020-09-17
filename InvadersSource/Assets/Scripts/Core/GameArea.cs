using UnityEngine;
using Invaders.Locator;

namespace Invaders.Core
{
    public class GameArea : ServiceRegister<GameArea>
    {
        [SerializeField] private Vector2 _gameAreaSize = default;
        [SerializeField] private float _limitLoseOffset = default;

        private Rect _rectArea = default;

        public float LimitLose => yMin + _limitLoseOffset;
        public float xMin { get; private set; }
        public float xMax { get; private set; }
        public float yMin { get; private set; }
        public float yMax { get; private set; }

        public Vector2 GameAreaSize
        {
            get => _gameAreaSize;
            set => _gameAreaSize = value;
        }


        private void Awake()
        {
            _rectArea = new Rect(_gameAreaSize.x * -0.5f, _gameAreaSize.y * -0.5f, _gameAreaSize.x, _gameAreaSize.y);

            var mins = new Vector2(_rectArea.xMin, _rectArea.yMin);
            var maxs = new Vector2(_rectArea.xMax, _rectArea.yMax);

            xMin = transform.TransformPoint(mins).x;
            yMin = transform.TransformPoint(mins).y;

            xMax = transform.TransformPoint(maxs).x;
            yMax = transform.TransformPoint(maxs).y;

            RegisterService(this);
        }


        #region GIZMOS
#if UNITY_EDITOR
        public bool showGameArea = true;
        public bool fitToScreen = false;

        private void OnDrawGizmos()
        {
            if (showGameArea)
            {
                _rectArea = new Rect(_gameAreaSize.x * -0.5f, _gameAreaSize.y * -0.5f, _gameAreaSize.x, _gameAreaSize.y);

                Gizmos.matrix = transform.localToWorldMatrix;

                Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
                Gizmos.DrawCube(Vector3.zero, new Vector3(_gameAreaSize.x, _gameAreaSize.y, 0));

                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(_gameAreaSize.x, _gameAreaSize.y, 0));

                Gizmos.color = Color.magenta;
                var pointA = new Vector3(_rectArea.xMin, _rectArea.yMin + _limitLoseOffset);
                var pointB = new Vector3(_rectArea.xMax, _rectArea.yMin + _limitLoseOffset);
                Gizmos.DrawLine(pointA, pointB);
            }
        }

#endif
        #endregion
    }
}