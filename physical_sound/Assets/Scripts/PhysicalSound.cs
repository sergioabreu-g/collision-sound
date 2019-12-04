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
        
        static Manager() {
            FMOD.Studio.Bank c;
            FMODUnity.RuntimeManager.StudioSystem.getBank("bank:/Master Bank", out c);

            FMOD.Studio.EventDescription[] descriptions;
            c.getEventList(out descriptions);
            foreach (FMOD.Studio.EventDescription des in descriptions)
            {
                // LOAD MATERIAL INTERACTIONS IN AN ARRAY
            }
        }

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
            playCollisionSound(yourself, other);
        }

        public static void genericCollisionDetected(SoundCollider yourself)
        {
            playCollisionSound(yourself);
        }

        private static void playCollisionSound(SoundCollider yourself, SoundCollider other)
        {
            yourself.getEventEmitter().Play();
        }

        private static void playCollisionSound(SoundCollider yourself)
        {
            yourself.getEventEmitter().Play();
        }
    }
}
