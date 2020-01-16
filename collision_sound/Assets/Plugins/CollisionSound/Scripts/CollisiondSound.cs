using System;
using System.Collections.Generic;
using UnityEngine;

namespace CollisionSound
{
    /// <summary>
    /// Contains the parameters that every collision must receive in order
    /// to play the sound events
    /// </summary>
    struct SoundCollision {
        public SoundCollision(SoundCollider yourself, SoundCollider other, Vector3 pos, float velocity) {
            _yourself = yourself;
            _other = other;
            _pos = pos;
            _velocity = velocity;
        }

        public SoundCollider _yourself, _other;
        public Vector3 _pos;
        public float _velocity;
    }

    public static class Manager
    {
        // 2D Matrix of event descriptions for the collisions between materials
        static private Dictionary<string, Dictionary<string, FMOD.Studio.EventDescription>> _soundEvents;
        static private List<SoundCollision> _collisions;
        static private string[] _materialNames;
        
        static Manager() {
            FMOD.Studio.Bank bank;
            FMOD.RESULT result = FMODUnity.RuntimeManager.StudioSystem.getBank("bank:/SoundMaterials", out bank);
            if (result != FMOD.RESULT.OK) {
                Debug.LogError("Collision Sound couldn't be initialized, error getting the" +
                               "'SoundMaterials' bank from the FMOD Studio project.");
                return;
            }

            FMOD.Studio.EventDescription[] eventDescriptions;
            bank.getEventList(out eventDescriptions);

            _collisions = new List<SoundCollision>();
            _soundEvents = new Dictionary<string, Dictionary<string, FMOD.Studio.EventDescription>>();

            // Iterate through each event path and store all materials and its interactions
            foreach (FMOD.Studio.EventDescription description in eventDescriptions)
            {
                string path;
                description.getPath(out path);
                path = path.Split(':')[1];

                string[] temp = path.Split('/');
                string root = temp[1];

                if (root != "SoundMaterials") continue;

                string material = temp[2];
                string collidesWith = temp[3];

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

            if (_soundEvents.Count == 0)
                Debug.LogWarning("No collision events found, check your FMOD Studio project."); 
        }

        /// <summary>
        /// Function called from the SoundCollider script when it detects a collision
        /// between two SoundColliders
        /// </summary>
        /// <param name="yourself">First SoundCollider involved in the collision</param>
        /// <param name="other">Second SoundCollider involved in the collision</param>
        /// <param name="pos">World position of the collision</param>
        /// <param name="velocity">Relative velocity of the colliding objects</param>
        public static void collisionDetected(SoundCollider yourself, SoundCollider other, Vector3 pos, float velocity)
        {
            if (yourself == null) {
#if UNITY_EDITOR
                Debug.LogError("Bad SoundCollision encountered: 'SoundCollision.yourself' cannot be null.");
#endif
                return;
            }

            // Switch to generic collision if there's only one SoundCollider
            if (other == null) genericCollisionDetected(yourself, pos, velocity);

            // When two SoundColliders collide, they both detect the collision.
            // This allows to get rid of duplicates and only play it once.
            foreach (SoundCollision c in _collisions) {
                if (c._yourself == other && c._other == yourself)
                {
                    _collisions.Remove(c);
                    return;
                }
            }

            SoundCollision collision = new SoundCollision(yourself, other, pos, velocity);
            _collisions.Add(collision);
            playCollision(collision);
        }

        /// <summary>
        /// Function called from the SoundCollider script when it detects a collision
        /// with only one SoundCollider involved
        /// </summary>
        /// <param name="yourself">SoundCollider that collided</param>
        /// <param name="pos">World position of the collision</param>
        /// <param name="force">Relative velocity of the colliding objects</param>
        public static void genericCollisionDetected(SoundCollider yourself, Vector3 pos, float force)
        {
            playGenericCollision(new SoundCollision(yourself, null, pos, force));
        }

        /// <summary>
        /// Plays a collision between two SoundColliders
        /// </summary>
        /// <param name="collision">SoundCollision struct with all the necessary parameters</param>
        private static void playCollision(SoundCollision collision)
        {
            SoundCollider yourself = collision._yourself, other = collision._other;

            if (yourself.mute || other.mute || (yourself.volume <= 0 && other.volume <= 0))
                return;

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
            if ((!yourself.alwaysPlayDefaultEvent && !other.alwaysPlayDefaultEvent) &&
                _soundEvents[collision._yourself.getSoundMaterial()].TryGetValue(other.getSoundMaterial(), out desc)) {
                string path;
                desc.getPath(out path);

                FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(path);

                // Set the instance built-in parameters (if the SoundCollider has marked them as active)
                if (yourself.sizeActive && other.sizeActive)
                    instance.setParameterByName("size", yourself.getWorldSize() + other.getWorldSize()/2);
                if (yourself.velocityActive && other.velocityActive)
                    instance.setParameterByName("velocity", collision._velocity);
                if (yourself.massActive && other.massActive)
                    instance.setParameterByName("mass", (yourself.getMass() + other.getMass())/ 2);

                // Set the custom parameters (iterate both dictionaries and average the common entries)
                foreach (KeyValuePair<string, float> customParam in yourself.getCustomParams()) {
                    float auxValue = customParam.Value;
                    if (other.getCustomParams().ContainsKey(customParam.Key))
                        auxValue = (auxValue + other.getCustomParam(customParam.Key)) / 2;

                    instance.setParameterByName(customParam.Key, auxValue);
                }
                foreach (KeyValuePair<string, float> customParam in other.getCustomParams()) {
                    float auxValue = customParam.Value;
                    if (yourself.getCustomParams().ContainsKey(customParam.Key))
                        auxValue = (auxValue + yourself.getCustomParam(customParam.Key)) / 2;

                    instance.setParameterByName(customParam.Key, auxValue);
                }

                Vector3 pos;
                if (yourself.yAxisIsForward2D || other.yAxisIsForward2D)
                    pos = new Vector3(collision._pos.x, 0, collision._pos.y);
                else
                    pos = new Vector3(collision._pos.x, collision._pos.y, collision._pos.z);

                instance.setVolume(Mathf.Clamp((yourself.volume + other.volume) / 2, 0, 1));
                instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));
                instance.start();
            }
            else { // Else, play each sound material default sound
                playGenericCollision(collision);
                collision._yourself = collision._other;
                playGenericCollision(collision);
            }
        }

