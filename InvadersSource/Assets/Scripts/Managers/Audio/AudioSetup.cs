using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Invaders.Managers
{
    public class AudioSetup : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private List<SoundClass> _musicSounds = new List<SoundClass>();
        [SerializeField] private List<SoundClass> _sfxSounds = new List<SoundClass>();


        private void Awake()
        {
            foreach (var music in _musicSounds)
            {
                music.audioSource = gameObject.AddComponent<AudioSource>();
                var refAudioSource = music.audioSource;

                refAudioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("Music")[0];

                refAudioSource.clip = music.AudioClip;
                refAudioSource.volume = music.Volume;
                refAudioSource.pitch = music.Pitch;
                refAudioSource.loop = music.Loop;
                refAudioSource.playOnAwake = music.PlayOnAwake;
            }

            foreach (var sfx in _sfxSounds)
            {
                sfx.audioSource = gameObject.AddComponent<AudioSource>();
                var refAudioSource = sfx.audioSource;

                refAudioSource.outputAudioMixerGroup = _audioMixer.FindMatchingGroups("SFX")[0];

                refAudioSource.clip = sfx.AudioClip;
                refAudioSource.volume = sfx.Volume;
                refAudioSource.pitch = sfx.Pitch;
                refAudioSource.loop = sfx.Loop;
                refAudioSource.playOnAwake = sfx.PlayOnAwake;
            }

            var audioManager = GetComponent<AudioManager>();
            audioManager.Initialize(_musicSounds, _sfxSounds, _audioMixer);
        }
    }
}