using System;
using Magicat.Helpers;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Magicat.JSON.BGMJSON
{
        /// <summary>
        /// Stores a bpm value and the associated timestamp for a given song.
        /// If a song has a set BPM, expected value would be BPM = [BPM], Timestamp = 0.0f
        /// </summary>
        public struct BPMData
    {
        public float BPM;
        public float TimestampInBeats; // Timestamp of the bpm 
    }

    /// <summary>
    /// Stores the time signature data for a song, including the quarter beats per measure and the timestamp of the change
    /// If a song has a set time signature of 4/4, expected values would be BeatsInAMeasure = 4, Timestamp = 0.0f
    /// </summary>
    public struct TimeSignatureData
    {
        public float BeatsInAMeasure; // Number of (quarter) beats in a measure
        public float TimestampInBeats; // Timestamp of the time signature change
    }

    public class BGMData
    {
        [JsonProperty("Name")]
        public string Name; // Addressable name/address

        [JsonProperty("LoopTimestampInBeats")]
        public float LoopTimestampInBeats; // Exact beat timestamp for loop point of song

        [JsonProperty("DurationInBeats")]
        public float DurationInBeats;

        [JsonProperty("BPM")]
        public BPMData[] BPM; // Stores BPM values and their associated timestamps. Important for songs with changing BPMs

        [JsonProperty("TimeSignature")]
        public TimeSignatureData[] TimeSignature; // Stores time signature values and their associated timestamps. Important for songs with varying time signatures.
    }
}