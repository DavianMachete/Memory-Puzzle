using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

using MP.Core;
using Random = UnityEngine.Random;

namespace MP.Audio
{
	public class AudioManager : Manager<AudioManager>
	{
		// Misc
		[SerializeField] private AudioSource fxAudioSource;
		[SerializeField] private AudioSource musicAudioSource;

		[Header("Fx sounds")]
		[SerializeField] private AudioClip cardFlip;
		[SerializeField] private AudioClip cardMatchCorrect;
		[SerializeField] private AudioClip cardMatchIncorrect;
		[SerializeField] private AudioClip levelWin;
		
		[Space]
		[SerializeField] private AudioClip[] backgroundMusics;
		
	    public bool IsSoundEnabled
		{
			get => GlobalVolume > 0f;
			set => GlobalVolume = value ? 1f : 0f;
		}

        // Volumes
	    private AudioCategorySettings[] _categorySettings;
	    private Dictionary<string, AudioCategorySettings> _categorySettingsByName;

	    public float GlobalVolume
	    {
	        get => GetVolume(AudioCategory.Global);
	        set => SetVolume(AudioCategory.Global, value);
	    }

	    public float FxVolume
	    {
	        get => GetVolume(AudioCategory.Fx);
	        set => SetVolume(AudioCategory.Fx, value);
	    }

	    public float MusicVolume
	    {
	        get => GetVolume(AudioCategory.Music);
	        set => SetVolume(AudioCategory.Music, value);
	    }
	    
	    // Volume listeners
	    private readonly LinkedList<IVolumeListener> _volumeListeners = new();
	    private readonly Stack<LinkedListNode<IVolumeListener>> _volumeListenerNodePool = new();

        // Events
	    public delegate void VolumeChangeHandler(AudioCategory category, float prevVolume, float newVolume);
	    public event VolumeChangeHandler OnVolumeChanged;

	    #region Manager related
	    
	    // ReSharper disable Unity.PerformanceAnalysis
	    protected override void OnAwakeManager()
		{
			LoadCategories();
			PlayBackgroundMusic();
		}

	    protected override bool HasInitialization() => false;

	    #endregion
		
	    protected AudioCategorySettings CreateSettings(AudioCategory category)
        {
            switch(category)
            {
                case AudioCategory.Global:
                    return new GlobalCategorySettings();
                default:
                    return new AudioCategorySettings(category);
            }
        }

        #region Audio categories
	    
        [ContextMenu("Load audio categories")]
	    protected virtual void LoadCategories()
	    {
	        var categories = Enum.GetValues(typeof(AudioCategory));

            // Create settings collections
            _categorySettings = new AudioCategorySettings[categories.Length];
            _categorySettingsByName = new Dictionary<string, AudioCategorySettings>();

            // Load categories
	        for(var i = 0; i < categories.Length; i++)
	        {
                var category = (AudioCategory)categories.GetValue(i);

                // Create category settings
	            var settings = CreateSettings(category);
                AddCategorySettings(settings);

                // Load the settings
                settings.Load();
	        }
	    }
	    
	    protected AudioCategorySettings GetCategorySettings(int index)
	    {
		    if(index < 0 || index > Instance._categorySettings.Length)
		    {
			    return null;
		    }

		    return Instance._categorySettings[index];
	    }

	    public AudioCategorySettings GetCategorySettings(AudioCategory category)
	    {
		    return GetCategorySettings((int)category);
	    }

	    protected AudioCategorySettings GetCategorySettings(string categoryName)
	    {
		    return _categorySettingsByName.TryGetValue(categoryName, out var settings) ? settings : null;
	    }

	    protected void AddCategorySettings(AudioCategorySettings settings)
	    {
		    var index = (int)settings.Category;

		    // Expand settings array if required
		    if(index > _categorySettings.Length - 1)
		    {
			    var newSize = Mathf.Max(index + 1, _categorySettings.Length * 2);
			    var newArray = new AudioCategorySettings[newSize];
			    Array.Copy(_categorySettings, 0, newArray, 0, _categorySettings.Length);
			    _categorySettings = newArray;
		    }

		    // Set the category settings
		    _categorySettings[index] = settings;
		    _categorySettingsByName[settings.Name] = settings;

		    // Bind volume change event
		    settings.OnVolumeChanged += OnCategoryVolumeChanged;
	    }
	    
	    #endregion
	    
	    #region Volumes
	    
	    public float GetVolume(AudioCategory category)
	    {
		    var settings = Instance.GetCategorySettings(category);
		    return settings?.Volume ?? 0f;
	    }

