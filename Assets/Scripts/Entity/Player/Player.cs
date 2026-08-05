using Magicat.Helpers;
using UnityEngine;

namespace Magicat.Entity.Player
{
    public class Player : Entity
    {
        // TODO: readd playerdata structure. This is temp to test movement!!
        public float speed = 3.0f;

        public SpriteRenderer Sprite { get { return _sprite; } }
        public Animator Anim { get { return _anim; } }

        private SpriteRenderer _sprite;
        private Animator _anim;
        private Rod _rod;

        private void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _anim = GetComponent<Animator>();
            _rod = GetComponentInChildren<Rod>();
        }

        // TODO: In the future we will have enemies copy abilities as their own classes
        // so this function should dictate what we're using based on our current ability equips
        // (e.g. base copy ability, use fireball, dash, etc.)
        public void UseRod(Directions dir)
        {
            _rod.UseCopyAbility(dir);
        }
    }
}
