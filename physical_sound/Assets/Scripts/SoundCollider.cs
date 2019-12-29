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
        public float volume = 1.0f;
        public bool mute = false;

        [Header("FMOD Studio parameters")]
        public bool sizeActive = true;
        public bool velocityActive = true;

        private void Start() {
            _collider = GetComponent<Collider>();
            if (_collider == null)
                Debug.LogWarning("No collider found. You must have a collider in order to use SoundCollider.");
        }

        /// <summary>
        /// Notifies the manager about a collision with another SoundCollider,
        /// given the collision position & relative velocity
        /// </summary>
        /// <param name="collidedWith"></param>
        /// <param name="pos">Position of the collision (the position where the sound will be played)</param>
        /// <param name="force">Relative velocity between the two colliding bodies</param>
        private void collisionSound(SoundCollider collidedWith, Vector3 pos, float force)
        {
            updateSize();
            Manager.collisionDetected(this, collidedWith, transform.position, 0);
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
            Manager.genericCollisionDetected(this, transform.position, 0);
        }

        /// <summary>
        /// Updates the size variables with the current size of the object's collider
        /// </summary>
        private void updateSize() {
            _worldSize = _collider.bounds.size.magnitude;
        }


        // *** COLLISION DETECTION for every possible scenario ***
        // Triggers will always detect their collisions with velocity 0
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
            float vel = 0;

            if (collidedWith != null)
                collisionSound(collidedWith, pos, vel);
            else if (!requireAnotherSoundMaterial) genericCollisionSound(pos, vel);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            Vector3 pos = collision.GetContact(0).point;
            float vel = collision.relativeVelocity.magnitude;

            if (collidedWith != null)
                collisionSound(collidedWith, pos, vel);
            else if (!requireAnotherSoundMaterial) genericCollisionSound(pos, vel);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            SoundCollider collidedWith = other.gameObject.GetComponent<SoundCollider>();
            Vector3 pos = other.ClosestPoint(transform.position);
            float vel = 0;

            if (collidedWith != null)
                collisionSound(collidedWith, pos, vel);
            else if (!requireAnotherSoundMaterial) genericCollisionSound(pos, vel);
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
    }
}
