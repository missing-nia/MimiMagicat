using UnityEngine;

namespace Magicat.Entity.Player
{
    public class Player : Entity
    {
        // TODO: readd playerdata structure. This is temp to test movement!!
        public float speed = 3.0f;

        public SpriteRenderer Sprite { get { return _sprite; } }

        private SpriteRenderer _sprite;
        private void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

    }
}
