using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSound
{
    [RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
    public class SoundCollider : MonoBehaviour
    {
        [FMODUnity.EventRef]
        public string soundMaterial = "Wood";

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
            if (!requireAnotherSoundMaterial)
            {
                genericCollisionSound();
                return;
            }

            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!requireAnotherSoundMaterial)
            {
                genericCollisionSound();
                return;
            }

            SoundCollider collidedWith = other.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!requireAnotherSoundMaterial)
            {
                genericCollisionSound();
                return;
            }

            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!requireAnotherSoundMaterial)
            {
                genericCollisionSound();
                return;
            }

            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
        }

        public FMODUnity.StudioEventEmitter getEventEmitter()
        {
            return _emitter;
        }
    }
}
