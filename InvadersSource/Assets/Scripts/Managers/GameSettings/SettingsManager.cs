using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using static Invaders.Static.ConstantValues;
using static Invaders.Save.SavingSystem;

namespace Invaders.Managers
{
    public class SettingsManager : MonoBehaviour
    {
        [Header("Sound")]
        [SerializeField] private Slider _masterVolume;
        [SerializeField] private Slider _musicVolume;
        [SerializeField] private Slider _sfxVolume;

        [Header("Visuals")]
        [SerializeField] private Toggle _chromatic;
        [SerializeField] private Toggle _distortion;
        [SerializeField] private Toggle _bloom;
        [SerializeField] private Toggle _vignette;

        [Space(30)]
        public UnityEvent OnSoundSettingsChanged;
        public UnityEvent OnVisualSettingsChanged;


        private void Start() => LoadOptionSettings();


        private void OnEnable()
        {
            _masterVolume.onValueChanged.AddListener(MasterSound);
            _musicVolume.onValueChanged.AddListener(MusicSound);
            _sfxVolume.onValueChanged.AddListener(SfxSound);

            _chromatic.onValueChanged.AddListener(ChromaticVisual);
            _distortion.onValueChanged.AddListener(DistortionVisual);
            _bloom.onValueChanged.AddListener(BloomVisual);
            _vignette.onValueChanged.AddListener(VignetteVisual);
        }

        private void OnDisable()
        {
            _masterVolume.onValueChanged.RemoveListener(MasterSound);
            _musicVolume.onValueChanged.RemoveListener(MusicSound);
            _sfxVolume.onValueChanged.RemoveListener(SfxSound);

            _chromatic.onValueChanged.RemoveListener(ChromaticVisual);
            _distortion.onValueChanged.RemoveListener(DistortionVisual);
            _bloom.onValueChanged.RemoveListener(BloomVisual);
            _vignette.onValueChanged.RemoveListener(VignetteVisual);
        }


        private void LoadOptionSettings()
        {
            _masterVolume.value = LoadValue<float>(MASTER_SAVE_PATH, defaultValue: 0.5f);
            _musicVolume.value = LoadValue<float>(MUSIC_SAVE_PATH, defaultValue: 0.5f);
            _sfxVolume.value = LoadValue<float>(SFX_SAVE_PATH, defaultValue: 0.5f);

            _chromatic.isOn = LoadValue<bool>(CHROMATIC_SAVE_PATH, defaultValue: true);
            _distortion.isOn = LoadValue<bool>(DISTORTION_SAVE_PATH, defaultValue: true);
            _bloom.isOn = LoadValue<bool>(BLOOM_SAVE_PATH, defaultValue: true);
            _vignette.isOn = LoadValue<bool>(VIGNETTE_SAVE_PATH, defaultValue: true);
        }


        //Sound
        private void MasterSound(float value) => SoundSettingsChanged(MASTER_SAVE_PATH, value);
        private void MusicSound(float value) => SoundSettingsChanged(MUSIC_SAVE_PATH, value);
        private void SfxSound(float value) => SoundSettingsChanged(SFX_SAVE_PATH, value);
        private void SoundSettingsChanged(string savePath, float value)
        {
            SaveValue(savePath, value);
            OnSoundSettingsChanged?.Invoke();
        }


        //Visuals
        private void ChromaticVisual(bool value) => VisualSettingsChanged(CHROMATIC_SAVE_PATH, value);
        private void DistortionVisual(bool value) => VisualSettingsChanged(DISTORTION_SAVE_PATH, value);
        private void BloomVisual(bool value) => VisualSettingsChanged(BLOOM_SAVE_PATH, value);
        private void VignetteVisual(bool value) => VisualSettingsChanged(VIGNETTE_SAVE_PATH, value);

        private void VisualSettingsChanged(string savePath, bool value)
        {
            SaveValue(savePath, value);
            OnVisualSettingsChanged?.Invoke();
        }
    }
}