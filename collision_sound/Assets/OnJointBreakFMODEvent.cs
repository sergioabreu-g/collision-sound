using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class OnJointBreakFMODEvent : MonoBehaviour
{
    private FMODUnity.StudioEventEmitter _emitter;

    private void Start() {
        _emitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    void OnJointBreak(float breakForce) {
        _emitter.Play();
    }
}
