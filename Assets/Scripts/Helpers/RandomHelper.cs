using UnityEngine;

namespace Magicat.Helpers
{
    public static class RandomHelper
    {
        // <summary>
        // Get a random index based on a list of weighted values passed in
        // weights == 1.0f
        // </summary>
        public static int GetRandomWeightedIndex(float[] weights)
        {
            if (weights == null || weights.Length == 0) return -1;

            float w = 0.0f;
            float t = 0.0f;
            int i;
            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsPositiveInfinity(w)) 
                    return i;
                else if (w >= 0.0f && !float.IsNaN(w)) 
                    t += weights[i];
            }

            float r = Random.value;
            float s = 0f;

            for (i = 0; i < weights.Length; i++)
            {
                w = weights[i];
                if (float.IsNaN(w) || w <= 0.0f)
                    continue;

                s += w / t;
                if (s >= r) 
                    return i;
            }

            return -1;
        }
    }
}
