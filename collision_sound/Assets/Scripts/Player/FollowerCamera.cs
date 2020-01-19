using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates the player body and its camera based on mouse input
/// </summary>
public class FollowerCamera : MonoBehaviour
{
    public GameObject whoToFollow;
    public Vector3 offset;

    public float sensivity = 100;
    public float verticalClampAngle = 55;
    public float horizontalClampAngle = 60;

    public bool autoCenter = true;
    public float autoCenterStrength = 1;

    private Vector3 _cameraRotation;

    private void Start() {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 auxOffset = whoToFollow.transform.forward;
        auxOffset *= offset.z;
        auxOffset.x += offset.x;
        auxOffset.y += offset.y;
        transform.position = whoToFollow.transform.position + auxOffset;
        transform.LookAt(whoToFollow.transform);
        Quaternion centeredQuat = transform.rotation;

        _cameraRotation.x += -Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime;
        _cameraRotation.y += Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;

        _cameraRotation.x = Mathf.Clamp(_cameraRotation.x, -verticalClampAngle, verticalClampAngle);
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y, -horizontalClampAngle, horizontalClampAngle);

        if (autoCenter)
            _cameraRotation = Vector3.Lerp(_cameraRotation, Vector3.zero, autoCenterStrength * Time.deltaTime);
        transform.localEulerAngles += _cameraRotation;

    }
}
