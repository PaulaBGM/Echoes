using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 2f;
    public float maxSpeed = 10f;
    public float rotationSpeed = 2f;
    public float smoothRotationTime = 0.1f;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxVerticalAngle = 45f;

    [Header("Detection Settings")]
    [SerializeField] private float viewDistance = 15f;
    [SerializeField] private float viewAngle = 45f;
    [SerializeField] private LayerMask detectionLayer;

    private Vector3 currentVelocity;
    private float currentSpeed = 0f;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    private void Update()
    {
        HandleRotation();
        HandleMovement();
        DetectIgnea();
    }

    private void HandleMovement()
    {
        float forwardInput = 0f;
        float strafeInput = 0f;

        // Movimiento hacia adelante y atrás (W/S o Flechas)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) forwardInput = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) forwardInput = -1f;

        // Movimiento lateral (A/D o Flechas)
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) strafeInput = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) strafeInput = 1f;

        // Aplicar aceleración hacia adelante/atrás
        float targetSpeed = forwardInput * maxSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Movimiento lateral
        transform.Translate(Vector3.right * strafeInput * moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Ajustar la rotación vertical
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);

        // Ajustar la rotación horizontal
        horizontalRotation += mouseX;

        // Aplicar la rotación
        transform.localEulerAngles = new Vector3(verticalRotation, horizontalRotation, 0f);
    }

    private void DetectIgnea()
    {
        Collider[] detectedObjects = Physics.OverlapSphere(transform.position, viewDistance, detectionLayer);

        foreach (Collider obj in detectedObjects)
        {
            if (obj.CompareTag("Ignea"))
            {
                Vector3 directionToTarget = (obj.transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget <= viewAngle / 2)
                {
                    Debug.Log("Ignea en el cono de visión: " + obj.name);

                    // Activar el componente QuestMarker si existe
                    QuestMarker questMarker = obj.GetComponent<QuestMarker>();
                    if (questMarker != null)
                    {
                        questMarker.enabled = true;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewDistance;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);
    }
}
