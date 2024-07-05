using System.Collections.Generic;

using UnityEngine;

namespace MP.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioCategorizer : MonoBehaviour, IVolumeListener
    {
        // Components
        private AudioSource _audioSource;
        public AudioSource AudioSource => _audioSource;

        // Misc
        public LinkedListNode<IVolumeListener> RegistryNode { get; set; }
        private float _unscaledVolume;
        public float UnscaledVolume
        {
            get => _unscaledVolume;
            set => SetUnscaledVolume(value);
        }

        [SerializeField]
        private AudioCategory category;
        public AudioCategory Category
        {
            get => category;
            set => SetCategory(value);
        }

        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _unscaledVolume = _audioSource.volume;
        }

        private void UpdateVolume()
        {
            // Ignore the global category because it is already governed by the audio listener volume
            if(category == AudioCategory.Global)
            {
                AudioSource.volume = _unscaledVolume;
                return;
            }
            
            AudioSource.volume = Mathf.Clamp01(_unscaledVolume * AudioManager.Instance.GetVolume(category));
        }

        protected virtual void SetCategory(AudioCategory newCategory)
        {
            category = newCategory;
            UpdateVolume();
        }
        
        private void SetUnscaledVolume(float newUnscaledVolume)
        {
            _unscaledVolume = newUnscaledVolume;
            UpdateVolume();
        }

        #region Events

        protected virtual void OnEnable()
        {
            // Subscribe to the audio manager as a volume listener
            AudioManager.Instance.AddVolumeListener(this);

            // Set the initial volume
            UpdateVolume();
        }

        protected virtual void OnDisable()
        {   
            // Unsubscribe from the audio manager
            AudioManager.Instance.RemoveVolumeListener(this);
        }
        
        public void OnVolumeChanged(AudioCategory audioCategory, float oldVolume, float newVolume)
        {
            if(audioCategory != category)
            {
                return;
            }

            AudioSource.volume = newVolume;
        }

        #endregion
    }
}