using Magicat.Helpers;
using UnityEngine;

namespace Magicat.Entity.Player
{
    public class Player : Entity
    {
        [SerializeField]
        private GameObject _rodObj;

        // TODO: readd playerdata structure. This is temp to test movement!!
        public float speed = 3.0f;

        public SpriteRenderer Sprite { get { return _sprite; } }
        public Animator Anim { get { return _anim; } }

        private SpriteRenderer _sprite;
        private Animator _anim;

        private void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _anim = GetComponent<Animator>();
        }

        // TODO MOVE ROD LOGIC INTO ITS OWN CLASS?
        public void UseRod(Directions dir)
        {
        }
    }
}
