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
        public const int SECONDS_PER_MINUTE = 60; // kinda stupid but also clarifies what the number is lol

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
        private float _bgmDuration;
        private float _bgmLoopTimestamp;

        private BGMData _bgmData;

        private bool _isBGMLoading; // Testing should handled this some other way maybe
        private bool _isBGMPlaying;

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

            _bgmTime += Time.deltaTime;
            if (_isBGMPlaying && _bgmTime > _bgmDuration)
            {
                // Time to loop
                foreach (var channel in _channels)
                {
                    channel.time = _bgmLoopTimestamp;
                }
                _bgmTime = _bgmLoopTimestamp;
            }
        }

        /// <summary>
        /// Function for calculating the realtime timestamp for
        /// song loops. Data is processed using BGM and beat count data
        /// </summary>
        private void CalculateLoopTimestamp()
        {
            _bgmLoopTimestamp = 0.0f;
            // TODO: maybe consider doing calculations elsewhere for use in the other beat logic for AI
            for(int i = 0; i < _bgmData.BPM.Length; ++i)
            {
                // 60 / BPM = number of seconds in each beat
                float secondsPerBeat = SECONDS_PER_MINUTE / _bgmData.BPM[i].BPM;

                // Special case if we're at the last bpm change
                if (i + 1 == _bgmData.BPM.Length) 
                {
                    _bgmLoopTimestamp += secondsPerBeat * (_bgmData.LoopTimestampInBeats - _bgmData.BPM[i].TimestampInBeats);
                }
                else if (_bgmData.LoopTimestampInBeats < _bgmData.BPM[i + 1].TimestampInBeats)
                {
                    _bgmLoopTimestamp += secondsPerBeat * (_bgmData.LoopTimestampInBeats - _bgmData.BPM[i].TimestampInBeats);
                }
                else
                {
                    // Count to next timestamp
                    _bgmLoopTimestamp += secondsPerBeat * (_bgmData.BPM[i + 1].TimestampInBeats - _bgmData.BPM[i].TimestampInBeats);
                }
            }
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
            // Failsafe in case no audio clips
            _bgmDuration = 0.0f;

            for (int i = 1; i <= CHANNEL_COUNT; ++i)
            {
                SoundLoader.Instance.RetrieveSound(_bgmData.Name + ".ch" + i, _channels[i - 1]);
                if (_channels[i - 1].clip != null)
                {
                    _channels[i - 1].Play();
                    _channels[i - 1].loop = true;
                    _bgmDuration = _channels[i - 1].clip.length; 
                }
            }

            CalculateLoopTimestamp();
            _bgmTime = 0.0f; // TODO: check if this lines up right xd
            _isBGMPlaying = true;
        }

        public void StopMusic()
        {
            // TODO
            _isBGMPlaying = false;
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
