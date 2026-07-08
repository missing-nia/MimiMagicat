using UnityEngine;

namespace Magicat.Entity
{
    /// <summary>
    /// Monobehaviour for handling Kinematic Rigidbody2D movements on an object
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class KinematicMovement : MonoBehaviour
    {
        public ContactFilter2D Filter { get { return _filter; } }
        public LayerMask ExcludeMask { get { return _excludeMask; } }

        /// <summary>
        /// Boolean for toggling knockback behavior. True by default
        /// </summary>
        public bool Knockback = true;

        private RaycastHit2D[] _hits;

        private Rigidbody2D _rb;

        // Require a CircleCollider2D for the time being
        // TODO: in the future we may want moving objects with box colliders
        // So we may need a way to specify either or :v
        [SerializeField]
        private CircleCollider2D _collider;

        private ContactFilter2D _filter;

        private LayerMask _excludeMask = 0;

        // Velocity vectors used every fixed update tick
        private Vector2 _velocity = Vector2.zero;
        private Vector2 _velocityFixedDeltaTime = Vector2.zero; 
        private Vector2 _direction = Vector2.zero;   
        private float _magnitude = 0.0f;

        private float _knockbackDuration = 0.0f;
        private bool _inKnockback = false;

        // Constants

        // How close we can get to collisions safely
        // to prevent colliders getting stuck in each other
        private const float _offsetDistance = 0.05f;
        // Account for rounding errors
        private const float _epsilon = 0.005f;
        // maximum number of iterations to perform per collision update
        private const int _maxIterations = 2;

        // Start is called before the first frame update
        private void Start()
        {
            _hits = new RaycastHit2D[2];
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic; // ALWAYS KINEMATIC
            _rb.useFullKinematicContacts = true; // Required for collision resolves
            _filter = new ContactFilter2D();
            _filter.useTriggers = false;
        }
    
        private void FixedUpdate()
        {
            _velocityFixedDeltaTime = _velocity * Time.fixedDeltaTime;
            CollisionUpdate();

            if (_inKnockback)
            {
                _knockbackDuration -= Time.fixedDeltaTime;
                if (_knockbackDuration <= 0.0f )
                {
                    _inKnockback = false;
                    _velocity = Vector2.zero; // Set velocity to zero for now (stops the knockback)
                }
            }
        }

        private void CollisionUpdate()
        {

            if (!_collider)
            {
                // No collider so just move around
                Debug.LogWarning("No collider found for kinematic object " + this.name);
                return;
            }

            // Don't perform any work if no movement is required.
            if (_velocityFixedDeltaTime.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            // Update collision filter
            // We need to XOR an integer that has every bit as 1 with the layers we want excluded
            // which is -1
            int mask = -1 ^ _excludeMask;
            _filter.SetLayerMask(mask);

            // We need to check the movement course for this frame update so get magnitude and normals
            _magnitude = _velocityFixedDeltaTime.magnitude;
            _direction = _velocityFixedDeltaTime / _magnitude;
            RaycastHit2D hit = default;

            float iterations = _maxIterations;
            float distanceRemaining = _magnitude;
            float distance;

            // Collider data
            float colliderRadius = _collider.radius;
            Vector2 colliderOffset = _collider.offset;

            // HACKY HACK:: Stored radius does ||NOT|| account for scale so add it in here
            // Scale is a vector so we will calculate the appropriate scale via the largest value
            // Inlining this for performance
            Vector2 scaleVector = (Vector2)_collider.transform.lossyScale;
            if (scaleVector != Vector2.one)
            {
                // Non-standard scale do more math xdd
                float scaleMultiplier = Mathf.Max(Mathf.Abs(scaleVector.x), Mathf.Abs(scaleVector.y));
                colliderRadius *= scaleMultiplier;
                colliderOffset *= scaleVector;
            }
            Vector2 pos = (Vector2)_collider.transform.position + colliderOffset;

            // Do collision checks until we've either
            // A.) performed the maximum allowed iterations
            // B.) reached our desired distance
            // C.) our direction is zero (stationary)
            while (
                iterations-- > 0 && 
                distanceRemaining > _epsilon &&
                _direction.sqrMagnitude > _epsilon)
            {
                distance = distanceRemaining;
                bool hasCollision = false;

                // Cast in the trajectory for collisions
                int collisionCount = Physics2D.CircleCast(pos, colliderRadius, _direction, _filter, _hits, distance);

                // Need to do an extra check because physics casts can hit our own trigger
                for (int i = 0; i < collisionCount; ++i) 
                {
                    // Check where the hit occurred
                    hit = _hits[i];
                    if (hit.collider == _collider)
                    {
                        // Hit ourself
                        continue;
                    }
                    else
                    {
                        // Not ourself
                        hasCollision = true;
                        break;
                    }
                }

                if (hasCollision)
                {
                    float hitDistance = hit.distance;
                    Debug.DrawLine(pos, hit.point);

                    // Check if actually moving farther than the offset (i.e. not at a wall)
                    if (hitDistance > _offsetDistance)
                    {
                        // Calculate the distance to move
                        distance = hitDistance - _offsetDistance;
                        pos += _direction * distance;
                    }
                    else
                    {
                        // We aren't moving
                        distance = 0.0f;
                    }

                    // Clamp to wall collisions (slide)
                    // direction = direction - wallNormal * DotProduct(direction, wallNormal)
                    Vector2 hitNormal = hit.normal;
                    _direction -= hitNormal * Vector2.Dot(_direction, hitNormal);
                }
                else
                {
                    // No collision so move the entire distance
                    pos += _direction * distance;
                }
                distanceRemaining -= distance;
            }

            pos -= colliderOffset;  // Reaccount for collider offset
            _rb.MovePosition(pos);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            RemoveOverlap(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            RemoveOverlap(collision);
        }

        /// <summary>
        /// Resolve overlapping colliders after a fixed update
        /// </summary>
        /// <param name="collision"></param>
        private void RemoveOverlap(Collision2D collision)
        {
            // If we're filtering out the collider we hit then ignore it.
            if(_filter.IsFilteringLayerMask(collision.collider.gameObject))
            {
                return;
            }

            // Check if collision has kinematics as well
            KinematicMovement other;
            if (collision.gameObject.TryGetComponent<KinematicMovement>(out other))
            {
                // Also don't collide with objects ignoring us
                if (other.Filter.IsFilteringLayerMask(_collider.gameObject))
                {
                    return;
                }
            }

            // Calculate the collider distance.
            var colliderDistance = Physics2D.Distance(collision.otherCollider, collision.collider);

            // If we're overlapped then remove the overlap.
            if (colliderDistance.isOverlapped)
            {
                collision.otherRigidbody.position += colliderDistance.normal * (colliderDistance.distance);
            }
        }

        /// <summary>
        /// Accessor function any objects that need to move a kinematic rigidbody
        /// </summary>
        /// <param name="velocity"></param>
        public void SetVelocity(Vector2 velocity)
        {
            if (_inKnockback)
            {
                // Overruled
                return;
            }
            _velocity = velocity;
        }

        /// <summary>
        /// Accessor function for setting the collider with which to perform collision detection for this object
        /// </summary>
        /// <param name="collider"></param>
        public void SetCollider(CircleCollider2D collider)
        {
            _collider = collider;
        }

        /// <summary>
        /// Accessor function so we are setting layer filters for kinematics
        /// in the same place as velocity (instead of two different places).
        /// </summary>
        /// <param name="mask"></param>
        public void SetExcludeMask(int mask)
        {
            _excludeMask = mask;
        }

        public void ApplyKnockback(Vector2 force, float duration)
        {
            _velocity = force;
            _knockbackDuration = duration;
            _inKnockback = true;
        }
    }
}
