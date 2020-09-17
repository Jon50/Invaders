using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Invaders.Locator;

using static Invaders.Static.ConstantValues;
using static Invaders.Save.SavingSystem;

namespace Invaders.Managers
{
    public class AudioManager : ServiceRegister<AudioManager>
    {
        private AudioMixer _audioMixer;
        private List<SoundClass> _musicSounds;
        private List<SoundClass> _sfxSounds;

        private AudioSource _currentMusicPlaying;


        private void Awake() => RegisterService(this);

        public void Initialize(List<SoundClass> musicSounds, List<SoundClass> sfxSounds, AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;
            _musicSounds = musicSounds;
            _sfxSounds = sfxSounds;
        }


        private void Start() => StartCoroutine(DelaySoundStartup());

        private IEnumerator DelaySoundStartup()
        {
            yield return new WaitForSeconds(.5f);
            UpdateAudioSettings();
        }


        public AudioSource PlayMusic(string musicName)
        {
            if (_musicSounds.IsNull() || string.IsNullOrEmpty(musicName)) return null;


            foreach (var music in _musicSounds)
            {
                if (music.SoundName == musicName && !music.audioSource.isPlaying)
                {
                    _currentMusicPlaying?.Stop();
                    _currentMusicPlaying = music.audioSource;
                    music.audioSource.Play();
                }
            }

            return _currentMusicPlaying;
        }


        public void PlaySFX(string sfxName)
        {
            if (_sfxSounds.IsNull() || string.IsNullOrEmpty(sfxName)) return;

            foreach (var sfx in _sfxSounds)
            {
                if (sfx.SoundName == sfxName)
                    sfx.audioSource.PlayOneShot(sfx.AudioClip);
            }
        }


        public void UpdateAudioSettings()
        {
            _audioMixer.SetFloat(MASTER_VOLUME, LoadValue<float>(MASTER_SAVE_PATH, defaultValue: 1f).LinearToDecibel());
            _audioMixer.SetFloat(MUSIC_VOLUME, LoadValue<float>(MUSIC_SAVE_PATH, defaultValue: 1f).LinearToDecibel());
            _audioMixer.SetFloat(SFX_VOLUME, LoadValue<float>(SFX_SAVE_PATH, defaultValue: 1f).LinearToDecibel());
        }
    }
}