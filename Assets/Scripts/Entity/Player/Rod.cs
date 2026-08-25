using Magicat.Helpers;
using System.Collections;
using UnityEngine;

namespace Magicat.Entity.Player
{
    public class Rod : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How long the collision box should remain active when searching for entities to copy")]
        private float _activationTime;

        private BoxCollider2D _collisionBox;

        private void Start()
        {
            _collisionBox = GetComponent<BoxCollider2D>();
        }
        private IEnumerator CollisionActivationRoutine()
        {
            yield return new WaitForSeconds(_activationTime);
            _collisionBox.transform.localPosition = Vector3.zero;
            _collisionBox.enabled = false;
        }

        /// <summary>
        /// Activates the rod's copy ability, which checks for 
        /// nearby valid entities in a collision box to copy
        /// </summary>
        /// <param name="dir"></param>
        public void UseCopyAbility(Directions dir)
        {
            Vector2 pos = Vector2.zero;

            switch (dir)
            {
                case Directions.North:
                    pos.y += 1.0f;
                    break;
                case Directions.South:
                    pos.y -= 1.0f;
                    break;
                case Directions.West:
                    pos.x -= 1.0f;
                    break;
                 case Directions.East:
                    pos.x += 1.0f;
                    break;
            }

            _collisionBox.transform.localPosition = pos;
            _collisionBox.enabled = true;
            StartCoroutine(CollisionActivationRoutine());
        }
    }
}