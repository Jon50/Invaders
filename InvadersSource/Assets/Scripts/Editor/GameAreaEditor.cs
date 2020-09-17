#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Invaders.Core
{
    [CustomEditor(typeof(GameArea)), CanEditMultipleObjects]
    public class GameAreaGUI : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GameArea gameArea = (GameArea)target;

            if (gameArea.fitToScreen)
            {
                if (GUILayout.Button("Fit"))
                {
                    var canvas = gameArea.gameObject.TryGetComponent<Canvas>(out Canvas cv) ? cv : gameArea.gameObject.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = Camera.main;
                    gameArea.transform.position = Vector3.zero;
                    gameArea.GameAreaSize = gameArea.GetComponent<RectTransform>().sizeDelta;
                }
            }
            else if (gameArea.TryGetComponent<Canvas>(out Canvas canvas))
            {
                DestroyImmediate(canvas);
            }
        }
    }
}
#endif