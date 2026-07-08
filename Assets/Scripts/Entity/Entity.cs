using System;
using UnityEngine;

namespace Magicat.Entity
{
    /// <summary>
    /// Base class for any health-managed objects in the game world, ranging from the player to enemies
    /// to breakable objects in the scene. This may manage other shared features in the future but for now
    /// facilitates a shared OnKilled function called when health reaches zero.
    /// </summary>
    public class Entity : MonoBehaviour
    {
        // Commenting out Event wrappers for now! Come back to this later

        // Called when this entity kills something
        /*public EventWrapper OnEntityKilled;
        public EventWrapper OnDamageTakenEvent; // Idk what to call this
        public EventWrapper OnCriticalStrike;

        // Called when entity deals damage to another entity
        public EventWrapperWithMessage<Entity> OnDamageDealt;*/

        [SerializeField]
        protected float _updateTime = 0.33f;

        protected bool _alive = true;

        protected virtual void Awake()
        {
            /*OnEntityKilled = new EventWrapper();
            OnDamageTakenEvent = new EventWrapper();
            OnCriticalStrike = new EventWrapper();
            OnDamageDealt = new EventWrapperWithMessage<Entity>();*/
        }

        public virtual void OnKilled()
        {
            _alive = false;
        }

        public virtual void OnDamageTaken(Entity attacker)
        {
            // OnDamageTakenEvent.Invoke();
        }

        /// <summary>
        /// Called when the entity kills another entity
        /// </summary>
        /// <param name="target"></param>
        public virtual void OnKill(Entity target)
        {
            Type type = target.GetType();
           /* if (type == typeof(Breakable))
            {
                // We don't care abt this for event logic
                return;
            }

            OnEntityKilled.Invoke();*/
        }
    }
}
