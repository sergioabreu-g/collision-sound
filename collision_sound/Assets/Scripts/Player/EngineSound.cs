using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
[RequireComponent(typeof(Rigidbody))]
public class EngineSound : MonoBehaviour
{
    public VehicleMovement vehicleMovement;
    public float maxRigidbodyVelocity;

    private FMODUnity.StudioEventEmitter _eventEmitter;
    private Rigidbody _rigidbody;

    private void Start() {
        _eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (vehicleMovement.getMotorSpeedPercent() > 0) {
            if (!_eventEmitter.IsPlaying()) _eventEmitter.Play();
            _eventEmitter.SetParameter("engine", (vehicleMovement.getMotorSpeedPercent() + Mathf.Clamp(_rigidbody.velocity.magnitude/maxRigidbodyVelocity, 0, 1)) / 2);
            _eventEmitter.SetParameter("engine_on", 1);
        }
        else
            _eventEmitter.SetParameter("engine_on", 0);
    }
}
