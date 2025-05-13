using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class AimStateManager : MonoBehaviour
{
    private PlayerBehavior playerBehavior;
    public string xAxisInput = "Mouse X";
    public string yAxisInput = "Mouse Y";
    public Transform camFollowPos;

    [Header("Smooth Settings")]
    [SerializeField] private float smoothSpeed = 10f;

    private float currentXRotation;
    private float currentYRotation;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera droneCamera;
    [SerializeField] private KeyCode switchCameraKey = KeyCode.E;

    [Header("Drone Settings")]
    [SerializeField] private GameObject droneObject;
    private bool isDroneCameraActive = false;

    void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
        currentXRotation = transform.eulerAngles.y;
        currentYRotation = camFollowPos.localEulerAngles.x;

        mainCamera.Priority = 10;
        droneCamera.Priority = 0;
        
        if (droneObject != null)
        {
            droneObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerBehavior == null || playerBehavior.IsDead) return;

        if (Input.GetKeyDown(switchCameraKey))
        {
            ToggleCamera();
        }
    }

    private void LateUpdate()
    {
        if (playerBehavior == null || playerBehavior.IsDead) return;
        SmoothRotate(camFollowPos);
    }

    public void ShootCamera()
    {
        currentYRotation = Mathf.Clamp(currentYRotation, -11f, 3f);
    }

    public void NormalCamera()
    {
        currentYRotation = Mathf.Clamp(currentYRotation, -15f, 15f);
    }

    private void ToggleCamera()
    {
        isDroneCameraActive = !isDroneCameraActive;
        mainCamera.Priority = isDroneCameraActive ? 0 : 10;
        droneCamera.Priority = isDroneCameraActive ? 10 : 0;

        if (droneObject != null)
        {
            droneObject.SetActive(isDroneCameraActive);
        }

        // Desactivar PlayerMove si la cámara principal no está activa
        PlayerMove playerMove = GetComponent<PlayerMove>();
        if (playerMove != null)
        {
            playerMove.enabled = !isDroneCameraActive;
        }
    }



    private void SmoothRotate(Transform location)
    {
        float mouseX = Input.GetAxis(xAxisInput) * smoothSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis(yAxisInput) * smoothSpeed * Time.deltaTime;

        currentXRotation += mouseX;
        currentYRotation -= mouseY;

        currentYRotation = Mathf.Clamp(currentYRotation, -15f, 15f);

        location.localRotation = Quaternion.Euler(currentYRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, currentXRotation, 0f);
    }
}
