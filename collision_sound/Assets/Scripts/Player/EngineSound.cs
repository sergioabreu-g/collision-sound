using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
[RequireComponent(typeof(Rigidbody))]
public class EngineSound : MonoBehaviour
{
    public PlayerVehicle vehicleMovement;
    public float maxRigidbodyVelocity;
    public float accelerationRatio = 1;

    private FMODUnity.StudioEventEmitter _eventEmitter;
    private Rigidbody _rigidbody;
    private float motorPercent;

    private void Start() {
        _eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float targetMotorPercent = (Mathf.Abs(vehicleMovement.getMotorSpeedPercent()) + Mathf.Clamp(_rigidbody.velocity.magnitude / maxRigidbodyVelocity, 0, 1)) / 2;
        motorPercent = Mathf.Lerp(motorPercent, targetMotorPercent, accelerationRatio * Time.deltaTime);
        _eventEmitter.SetParameter("engine", motorPercent);

        if (Mathf.Abs(vehicleMovement.getMotorSpeedPercent()) > 0) {
            if (!_eventEmitter.IsPlaying()) {
                _eventEmitter.Stop();
                _eventEmitter.Play();
            }
            _eventEmitter.SetParameter("engine_on", 1);
        }
        else
            _eventEmitter.SetParameter("engine_on", 0);
    }
}
