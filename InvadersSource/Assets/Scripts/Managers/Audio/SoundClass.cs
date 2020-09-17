using UnityEngine;

namespace Invaders.Managers
{
    [CreateAssetMenu(menuName = "New Sound")]
    public class SoundClass : ScriptableObject
    {
        [SerializeField] private string _soundName;
        [SerializeField] private AudioClip _audioClip;
        [Range(0, 1)] [SerializeField] private float _volume;
        [Range(0, 1)] [SerializeField] private float _pitch;
        [SerializeField] private bool _loop;
        [SerializeField] private bool _playOnAwake;

        [HideInInspector] public AudioSource audioSource;

        public string SoundName => _soundName;
        public AudioClip AudioClip => _audioClip;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public bool Loop => _loop;
        public bool PlayOnAwake => _playOnAwake;
    }
}