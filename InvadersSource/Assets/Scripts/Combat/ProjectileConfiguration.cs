using UnityEngine;

namespace Invaders.Combat
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Projectile/New Projectile", order = 0)]
    public class ProjectileConfiguration : ScriptableObject
    {
        public float velocity;
        public Vector2 direction;
        [Range(0.1f, 0.2f)] public float explosionRadius;
        public Color color;
    }
}