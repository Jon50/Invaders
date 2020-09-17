using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

using static Invaders.Save.SavingSystem;
using static Invaders.Static.ConstantValues;

namespace Invaders.Managers
{
    public class PostProcessingManager : MonoBehaviour
    {
        [SerializeField] private PostProcessVolume _postProcess;

        private ChromaticAberration _chromatic;
        private LensDistortion _distortion;
        private Bloom _bloom;
        private Vignette _vignette;


        private void Awake()
        {
            UpdateVisualSettings();
        }


        public void UpdateVisualSettings()
        {
            _postProcess.profile.TryGetSettings(out _chromatic);
            _postProcess.profile.TryGetSettings(out _distortion);
            _postProcess.profile.TryGetSettings(out _bloom);
            _postProcess.profile.TryGetSettings(out _vignette);

            _chromatic.enabled.value = LoadValue<bool>(CHROMATIC_SAVE_PATH, defaultValue: true);
            _distortion.enabled.value = LoadValue<bool>(DISTORTION_SAVE_PATH, defaultValue: true);
            _bloom.enabled.value = LoadValue<bool>(BLOOM_SAVE_PATH, defaultValue: true);
            _vignette.enabled.value = LoadValue<bool>(VIGNETTE_SAVE_PATH, defaultValue: true);
        }
    }
}