using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magicat.Helpers
{
    /// <summary>
    /// Helper class for collider functions
    /// </summary>
    public static class Collider2DHelper
    {
        /// <summary>
        /// Get Scaled position of the collider accounting for offset
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public static Vector2 GetScaledOffsetPosition(Collider2D collider)
        {
            Vector2 scaleVector = (Vector2)collider.transform.lossyScale;
            if (scaleVector != Vector2.one)
            {
                // Non-standard scale do more math xdd
                return (Vector2)collider.transform.position + collider.offset * scaleVector;
            }
            return (Vector2)collider.transform.position + collider.offset;
        }

        /// <summary>
        /// Gets the scale offset of the collider not accounting for world space position
        /// </summary>
        /// <param name="collider"></param>
        /// <returns></returns>
        public static Vector2 GetScaledOffset(Collider2D collider)
        {
            Vector2 scaleVector = (Vector2)collider.transform.lossyScale;
            if (scaleVector != Vector2.one)
            {
                // Non-standard scale do more math xdd
                return collider.offset * scaleVector;
            }
            return collider.offset;
        }

        /// <summary>
        /// Get Scaled radius of the collider
        /// </summary>
        public static float GetScaledRadius(CircleCollider2D collider)
        {
            Vector2 scaleVector = (Vector2)collider.transform.lossyScale;
            if (scaleVector != Vector2.one)
            {
                // Non-standard scale do more math xdd
                float scaleMultiplier = Mathf.Max(Mathf.Abs(scaleVector.x), Mathf.Abs(scaleVector.y));
                return collider.radius * scaleMultiplier;
            }
            return collider.radius;
        }
    }
}
