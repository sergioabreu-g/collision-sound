using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CollisionSound
{
    struct SoundCollision {
        public SoundCollision(SoundCollider yourself, SoundCollider other, Vector3 pos, float force) {
            _yourself = yourself;
            _other = other;
            _pos = pos;
            _force = force;
        }

        public SoundCollider _yourself, _other;
        public Vector3 _pos;
        public float _force;
    }

    static class Manager
    {
        // 2D Matrix of event descriptions for the collisions between materials
        static private Dictionary<string, Dictionary<string, FMOD.Studio.EventDescription>> _soundEvents;
        static private List<SoundCollision> _collisions;
        static private string[] _materialNames;
        
        static Manager() {
            FMOD.Studio.Bank bank;
            FMODUnity.RuntimeManager.StudioSystem.getBank("bank:/Master Bank", out bank);
            FMOD.Studio.EventDescription[] eventDescriptions;
            bank.getEventList(out eventDescriptions);

            _collisions = new List<SoundCollision>();
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
                else {
                    warnDuplicate(material, collidesWith);
                    continue;
                }

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
                else warnDuplicate(material, collidesWith);
            }
            
            _materialNames = new string[_soundEvents.Keys.Count];
            _soundEvents.Keys.CopyTo(_materialNames, 0);
        }

        public static void collisionDetected(SoundCollider yourself, SoundCollider other, Vector3 pos, float force)
        {
            if (yourself == null) {
#if UNITY_EDITOR
                Debug.LogError("Bad SoundCollision encountered: 'SoundCollision.yourself' cannot be null.");
#endif
                return;
            }

            // Switch to generic collision if there's only one SoundCollider
            if (other == null) genericCollisionDetected(yourself, pos, force);

            // When two SoundColliders collide, they both detect the collision.
            // This allows to get rid of duplicates and only play it once.
            foreach (SoundCollision c in _collisions) {
                if (c._yourself == other && c._other == yourself)
                {
                    _collisions.Remove(c);
                    return;
                }
            }

            SoundCollision collision = new SoundCollision(yourself, other, pos, force);
            _collisions.Add(collision);
            playCollision(collision);
        }

        public static void genericCollisionDetected(SoundCollider yourself, Vector3 pos, float force)
        {
            playGenericCollision(new SoundCollision(yourself, null, pos, force));
        }

        private static void playCollision(SoundCollision collision)
        {
            SoundCollider yourself = collision._yourself, other = collision._other;

            // Log an error if one of the materials doesn't exist in the Fmod project
            if (!_soundEvents.ContainsKey(yourself.getSoundMaterial())) {
                errorMaterialNotFound(yourself.getSoundMaterial());
                return;
            }
            if (!_soundEvents.ContainsKey(other.getSoundMaterial())) {
                errorMaterialNotFound(other.getSoundMaterial());
                return;
            }

            // If there is a particular event defined for this collision, play it
            FMOD.Studio.EventDescription desc;
            if (_soundEvents[collision._yourself.getSoundMaterial()].TryGetValue(other.getSoundMaterial(), out desc)) {
                string path;
                desc.getPath(out path);
                playEvent(yourself, path);
            }

            else { // Else, play each sound material default sound
                string path;
                _soundEvents[yourself.getSoundMaterial()][yourself.getSoundMaterial()].getPath(out path);
                playEvent(yourself, path);

                _soundEvents[other.getSoundMaterial()][other.getSoundMaterial()].getPath(out path);
                playEvent(other, path);
            }
        }

        private static void playGenericCollision(SoundCollision collision)
        {
            SoundCollider yourself = collision._yourself;

            if (!_soundEvents.ContainsKey(yourself.getSoundMaterial())) {
                errorMaterialNotFound(yourself.getSoundMaterial());
                return;
            }

            string path;
            _soundEvents[yourself.getSoundMaterial()][yourself.getSoundMaterial()].getPath(out path);
            playEvent(yourself, path);
        }

        private static void playEvent(SoundCollider soundcollider, string path) {
            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(path);
            instance.setParameterByName("size", soundcollider.getWorldFixedSize());
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(soundcollider.transform.position));
            instance.start();
        }

        public static int materialCount() {
            return _soundEvents.Count;
        }

        public static string[] getMaterialNames() {

            return _materialNames;
        }

        private static void warnDuplicate(string first_material, string second_material) {
#if UNITY_EDITOR
            Debug.LogWarning(String.Format("There's already an event defined for the collision between '{0}' " +
                "and '{1}'. One of them will be ignored." +
                "\nCheck your FMOD Studio project and remove any duplicates to avoid shadowing."
                , first_material, second_material));
#endif
        }

        private static void errorMaterialNotFound(string material) {
#if UNITY_EDITOR
            Debug.LogError("Sound material '" + material + "' not found.\n" +
                            "Remember to re-build the FMOD Studio project after making any change.");
#endif

        }
    }
}
