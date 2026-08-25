using Magicat.Addressable;
using System;
using UnityEngine;
using Magicat.JSON;
using Magicat.JSON.BGMJSON;

namespace Magicat.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public const int CHANNEL_COUNT = 4;

        [SerializeField]
        private AudioSource[] _channels = new AudioSource[CHANNEL_COUNT];

        public static SoundManager Instance { get { return _instance; } }
        private static SoundManager _instance;

        // TODO: Look into DR audio mixer thing i did idr this lol
        //[SerializeField]
        //private AudioMixer _audioMixer;

        // Volume fields
        [SerializeField]
        private int _masterVolume = 100;

        //Music Testing
        [SerializeField]
        private TextAsset _bgmJSON;

        private float _bgmTime; // Current time of the running bgm for audio clip swapping
        private BGMData _bgmData;

        private bool _isBGMLoading; // Testing should handled this some other way maybe

        // Start is called before the first frame update
        private void Start()
        {
            _bgmData = JSONReader.ReadBGMJSON(_bgmJSON);
            SoundLoader.Instance.LoadSound(_bgmData.Name + ".ch1");
            SoundLoader.Instance.LoadSound(_bgmData.Name + ".ch2");
            SoundLoader.Instance.LoadSound(_bgmData.Name + ".ch3");
            SoundLoader.Instance.LoadSound(_bgmData.Name + ".ch4");
            _isBGMLoading = true;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                // Cant set this until we separate the marriage of MainCamera and SoundManager :(
                //DontDestroyOnLoad(gameObject);
                _instance = this;
            }
        }

        private void Update()
        {
            UpdateVolume();
            if (_isBGMLoading && SoundLoader.Instance.IsLoading == false)
            {
                PlayMusic();
                _isBGMLoading = false;
            }
        }

        private void FixedUpdate()
        {
            _bgmTime += Time.fixedDeltaTime;

            // TODO: looping
        }

        private void UpdateVolume()
        {
            // Logarithmic math mumbo jumbo so we can change volume without actually changing values the vizualizer sees
            //_audioMixer.SetFloat("Master", Mathf.Log10(_masterVolume / 100.0f) * 20.0f);
        }

        public void PlaySoundEffect(string sound, float delay)
        {
            // TODO
            // CONCEPT: stop audio channel based on the provided channel, resume audio after (music)
        }

        public void PlayMusic()
        {
            for (int i = 1; i <= CHANNEL_COUNT; ++i)
            {
                SoundLoader.Instance.RetrieveSound(_bgmData.Name + ".ch" + i, _channels[i - 1]);
                if (_channels[i - 1].clip != null)
                {
                    _channels[i - 1].Play();
                }
            }

            _bgmTime = 0.0f; // TODO: check if this lines up right xd
        }

        public void StopMusic()
        {
            // TODO
        }

        public void SetVolume(SoundType soundType, int volume)
        {
            switch (soundType)
            {
                case SoundType.Master:
                    _masterVolume = Math.Min(volume, 100);
                    break;
            }
        }

        public int GetVolume(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Master:
                    return _masterVolume;
            }
            return 0;
        }

        public void LockActiveSounds()
        {
            // TODO 
            // Not sure if want to keep
        }

        public enum SoundType
        {
            Master = 0
        }
    }
}
