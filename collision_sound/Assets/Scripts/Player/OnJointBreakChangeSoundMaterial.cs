
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionSound.SoundCollider))]
public class OnJointBreakChangeSoundMaterial : MonoBehaviour {
    public string material;
    private CollisionSound.SoundCollider _soundCollider;

    private void Start() {
        _soundCollider = GetComponent<CollisionSound.SoundCollider>();
    }

    void OnJointBreak(float breakForce) {
        _soundCollider.setSoundMaterial(material);
    }
}
