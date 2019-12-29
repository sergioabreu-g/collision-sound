using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollisionSound
{
    public class SoundCollider : MonoBehaviour
    {
        [Tooltip("Name of the material. There must be a folder with this exact name in the FMOD Studio project, " +
                    "and it must contain a default sound with the same name ['wood/wood']")]
        [SerializeField]
        private string _soundMaterial = "wood";

        private Collider _collider;
        private float _worldSize;

        [Tooltip("If set to true, sounds will only be played if the" +
            "other gameobject also has a SoundMaterial component.")]
        public bool requireAnotherSoundMaterial = false;

        private void Start() {
            _collider = GetComponent<Collider>();
            if (_collider == null)
                Debug.LogWarning("No collider found. You must have a collider in order to use SoundCollider.");
        }

        private void collisionSound(SoundCollider collidedWith, Vector3 pos, float force)
        {
            updateSize();
            Manager.collisionDetected(this, collidedWith, transform.position, 0);
        }

        private void genericCollisionSound(Vector3 pos, float force)
        {
            updateSize();
            Manager.genericCollisionDetected(this, transform.position, 0);
        }

        private void updateSize() {
            _worldSize = _collider.bounds.size.magnitude;
        }


        // Collision detection for every possible scenario
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

        public void setSoundMaterial(string material) {
            _soundMaterial = material;
        }

        public string getSoundMaterial() {
            return _soundMaterial;
        }

        public float getWorldSize() {
            return _worldSize;
        }
    }
}
