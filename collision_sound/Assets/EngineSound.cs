using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class EngineSound : MonoBehaviour
{
    public VehicleMovement vehicleMovement;
    private FMODUnity.StudioEventEmitter _eventEmitter;

    private void Start() {
        _eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (vehicleMovement.getMotorSpeedPercent() > 0) {
            if (!_eventEmitter.IsPlaying()) _eventEmitter.Play();
            _eventEmitter.SetParameter("engine", vehicleMovement.getMotorSpeedPercent());
            _eventEmitter.SetParameter("engine_on", 1);
        }
        else
            _eventEmitter.SetParameter("engine_on", 0);
    }
}
