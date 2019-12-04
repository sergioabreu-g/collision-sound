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
        // 2D Matrix of event descriptions for the collisions between materials
        static private Dictionary<string, Dictionary<string, FMOD.Studio.EventDescription>> _soundEvents;
        static private List<Tuple<SoundCollider, SoundCollider>> _collisions;
        
        static Manager() {
            FMOD.Studio.Bank bank;
            FMODUnity.RuntimeManager.StudioSystem.getBank("bank:/Master Bank", out bank);

            FMOD.Studio.EventDescription[] eventDescriptions;
            bank.getEventList(out eventDescriptions);

            _soundEvents = new Dictionary<string, Dictionary<string, FMOD.Studio.EventDescription>>();

            foreach (FMOD.Studio.EventDescription description in eventDescriptions)
            {
                string path;
                description.getPath(out path);
                path = path.Split(':')[1];

                string[] temp = path.Split('/');
                string material = temp[1];
                string collidesWith = temp[2];

                // Adds a new material it it hasn't already been added
                Dictionary<string, FMOD.Studio.EventDescription> materialEvents;
                if (!_soundEvents.TryGetValue(material, out materialEvents)) {
                    materialEvents = new Dictionary<string, FMOD.Studio.EventDescription>();
                    _soundEvents.Add(material, materialEvents);
                }
                if (!materialEvents.ContainsKey(collidesWith)) materialEvents.Add(collidesWith, description);

                if (material == collidesWith) continue;


                // Adds the other material so the same event description can be retrieved
                // no matter the order given
                // [material1][material2] = [material2][material1]

                Dictionary<string, FMOD.Studio.EventDescription> collidesWithEvents;
                if (!_soundEvents.TryGetValue(collidesWith, out collidesWithEvents)) {
                    collidesWithEvents = new Dictionary<string, FMOD.Studio.EventDescription>();
                    _soundEvents.Add(collidesWith, collidesWithEvents);
                }
                if (!collidesWithEvents.ContainsKey(material)) collidesWithEvents.Add(material, description);
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
            if (!_soundEvents.ContainsKey(yourself.soundMaterial) || !_soundEvents.ContainsKey(other.soundMaterial)) {
#if UNITY_EDITOR
                Debug.LogError("Sound material name not found");
#endif
                return;
            }

            // If there is a particular event defined for this collision, play it
            FMOD.Studio.EventDescription desc;
            if (_soundEvents[yourself.soundMaterial].TryGetValue(other.soundMaterial, out desc)) {
                string path;
                desc.getPath(out path);
                yourself.getEventEmitter().Event = path;
                yourself.getEventEmitter().Play();
            }

            else { // Else, play each sound material default sound
                string path;
                _soundEvents[yourself.soundMaterial][yourself.soundMaterial].getPath(out path);
                yourself.getEventEmitter().Event = path;

                _soundEvents[other.soundMaterial][other.soundMaterial].getPath(out path);
                other.getEventEmitter().Event = path;

                yourself.getEventEmitter().Play();
                other.getEventEmitter().Play();
            }
        }

        private static void playCollisionSound(SoundCollider yourself)
        {
            yourself.getEventEmitter().Play();
        }
    }
}