	    public void SetVolume(AudioCategory category, float newVolume)
	    {
		    var settings = Instance.GetCategorySettings(category);
		    if(settings == null)
		    {
			    return;
		    }

		    settings.Volume = newVolume;
			settings.Save();
		}

	    public void SetDefaultVolume(AudioCategory category, float defaultVolume)
	    {
		    var settings = Instance.GetCategorySettings(category);
		    settings.DefaultVolume = defaultVolume;
	    }

	    public void ReApplyVolumes()
	    {
		    var categorySettings = Instance._categorySettings;
		    foreach (var setting in categorySettings)
		    {
			    setting.ReApplyVolume();
		    }
	    }

	    private void UpdateVolumeListeners(AudioCategory category, float prevVolume, float newVolume)
	    {
		    var node = _volumeListeners.First;
		    while(node != null)
		    {
			    var volumeListener = node.Value;
			    node = node.Next;

			    if(volumeListener == null)
			    {
				    Debug.LogWarning("Encountered a null volume listener.");
				    continue;
			    }

			    try
			    {
				    volumeListener.OnVolumeChanged(category, prevVolume, newVolume);
			    }
			    catch(Exception exception)
			    {
				    Debug.LogException(exception);
			    }
		    }
	    }

	    public void AddVolumeListener(IVolumeListener volumeListener)
	    {
		    var node = volumeListener.RegistryNode;
		    if(node != null)
		    {
			    return;
		    }

		    node = GetFreeVolumeListenerNode(volumeListener);
		    volumeListener.RegistryNode = node;
		    _volumeListeners.AddLast(node);
	    }

	    public void RemoveVolumeListener(IVolumeListener volumeListener)
	    {
		    var node = volumeListener.RegistryNode;
		    if(node == null)
		    {
			    return;
		    }

		    // Remove the volume listener
		    _volumeListeners.Remove(node);
		    volumeListener.RegistryNode = null;

		    // Recycle the node
		    node.Value = null;
		    _volumeListenerNodePool.Push(node);
	    }

	    private LinkedListNode<IVolumeListener> GetFreeVolumeListenerNode(IVolumeListener volumeListener)
	    {
		    LinkedListNode<IVolumeListener> node;
		    if(_volumeListenerNodePool.Count > 0)
		    {
			    node = _volumeListenerNodePool.Pop();
			    node.Value = volumeListener;
		    }
		    else
		    {
			    node = new LinkedListNode<IVolumeListener>(volumeListener);
		    }

		    return node;
	    }

	    #endregion

	    #region Clip playing

	    public void PlayCardFlip()
	    {
		    PlayFx(cardFlip);
	    }
	    
	    public void PlayCardsMatchCorrect()
	    {
		    PlayFx(cardMatchCorrect);
	    }
	    
	    public void PlayCardsMatchIncorrect()
	    {
		    PlayFx(cardMatchIncorrect);
	    }

	    public void PlayLevelWin()
	    {
		    PlayFx(levelWin);
	    }

	    private void PlayBackgroundMusic()
	    {
		    var rnd = Random.Range(0, backgroundMusics.Length);
		    var rndMusic = backgroundMusics[rnd];
		    var length = rndMusic.length;
		    PlayMusic(rndMusic, false);
		    StartCoroutine(DelayedPlayBackgroundMusic(length));
	    }

	    // ReSharper disable Unity.PerformanceAnalysis
	    private IEnumerator DelayedPlayBackgroundMusic(float time)
	    {
		    yield return new WaitForSeconds(time);
		    PlayBackgroundMusic();
	    }
	    
        private AudioSource PlayMusic(AudioClip clip, bool loop)
        {
	        if(clip == null)
	        {
		        return null;
	        }

            // Configure the audio source
            var audioSource = Instance.musicAudioSource;
            audioSource.pitch = 1f;
            audioSource.spatialBlend = 0f;
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.Play();

            return audioSource;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
	    private void PlayFx(AudioClip clip, float volumeScale = 1f)
        {
	        if(clip == null)
	        {
		        return;
	        }
	        
            Instance.fxAudioSource.PlayOneShot(clip, volumeScale);
        }
        
        #endregion
	    
	    #region Events

	    private void OnCategoryVolumeChanged(AudioCategorySettings settings, float prevVolume, float newVolume)
	    {
		    // Update our volume listeners
		    var audioCategory = settings.Category;
		    UpdateVolumeListeners(audioCategory, prevVolume, newVolume);
		    
		    // Fire volume changed event
		    OnVolumeChanged?.Invoke(audioCategory, prevVolume, newVolume);
	    }

		#endregion
    }
}