using UnityEngine;

namespace Invaders.Attributes
{
    public class AlienAnimation : MonoBehaviour
    {
        [SerializeField] private Sprite[] _animationSprites;

        private int _index = 0;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void AnimateAlien()
        {
            _index = (_index == 0 ? 1 : 0);
            _spriteRenderer.sprite = _animationSprites[_index];
        }
    }
}