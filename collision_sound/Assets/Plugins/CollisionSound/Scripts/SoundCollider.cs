using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollisionSound
{
    public class SoundCollider : MonoBehaviour {
        [Header("General")]
        [Tooltip("Name of the material. There must be a folder with this exact name in the FMOD Studio project, " +
                    "and it must contain a default sound with the same name ['wood/wood']")]
        [SerializeField]
        private string _soundMaterial = "wood";

        private Collider _collider;
        private float _worldSize;

        [Tooltip("If set to true, sounds will only be played if the " +
                "other gameobject also has a SoundMaterial component.")]
        public bool requireAnotherSoundMaterial = false;

        [Tooltip("If set to true, this SoundCollider will always play its default event, " +
                "ignoring the events defined for specific material interactions.")]
        public bool alwaysPlayDefaultEvent = false;

        [Tooltip("ONLY FOR 2D COLLIDERS. If set to true, Y axis will " +
                "be treated as forward/backward when positioning the sounds in 3D" +
                ", instead of up/down.")]
        public bool yAxisIsForward2D = false;

        [Header("Sound parameters")]
        [Range(0, 1)]
        [Tooltip("Volume the events of this SoundCollider will be played this. When playing specific interaction" +
                    "events, the volume will be the average of both volumes.")]
        public float volume = 1.0f;

        [Tooltip("Mute the SoundCollider.When two SoundColliders collide, if one of them is muted(or both)" + 
            "the event will not be played.")]
        public bool mute = false;

        [Header("FMOD Studio parameters")]
        public bool sizeActive = true;
        public bool massActive = true;
        public bool velocityActive = true;

        private Dictionary<string, float> _customParams;
        private Rigidbody _rigidbody3D = null;
        private Rigidbody2D _rigidbody2D = null;

        private void Start() {
            _collider = GetComponent<Collider>();
            if (_collider == null)
                Debug.LogError("No collider found. You must have a collider in order to use SoundCollider.");

            _rigidbody3D = GetComponent<Rigidbody>();
            _rigidbody2D = GetComponent<Rigidbody2D>();

            _customParams = new Dictionary<string, float>();
        }

        /// <summary>
        /// Notifies the manager about a collision with another SoundCollider,
        /// given the collision position & relative velocity
        /// </summary>
        /// <param name="collidedWith"></param>
        /// <param name="pos">Position of the collision (the position where the sound will be played)</param>
        /// <param name="velocity">Relative velocity between the two colliding bodies</param>
        private void collisionSound(SoundCollider collidedWith, Vector3 pos, float velocity)
        {
            updateSize();
            Manager.collisionDetected(this, collidedWith, pos, velocity);
        }

        /// <summary>
        /// Notifies the manager about a collision with another GameObject without SoundColllider,
        /// given the collision position & relative velocity
        /// </summary>
        /// <param name="pos">Position of the collision (the position where the sound will be played)</param>
        /// <param name="velocity">Relative velocity between the two colliding bodies</param>
        private void genericCollisionSound(Vector3 pos, float velocity)
        {
            updateSize();
            Manager.genericCollisionDetected(this, transform.position, velocity);
        }

        /// <summary>
        /// Updates the size variables with the current size of the object's collider
        /// </summary>
        private void updateSize() {
            _worldSize = _collider.bounds.size.magnitude;
        }


        // *** COLLISION DETECTION for every possible scenario ***
        private void OnCollisionEnter(Collision collision)
        {
            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            Vector3 pos = collision.GetContact(0).point;
            float vel = collision.relativeVelocity.magnitude;

            if (collidedWith != null)
                collisionSound(collidedWith, pos, vel);
            else if (!requireAnotherSoundMaterial) genericCollisionSound(pos, vel);
        }

        private void OnTriggerEnter(Collider other)
        {
            SoundCollider collidedWith = other.gameObject.GetComponent<SoundCollider>();
            Vector3 pos = other.ClosestPointOnBounds(transform.position);
            Vector3 vel = getVelocity();
            if (other.attachedRigidbody != null) vel += other.attachedRigidbody.velocity;

            if (collidedWith != null)
                collisionSound(collidedWith, pos, vel.magnitude);
            else if (!requireAnotherSoundMaterial) genericCollisionSound(pos, vel.magnitude);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            Vector2 pos = collision.GetContact(0).point;
            float vel = collision.relativeVelocity.magnitude;

            if (collidedWith != null)
                collisionSound(collidedWith, pos, vel);
            else if (!requireAnotherSoundMaterial) genericCollisionSound(pos, vel);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            SoundCollider collidedWith = other.gameObject.GetComponent<SoundCollider>();
            Vector2 pos = other.ClosestPoint(transform.position);
            Vector3 vel = getVelocity();
            if (other.attachedRigidbody != null) vel += (Vector3) other.attachedRigidbody.velocity;

            if (collidedWith != null)
                collisionSound(collidedWith, pos, vel.magnitude);
            else if (!requireAnotherSoundMaterial) genericCollisionSound(pos, vel.magnitude);
        }

        /// <summary>
        /// Sets the material of this SoundCollider
        /// </summary>
        /// <param name="material">Material name</param>
        public void setSoundMaterial(string material) {
            
            _soundMaterial = material;
        }

        /// <summary>
        /// Sets the material of this SoundCollider
        /// </summary>
        /// <returns>Material name</returns>        
        public string getSoundMaterial() {
            return _soundMaterial;
        }

        /// <summary>
        /// Gets the magnitude of the object's size in world units
        /// </summary>
        /// <returns>magnitude of the object's size in world units</returns>
        public float getWorldSize() {
            return _worldSize;
        }

        /// <summary>
        /// Gets the mass of this GameObject's Rigidbody/Rigidbody2D (kg)
        /// </summary>
        /// <returns>The mass of this GameObject's Rigidbody/Rigidbody2D (kg)</returns>
        public float getMass() {
            if (_rigidbody3D != null) return _rigidbody3D.mass;
            else if (_rigidbody2D != null) return _rigidbody2D.mass;
            
            return 0;
        }

        /// <summary>
        /// Gets the current velocity of this GameObject's Rigidbody/Rigidbody2D (m/s)
        /// </summary>
        /// <returns>The velocity of this GameObject's Rigidbody/Rigidbody2D (m/s)</returns>
        public Vector3 getVelocity() {
            if (_rigidbody3D != null) return _rigidbody3D.velocity;
            else if (_rigidbody2D != null) return _rigidbody2D.velocity;
            
            return Vector3.zero;
        }

        /// <summary>
        /// Get a dictionary with all the custom parameters that have been already set
        /// </summary>
        /// <returns>A dictionary with all the custom parameters that have been already set</returns>
        public Dictionary<string, float> getCustomParams() {
            return _customParams;
        }

        /// <summary>
        /// Set the custom parameters of this object to the given ones
        /// </summary>
        /// <param name="customParams">Dictionary with the custom parameters as pairs [name, value]</param>
        public void setCustomParams(Dictionary<string, float> customParams) {
            _customParams = customParams;
        }

        /// <summary>
        /// Set a custom parameter to the given value
        /// </summary>
        /// <param name="parameter">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public void setCustomParam(string parameter, float value) {
            _customParams[parameter] = value;
        }

        /// <summary>
        /// Gets a custom parameter that has been already set
        /// </summary>
        /// <param name="parameter">Name of the custom parameter that has been already set</param>
        /// <returns>The current value of the parameter</returns>
        public float getCustomParam(string parameter) {
            if (_customParams.ContainsKey(parameter))
                return _customParams[parameter];

            Debug.Log("Parameter " + parameter + " not set.");
            return 0;
        }
    }
}
