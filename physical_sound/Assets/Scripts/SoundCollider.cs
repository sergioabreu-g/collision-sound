using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSound
{
    public class SoundCollider : MonoBehaviour
    {
        [Tooltip("Name of the material. There must be a folder with this exact name in the FMOD Studio project, " +
                    "and it must contain a default sound with the same name ['wood/wood']")]
        [SerializeField]
        private string _soundMaterial = "wood";

        [Tooltip("Object size (determined by its collider) will be multiplied by this factor when setting " +
                    "the parameter 'size' of the FMOD Event.")]
        [SerializeField]
        private float _sizeFactor = 1;

        private Collider _collider;
        private float _worldSize, _worldFixedSize;

        [Tooltip("If set to true, sounds will only be played if the" +
            "other gameobject also has a SoundMaterial component.")]
        public bool requireAnotherSoundMaterial = false;

        private void Start() {
            _collider = GetComponent<Collider>();
            if (_collider == null)
                Debug.LogWarning("No collider found. You must have a collider in order to use SoundCollider.");
        }

        private void collisionSound(SoundCollider collidedWith)
        {
            updateSize();
            Manager.collisionDetected(this, collidedWith);
        }

        private void genericCollisionSound()
        {
            updateSize();
            Manager.genericCollisionDetected(this);
        }

        private void updateSize() {
            _worldSize = _collider.bounds.size.magnitude;
            _worldFixedSize = _sizeFactor * _worldSize;
        }

        // Collision detection for every possible scenario
        private void OnCollisionEnter(Collision collision)
        {
            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
            else if (!requireAnotherSoundMaterial) genericCollisionSound();
        }

        private void OnTriggerEnter(Collider other)
        {
            SoundCollider collidedWith = other.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
            else if (!requireAnotherSoundMaterial) genericCollisionSound();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
            else if (!requireAnotherSoundMaterial) genericCollisionSound();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
            else if (!requireAnotherSoundMaterial) genericCollisionSound();
        }

        public void setSoundMaterial(string material) {
            _soundMaterial = material;
        }

        public string getSoundMaterial() {
            return _soundMaterial;
        }

        public void setSizeFactor(float sizeFactor) {
            _sizeFactor = sizeFactor;
        }

        public float getSizeFactor() {
            return _sizeFactor;
        }

        public float getWorldSize() {
            return _worldSize;
        }

        public float getWorldFixedSize() {
            return _worldFixedSize;
        }
    }
}
