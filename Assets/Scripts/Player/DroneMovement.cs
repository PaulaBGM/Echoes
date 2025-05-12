using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;

    private void Update()
    {
        // Movimiento hacia adelante y atrás
        float moveDirection = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveDirection);

        // Movimiento lateral
        float strafeDirection = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.right * strafeDirection);

        // Rotación horizontal
        float rotationY = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(0, rotationY, 0);

        // Rotación vertical
        float rotationX = -Input.GetAxis("Mouse Y") * rotationSpeed;
        transform.Rotate(rotationX, 0, 0);
    }
}