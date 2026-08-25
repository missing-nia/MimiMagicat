using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Magicat.Addressable
{
    public class SoundLoader : MonoBehaviour
    {
        public static SoundLoader Instance { get { return _instance; } }

        public bool IsLoading { get { return _loadCounter > 0 ? true : false; } }

        private Dictionary<string, AsyncOperationHandle> _soundMap;

        // sounds we don't want to unload on scene change
        private List<string> _lockedSounds;

        // Use this to track if assets are currently loading
        private int _loadCounter = 0;

        private static SoundLoader _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                _instance = this;
            }

            _soundMap = new Dictionary<string, AsyncOperationHandle>();
            _lockedSounds = new List<string>();
        }

        public void LoadSound(string sound)
        {
            // Not a sound
            if (sound == "" || sound == null)
            {
                return;
            }

            if (_soundMap.ContainsKey(sound))
            {
                // Already loaded
                return;
            }

            ++_loadCounter;

            Addressables.LoadAssetAsync<AudioClip>(sound).Completed +=
                (asyncOperationHandle) =>
                {
                    if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        // We're loading these all at once so duplicate loads might be called
                        if (!_soundMap.TryAdd(sound, asyncOperationHandle))
                        {
                            Addressables.Release(asyncOperationHandle);
                        }
                    }
                    else
                    {
                        Debug.Log("Failed to load sound asset " + sound);
                    }

                    --_loadCounter;
                };
        }

        // Play a sound with a provided AudioSource
        public void RetrieveSound(string sound, AudioSource source)
        {
            // No sound to play
            if (sound == "" || sound == null)
            {
                source.clip = null;
                return;
            }

            // This should literally never happen unless the code is being misused!
            // MAKE SURE EVERY ASSET IS LOADED BEFORE WE START DRAWING THE SCENE!!!
            if (!_soundMap.ContainsKey(sound))
            {
                Debug.LogError(message: "ERROR: Addressable " + sound + " not found! This should never happen unless assets are being called independent of cutscene read!");
                return;
            }

            source.clip = (AudioClip)_soundMap[sound].Result;
        }

        public AudioClip RetrieveSoundClip(string sound)
        {
            // No sound to play
            if (sound == "" || sound == null)
            {
                return null;
            }

            // This should literally never happen unless the code is being misused!
            // MAKE SURE EVERY ASSET IS LOADED BEFORE WE START DRAWING THE SCENE!!!
            if (!_soundMap.ContainsKey(sound))
            {
                Debug.LogError(message: "ERROR: Addressable " + sound + " not found! This should never happen unless assets are being called independent of cutscene read!");
                return null;
            }

            return (AudioClip)_soundMap[sound].Result;
        }

        // Lock a sound so we don't unload it during scene change
        public void LockSound(string sound)
        {
            // Not a sound
            if (sound == "" || sound == null)
            {
                return;
            }

            if (!_soundMap.ContainsKey(sound))
            {
                Debug.LogError(message: "ERROR: Addressable " + sound + " not found! This should never happen unless assets are being called independent of cutscene read!");
                return;
            }

            _lockedSounds.Add(sound);
        }

        public void UnlockSound(string sound)
        {
            // Not a sound
            if (sound == "" || sound == null)
            {
                return;
            }

            if (!_soundMap.ContainsKey(sound))
            {
                Debug.LogError(message: "ERROR: Addressable " + sound + " not found! This should never happen unless assets are being called independent of cutscene read!");
                return;
            }

            _lockedSounds.Remove(sound);
        }

        // Grab a handle directly from the sound map
        public AsyncOperationHandle GetAsyncOperationHandle(string key)
        {
            return _soundMap[key];
        }


        // Call this when we load a new scene/cutscene (this will unload ALL loaded sounds)
        public void UnloadSounds()
        {
            List<AsyncOperationHandle> lockedHandleContainer = new List<AsyncOperationHandle>();
            foreach (string key in _lockedSounds)
            {
                lockedHandleContainer.Add(_soundMap[key]);
            }

            foreach (AsyncOperationHandle handle in _soundMap.Values)
            {
                if (lockedHandleContainer.Contains(handle))
                {
                    continue;
                }

                Addressables.Release(handle);
            }

            // Add back in the locked sounds
            _soundMap.Clear();
            for (int i = 0; i < _lockedSounds.Count; ++i)
            {
                // Sometimes we'll have multiple of the same SFX loaded so don't try to add the same thing twice
                if (!_soundMap.ContainsKey(_lockedSounds[i]))
                {
                    _soundMap.Add(_lockedSounds[i], lockedHandleContainer[i]);
                }
            }
            _lockedSounds.Clear();
        }
    }
}