        /// <summary>
        /// Plays a generic collision
        /// </summary>
        /// <param name="collision">SoundCollision struct with all the necessary parameters</param>
        private static void playGenericCollision(SoundCollision collision)
        {
            SoundCollider yourself = collision._yourself;

            if (yourself.mute || yourself.volume <= 0)
                return;

            // Check if the material of the SoundCollider exists
            if (!_soundEvents.ContainsKey(yourself.getSoundMaterial())) {
                errorMaterialNotFound(yourself.getSoundMaterial());
                return;
            }

            // Get the path to its default event
            string path;
            _soundEvents[yourself.getSoundMaterial()][yourself.getSoundMaterial()].getPath(out path);

            FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(path);

            // Set the instance built-in parameters (if the SoundCollider has marked them as active)
            if (yourself.sizeActive) instance.setParameterByName("size", yourself.getWorldSize());
            if (yourself.velocityActive) instance.setParameterByName("velocity", collision._velocity);
            if (yourself.massActive) instance.setParameterByName("mass", yourself.getMass());

            // Set the custom parameters
            foreach (KeyValuePair<string, float> customParam in yourself.getCustomParams()) {
                instance.setParameterByName(customParam.Key, customParam.Value);
            }

            Vector3 pos = collision._pos;
            if (yourself.yAxisIsForward2D) {
                pos.y = collision._pos.z;
                pos.z = collision._pos.y;
            }

            instance.setVolume(Mathf.Clamp((yourself.volume) / 2, 0, 1));
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(pos));
            instance.start();
        }

        /// <summary>
        /// Gets the total number of materials
        /// </summary>
        /// <returns>Total number of materials</returns>
        public static int materialCount() {
            return _soundEvents.Count;
        }

        /// <summary>
        /// Gets an array with all the material names
        /// </summary>
        /// <returns>Array with all the material names</returns>
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
