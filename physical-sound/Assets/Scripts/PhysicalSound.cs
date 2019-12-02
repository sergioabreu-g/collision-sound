using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PhysicalSound
{
    static class Manager
    {
        static private List<Tuple<SoundCollider, SoundCollider>> _collisions;
        
        public static void collisionDetected(SoundCollider yourself, SoundCollider other)
        {
            if (_collisions == null) _collisions = new List<Tuple<SoundCollider, SoundCollider>>();

            Tuple<SoundCollider, SoundCollider> collision = new Tuple<SoundCollider, SoundCollider>(yourself, other);

            foreach (Tuple<SoundCollider, SoundCollider> c in _collisions) {
                if (c.Item1 == collision.Item2 && c.Item2 == collision.Item1)
                {
                    _collisions.Remove(c);
                    return;
                }
            }

            _collisions.Add(collision);
            playCollisionSound();
        }

        private static void playCollisionSound()
        {
            Debug.Log("Playing collision sound " + _collisions.Count()); // TESTING
        }
    }
}
