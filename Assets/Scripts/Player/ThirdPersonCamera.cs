using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; // El personaje al que seguirá la cámara
    public Vector3 offset = new Vector3(0, 2, -4); // Posición relativa de la cámara
    public float sensitivityX = 200f; // Sensibilidad horizontal
    public float sensitivityY = 200f; // Sensibilidad vertical
    public float minY = -30f, maxY = 60f; // Límites de la cámara en el eje Y

    private float currentX = 0f;
    private float currentY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        currentX += mouseX;
        currentY -= mouseY;
        currentY = Mathf.Clamp(currentY, minY, maxY); // Restringe la cámara en el eje Y
    }

    void LateUpdate()
    {
        // Calcula la posición y rotación de la cámara
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Mueve la cámara a la posición deseada
        transform.position = desiredPosition;
        transform.LookAt(target.position);
    }
}
