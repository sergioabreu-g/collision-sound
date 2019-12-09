using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSound
{
    [RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
    public class SoundCollider : MonoBehaviour
    {
        [SerializeField]
        private string _soundMaterial = "wood";

        [Tooltip("If set to true, sounds will only be played if the" +
            "other gameobject also has a SoundMaterial component.")]
        public bool requireAnotherSoundMaterial = false;

        private FMODUnity.StudioEventEmitter _emitter;

        private void Start() {
            _emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        }

        private void collisionSound(SoundCollider collidedWith)
        {
            Manager.collisionDetected(this, collidedWith);
        }

        private void genericCollisionSound()
        {
            Manager.genericCollisionDetected(this);
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

        public FMODUnity.StudioEventEmitter getEventEmitter()
        {
            return _emitter;
        }

        public void setSoundMaterial(string material) {
            _soundMaterial = material;
        }

        public string getSoundMaterial() {
            return _soundMaterial;
        }
    }
}
