using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicalSound
{
    public class SoundCollider : MonoBehaviour
    {
        private void collisionSound(SoundCollider collidedWith)
        {
            Manager.collisionDetected(this, collidedWith);
        }


        // Collision detection for every possible scenario
        private void OnCollisionEnter(Collision collision)
        {
            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
        }

        private void OnTriggerEnter(Collider other)
        {
            SoundCollider collidedWith = other.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            SoundCollider collidedWith = collision.gameObject.GetComponent<SoundCollider>();
            if (collidedWith != null)
                collisionSound(collidedWith);
        }
    }
}
