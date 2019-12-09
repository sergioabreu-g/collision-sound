using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public float speed = 100;
    public float cameraSensitivity = 1;
    public UIManager ui_manager;
    public PhysicalSound.SoundCollider spawnable;
    public float spawnableForce;
    public float spawnDistance = 0.5f;

    [SerializeField]
    private string _currentMaterial;
    [SerializeField]
    private int _currentMaterialIndex = 0;

    private Rigidbody _rb;
    private Camera _cam;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = GetComponentInChildren<Camera>();
        if (_cam == null)
            Debug.LogError("Player camera is missing.");

        _currentMaterial = PhysicalSound.Manager.getMaterialNames()[_currentMaterialIndex];
        ui_manager.changeMaterial(_currentMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CameraRotation();
        MaterialSelection();
        MaterialSpawn();
    }

    void Movement() {
        Vector3 force = transform.forward;
        force.y = 0;
        _rb.AddForce(force.normalized * Input.GetAxis("Vertical") * speed);

        force = transform.right;
        force.y = 0;
        _rb.AddForce(force.normalized * Input.GetAxis("Horizontal") * speed);
    }

    void CameraRotation() {
        yaw += cameraSensitivity * Input.GetAxis("Mouse X");
        pitch -= cameraSensitivity * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
        Vector3 camRot = _cam.transform.eulerAngles;
        camRot.x = pitch;
        _cam.transform.eulerAngles = camRot;
    }

    void MaterialSelection() {
        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) < 0.05f) return;
        int extra = Input.GetAxis("Mouse ScrollWheel") > 0 ? 1 : -1;

        // Loop the index
        _currentMaterialIndex = (_currentMaterialIndex + PhysicalSound.Manager.materialCount() + extra)
                                    % PhysicalSound.Manager.materialCount();
        _currentMaterial = PhysicalSound.Manager.getMaterialNames()[_currentMaterialIndex];

        ui_manager.changeMaterial(_currentMaterial);
    }

    void MaterialSpawn() {
        if (Input.GetButtonDown("Fire1")) {
            PhysicalSound.SoundCollider spawned = Instantiate(spawnable);
            spawned.transform.position = _cam.transform.position + _cam.transform.forward * spawnDistance;
            spawned.setSoundMaterial(_currentMaterial);
            Rigidbody rb = spawned.GetComponent<Rigidbody>();
            rb.AddForce(_cam.transform.forward * spawnableForce);
        }
    }
}
