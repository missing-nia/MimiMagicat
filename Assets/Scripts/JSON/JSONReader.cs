using Newtonsoft.Json;
using UnityEngine;
using Magicat.JSON.BGMJSON;

namespace Magicat.JSON
{
    /// <summary>
    /// Class with helper functions for parsing various JSON files needed for gameplay function
    /// </summary>
    public static class JSONReader
    {
        /// <summary>
        /// Parses a BGM JSON file into a BGMData structure
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static BGMData ReadBGMJSON(TextAsset file)
        {
            BGMData collection = JsonConvert.DeserializeObject<BGMData>(file.text);
            return collection;
        }
    }
}
